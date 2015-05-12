using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using Distant;
using Majestic12;
using System.Net;

namespace IsmUzParser
{
    class IsmUzBackgroundParser
    {
        /// <summary>
        /// Класс для парсинга HTML-данных
        /// </summary>
        HTMLparser parser;
        /// <summary>
        /// Класс для работы с HTTP-запросами
        /// </summary>
        IHttpRequests http;
        /// <summary>
        /// Извещатель состояния работы
        /// </summary>
        WorkerState state;
        /// <summary>
        /// Список имен
        /// </summary>
        List<IsmModel> ismList;
        /// <summary>
        /// URL-адрес сайта с именами (http://ism.uz)
        /// </summary>
        string ismUzUrl;
        /// <summary>
        /// Конструктор по-умолчанию
        /// </summary>
        /// <param name="ismUzWebSiteUrl">URL-адрес сайта с именами</param>
        /// <param name="useProxy">Использовать прокси</param>
        /// <param name="proxyHost">Прокси-сервер</param>
        /// <param name="proxyUser">Пользователь прокси</param>
        /// <param name="proxyPassword">Пароль прокси</param>
        public IsmUzBackgroundParser(string ismUzWebSiteUrl, bool useProxy = false, string proxyHost = "", string proxyUser = "", string proxyPassword = "")
        {
            ismUzUrl = ismUzWebSiteUrl;

            // Инициализация HttpRequests
            http = new PureDotNetHttpRequests();

            if (useProxy)
                http.EnableProxy(proxyHost, proxyUser, proxyPassword, true);

            // Инициализация HTML-парсера
            parser = new HTMLparser();

            parser.bAutoKeepComments = false;
            parser.bAutoKeepScripts = false;
            parser.bAutoMarkClosedTagsWithParamsAsOpen = true;
            parser.bCompressWhiteSpaceBeforeTag = true;
            parser.bKeepRawHTML = false;
            parser.bDecodeEntities = true;
            parser.bDecodeMiniEntities = true;
            parser.InitMiniEntities();
            parser.SetEncoding(Encoding.UTF8);
        }

        /// <summary>
        /// Получить список имен
        /// </summary
        public IList<IsmModel> GetIsmList()
        {
            return ismList;
        }
        /// <summary>
        /// Запустить работу
        /// </summary>
        /// <param name="worker">Класс <c>BackgroundWorker</c></param>
        /// <param name="e">Класс <c>DoWorkEventArgs</param>
        public void DoWork(BackgroundWorker worker, DoWorkEventArgs e)
        {
            string applicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
            string logFileName = applicationPath + "parser.log";

            int percentage = 0;

            TextWriter log;

            using (log = new StreamWriter(logFileName, false, Encoding.UTF8))
            {
                state = new WorkerState(log, worker);

                state.SetState(++percentage, "Log file: " + logFileName, WORKER_STATE_STATUS.NORMAL);
                state.SetState(++percentage, "Starting logging and parsing...", WORKER_STATE_STATUS.NORMAL);

                state.SetState(++percentage, "URL: " + ismUzUrl, WORKER_STATE_STATUS.NORMAL);
                state.SetState(++percentage, "Parsing letters...", WORKER_STATE_STATUS.NORMAL);

                IList<string> letters = ParseAndGetLetters();

                // Проверить, получены ли буквы
                if (letters == null || letters.Count == 0)
                {
                    // Буквы не получены, остановим работу
                    state.SetState(100, "Letters not retreived", WORKER_STATE_STATUS.ERROR);
                    e.Result = this;
                    return;
                }
                
                percentage += 5;
                state.SetState(percentage, String.Format("Letters retreived: {0} letters", letters.Count), WORKER_STATE_STATUS.NORMAL);

                state.SetState(++percentage, "Parsing names...", WORKER_STATE_STATUS.NORMAL);

                int totalNamesCount = 0;
                int eachLetterWorkPercent = (100 - percentage) / letters.Count;

                ismList = new List<IsmModel>();

                foreach (string letter in letters)
                {
                    IList<IsmModel> parsedNames = ParseAndGetNames(letter);

                    percentage += eachLetterWorkPercent;

                    if (percentage > 100)
                        percentage = 100;

                    if (parsedNames != null && parsedNames.Count > 0)
                    {
                        totalNamesCount += parsedNames.Count;
                        ismList.AddRange(parsedNames);
                        state.SetState(percentage, String.Format("Names for letter {0}: {1} names", letter, parsedNames.Count), WORKER_STATE_STATUS.NORMAL);
                    }
                    else
                    {
                        state.SetState(percentage, String.Format("Names for letter {0}: Nothing parsed", letter), WORKER_STATE_STATUS.WARNING);
                    }
                }

                state.SetState(percentage, String.Format("Parsing finished. Total parsed names: {0} names", totalNamesCount), WORKER_STATE_STATUS.NORMAL);

                state.SetState(100, "Work done.", WORKER_STATE_STATUS.NORMAL);
            }

            e.Result = this;
        }
        /// <summary>
        /// Получить список букв, на которые начинаются имена
        /// </summary>
        /// <returns>При успешной обработке возваратит список букв, иначе <c>null</c></returns>
        private IList<string> ParseAndGetLetters()
        {
            // Отправить GET запрос и получить байты страницы
            byte[] pageBytes = http.SendGetRequest(ismUzUrl);
            
            if (pageBytes != null)
            {
                List<string> parsedLetters = new List<string>();

                parser.Init(pageBytes);
                parser.SetEncoding(Encoding.UTF8);

                HTMLchunk chunk = null;

                string letter = "";

                // начать парсинг
                while ((chunk = parser.ParseNext()) != null)
                {
                    if (chunk.oType ==  HTMLchunkType.OpenTag)
                    {
                        if (chunk.sTag.Equals("ul", StringComparison.InvariantCultureIgnoreCase) 
                            && chunk.GetParamValue("class").Equals("alphabet"))
                        {
                            chunk = null;

                            // html list open tag for letters found,
                            // let's parse letters
                            while ((chunk = parser.ParseNext()) != null)
                            {
                                if (chunk.oType == HTMLchunkType.OpenTag)
                                {
                                    if (chunk.sTag.Equals("a", StringComparison.InvariantCultureIgnoreCase) 
                                        && chunk.GetParamValue("href").Contains("letters"))
                                    {
                                        HTMLchunk t_chunk = parser.ParseNext();

                                        if (t_chunk != null && t_chunk.oType == HTMLchunkType.Text)
                                        {
                                            letter = WebUtility.HtmlDecode(t_chunk.oHTML).Trim();

                                            // add letter to list
                                            parsedLetters.Add(letter);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    chunk = null;
                    letter = "";
                }

                return parsedLetters;
            }
            else
                return null;
        }
        /// <summary>
        /// Получить список имен по начальной букве
        /// </summary>
        /// <param name="letter">Начальная буква имени</param>
        /// <returns>При успешной обработке возваратит список имен, иначе <c>null</c></returns>
        private IList<IsmModel> ParseAndGetNames(string letter)
        {
            
            return null;
        }
    }
}
