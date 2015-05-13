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
                http.EnableProxy(proxyHost, proxyUser, proxyPassword, false);

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

        public IsmUzBackgroundParser(string ismUzWebSiteUrl, bool useDefaultProxy = false, bool useNtlm = false)
        {
            ismUzUrl = ismUzWebSiteUrl;

            // Инициализация HttpRequests
            http = new PureDotNetHttpRequests();

            if (useDefaultProxy)
                http.EnableDefaultProxy(useNtlm);

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

                // Получем имена каждой буквы
                foreach (string letter in letters)
                {
                    if (letter != "O‘")
                        continue;

                    state.SetState(percentage, String.Format("Parsing names for letter {0}...", letter), WORKER_STATE_STATUS.NORMAL);

                    //IList<IsmModel> parsedNames = ParseAndGetNamesList(letter);
                    IList<string> parsedNames = ParseAndGetNamesList(letter);

                    percentage += eachLetterWorkPercent;

                    if (percentage > 100)
                        percentage = 100;

                    // Имена для данной буквы получены
                    if (parsedNames != null && parsedNames.Count > 0)
                    {
                        totalNamesCount += parsedNames.Count;
                        //ismList.AddRange(parsedNames);
                        state.SetState(percentage, String.Format("Names for letter {0}: {1} names", letter, parsedNames.Count), WORKER_STATE_STATUS.NORMAL);
                        int parsedNamesCount = 0;

                        IList<IsmModel> parsedIsmList = new List<IsmModel>();

                        foreach (string name in parsedNames)
                        {
                            parsedNamesCount++;

                            //state.SetState(percentage, String.Format("[{0} of {1}] Parsing name: {2}", parsedNamesCount, parsedNames.Count, name), WORKER_STATE_STATUS.NORMAL);
                            IsmModel ism = ParseAndGetName(letter, name);

                            if (ism != null)
                            {
                                parsedIsmList.Add(ism);
                                state.SetState(percentage, String.Format("[{0} of {1} : {2}%] PARSED name: {3}", parsedNamesCount, parsedNames.Count, (parsedNamesCount * 100 / parsedNames.Count), name), WORKER_STATE_STATUS.NORMAL);
                            }
                            else
                            {
                                state.SetState(percentage, String.Format("[{0} of {1} : {2}%] NOT PARSED name: {3}", parsedNamesCount, parsedNames.Count, (parsedNamesCount * 100 / parsedNames.Count), name), WORKER_STATE_STATUS.WARNING);
                            }

                            if (parsedNamesCount > 5)
                                break;
                        }

                        ismList.AddRange(parsedIsmList);
                    }
                    // Имена для данной буквы не получены
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
            string page = Encoding.UTF8.GetString(pageBytes);

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

                            // найден тэг начала списка букв,
                            // парсим элементы списка
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
                                            // декодируем строку
                                            letter = WebUtility.HtmlDecode(t_chunk.oHTML).Trim();

                                            // добавим букву в список
                                            parsedLetters.Add(letter);
                                        }
                                    }
                                }
                                else if (chunk.oType == HTMLchunkType.CloseTag)
                                {
                                    if (chunk.sTag.Equals("ul", StringComparison.InvariantCultureIgnoreCase))
                                        break;
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
        //private IList<IsmModel> ParseAndGetNamesList(string letter)
        private IList<string> ParseAndGetNamesList(string letter)
        {
            // Отправить GET запрос и получить байты страницы
            //byte[] pageBytes = http.SendGetRequest(ismUzUrl + "/letters/" + WebUtility.UrlEncode(letter));
            byte[] pageBytes = http.SendGetRequest(ismUzUrl + "/letters/" + letter.Replace("‘", Uri.EscapeUriString("‘")));
            if (pageBytes != null)
            {
                //List<IsmModel> parsedNames = new List<IsmModel>();
                List<string> parsedNames = new List<string>();

                parser.Init(pageBytes);
                parser.SetEncoding(Encoding.UTF8);

                HTMLchunk chunk = null;

                string ism_name = "";

                // начать парсинг
                while ((chunk = parser.ParseNext()) != null)
                {
                    if (chunk.oType == HTMLchunkType.OpenTag)
                    {
                        if (chunk.sTag.Equals("ol", StringComparison.InvariantCultureIgnoreCase)
                            && chunk.GetParamValue("class").Contains("names-list"))
                        {
                            chunk = null;

                            // найден тэг начала списка имен,
                            // парсим элементы списка
                            while ((chunk = parser.ParseNext()) != null)
                            {
                                if (chunk.oType == HTMLchunkType.OpenTag)
                                {
                                    if (chunk.sTag.Equals("a", StringComparison.InvariantCultureIgnoreCase)
                                        && chunk.GetParamValue("href").Contains("names"))
                                    {
                                        HTMLchunk t_chunk = parser.ParseNext();

                                        if (t_chunk != null && t_chunk.oType == HTMLchunkType.Text)
                                        {
                                            // декодируем строку с именем
                                            ism_name = WebUtility.HtmlDecode(t_chunk.oHTML).Trim();

                                            if (ism_name != null && ism_name.Length > 0)
                                                // добавим имя в список
                                                parsedNames.Add(ism_name);

                                            // Получим инфо об имени
                                            //IsmModel name = ParseAndGetName(letter, ism_name);

                                            /*if (name != null)
                                            {
                                                // добавим имя в список
                                                parsedNames.Add(name);
                                            }*/
                                        }
                                    }
                                }
                                else if (chunk.oType == HTMLchunkType.CloseTag)
                                {
                                    if (chunk.sTag.Equals("ol", StringComparison.InvariantCultureIgnoreCase))
                                        break;
                                }

                                ism_name = "";
                            }
                        }
                    }
                }

                return parsedNames;
            }
            else
                return null;
        }
        /// <summary>
        /// Получить информацию об имени
        /// </summary>
        /// <param name="name">Имя</param>
        /// <returns>Возвратит информацию об имени <c>IsmModel</c>, иначе <c>null</c></returns>
        private IsmModel ParseAndGetName(string letter, string name)
        {
            // Отправить GET запрос и получить байты страницы
            //byte[] pageBytes = http.SendGetRequest(ismUzUrl + "/names/" + WebUtility.UrlEncode(name));
            byte[] pageBytes = http.SendGetRequest(ismUzUrl + "/names/" + name.Replace("‘", Uri.EscapeUriString("‘")));

            if (pageBytes != null)
            {
                string page = Encoding.UTF8.GetString(pageBytes);

                IsmModel parsedName = null;

                parser.Init(pageBytes);
                parser.SetEncoding(Encoding.UTF8);

                HTMLchunk chunk = null;

                string ism_letter = letter;
                GENDER ism_gender = GENDER.MALE;
                string ism_name = name;
                string ism_origin = "";
                string ism_meaning = "";

                bool found = false;

                // начать парсинг
                while (!found && (chunk = parser.ParseNext()) != null )
                {
                    if (chunk.oType == HTMLchunkType.OpenTag)
                    {
                        if (chunk.sTag.Equals("div", StringComparison.InvariantCultureIgnoreCase)
                            && chunk.GetParamValue("class").Contains("male") && chunk.GetParamValue("class").Contains("names"))
                        {
                            // получим пол
                            ism_gender = chunk.GetParamValue("class").Contains("female") ? GENDER.FEMALE : GENDER.MALE;

                            chunk = null;

                            while (!found && (chunk = parser.ParseNext()) != null)
                            {
                                if (chunk.oType == HTMLchunkType.OpenTag)
                                {
                                    HTMLchunk t_chunk;

                                    switch (chunk.sTag)
                                    {
                                            // происхождение
                                        case "h4":
                                            t_chunk = parser.ParseNext();

                                            if (t_chunk != null && t_chunk.oType == HTMLchunkType.Text)
                                            {
                                                ism_origin = WebUtility.HtmlDecode(t_chunk.oHTML);
                                                continue;
                                            }
                                            break;
                                            // значение
                                        case "p":
                                            t_chunk = parser.ParseNext();
                                            t_chunk = parser.ParseNext(); // double <p> tag

                                            if (t_chunk != null && t_chunk.oType == HTMLchunkType.Text)
                                            {
                                                if (t_chunk.oHTML.Trim().Length > 0)
                                                {
                                                    ism_meaning = WebUtility.HtmlDecode(t_chunk.oHTML);
                                                    found = true;
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (found)
                    parsedName = new IsmModel(ism_letter, ism_gender, ism_name, ism_origin, ism_meaning);

                return parsedName;
            }
            else
                return null;
        }
    }
}
