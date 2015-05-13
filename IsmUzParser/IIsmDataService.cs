using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsmUzParser
{
    /// <summary>
    /// Класс-сервис для работы с БД Ism
    /// </summary>
    public interface IIsmDataService
    {
        /// <summary>
        /// Подключится к базе данных Ism
        /// </summary>
        /// <param name="dataSourceName">Имя источника данных</param>
        /// <param name="userName">Пользователь</param>
        /// <param name="password">Пароль</param>
        /// <returns>При успешном подключении возвратит <c>true</c></returns>
        bool Connect(string dataSourceName, string userName, string password);
        /// <summary>
        /// Отключится от БД
        /// </summary>
        void Disconnect();
        /// <summary>
        /// Создать БД
        /// </summary>
        /// <param name="name">Имя БД</param>
        /// <returns></returns>
        bool CreateDatabase(string name);
        /// <summary>
        /// Создать модель данных БД Ism, если таковая отсутствует
        /// </summary>
        /// <returns>При успешном создании модели вернет <c>true</c></returns>
        bool CreateDataModelIfNotExists();
        /// <summary>
        /// Получить весь список IsmModel
        /// </summary>
        /// <returns>Возвратит список <c>IList&lt;IsmModel&gt;</c> при успешном выполнении, 
        /// или <c>null</c> в другом случае</returns>
        IList<IsmModel> GetAllIsm();
        /// <summary>
        /// Получить IsmModel по имени (IsmModel.Name)
        /// </summary>
        /// <returns>Возвратит объект <c>IsmModel</c> при успешном выполнении, 
        /// или <c>null</c> в другом случае</returns>
        IsmModel GetIsmInfoByName(string name);
        /// <summary>
        /// Получить отфильтрованный список IsmModel,
        /// при этом фильтр использует оператор SQL "LIKE".
        /// </summary>
        /// <param name="letter">Начальная буква имени</param>
        /// <param name="gender">Пол</param>
        /// <param name="name">Имя</param>
        /// <param name="meaning">Значение имени</param>
        /// <param name="origin">Происхождение</param>
        /// <returns>Возвратит список <c>IList&lt;IsmModel&gt;</c> при успешном выполнении, 
        /// или <c>null</c> в другом случае</returns>
        IList<IsmModel> GetFilteredIsmList(string letter, string name, GENDER gender, string meaning, string origin);
        /// <summary>
        /// Создать запись IsmModel в БД
        /// </summary>
        /// <param name="ism"><c>IsmModel</c></param>
        /// <returns>При успешном подключении возвратит <c>true</c></returns>
        bool CreateIsm(IsmModel ism);
        /// <summary>
        /// Удалить запись IsmModel (см. <c>DeleteIsm(string)</c>)
        /// </summary>
        /// <param name="ism">IsmModel</param>
        /// <returns>Возвратит результат метода<c>DeleteIsm(ism.Name)</c></returns>
        bool DeleteIsm(IsmModel ism);
        /// <summary>
        /// Удалить запись IsmModel по имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns>При успешном удалении возвратит <c>true</c></returns>
        bool DeleteIsm(string name);
        /// <summary>
        /// Удалить все записи IsmModel
        /// </summary>
        /// <returns>При успешном удалении возвратит <c>true</c></returns>
        bool DeleteAllIsm();
    }
}
