using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.SQLite.Linq;
using System.Collections.Specialized;

namespace IsmUzParser
{
    public class SqliteIsmDataService : IIsmDataService
    {
        SQLiteConnection connection;
        private bool connected;
        /// <summary>
        /// Состояние подключения к БД
        /// </summary>
        public bool IsConnected { 
            get
            {
                return connected;
            } 
        }
        public SqliteIsmDataService()
        {

        }
        /// <inheritdoc />
        public bool Connect(string dataSourceName, string userName, string password)
        {
            string connectionString = String.Format("Data Source={0};Version=3;", dataSourceName);

            connection.ConnectionString = connectionString;

            try
            {
                connection.Open();
                    
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection = null;
                }
            }
        }
        /// <inheritdoc />
        public void Disconnect()
        {
            if (connected && connection != null)
            {
                connection.Close();
                connection.Dispose();
                connection = null;
                connected = false;
            }
        }
        /// <inheritdoc />
        public bool CreateDatabase(string name)
        {
            try
            {
                SQLiteConnection.CreateFile(name);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <inheritdoc />
        public bool CreateDataModelIfNotExists()
        {
            if (IsConnected)
            {
                string createIsmTableQuery =
                    @"CREATE TABLE IF NOT EXISTS ism 
                    (
                        letter VARCHAR(3) NOT NULL
                        , gender CHAR NOT NULL
                        , name VARCHAR(256) NOT NULL UNIQUE
                        , origin VARCHAR(256)
                        , meaning VARCHAR(4096)
                    )";

                SQLiteCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = createIsmTableQuery;

                try
                {
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    if (command != null)
                        command.Dispose();
                }
            }

            return false;
        }
        /// <inheritdoc />
        public IList<IsmModel> GetAllIsm()
        {
            if (IsConnected)
            {
                string selectAllIsmQuery =
                    @"SELECT
                        letter
                        , gender
                        , name
                        , meaning
                        , origin
                    FROM ism";

                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = selectAllIsmQuery;
                command.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader reader = null;

                try
                {
                    reader = command.ExecuteReader();

                    IList<IsmModel> ismList = new List<IsmModel>();

                    while (reader.Read())
                    {
                        NameValueCollection row = reader.GetValues();

                        ismList.Add(
                            new IsmModel(row["letter"]
                                , row["gender"].Equals("F") ? GENDER.FEMALE : GENDER.MALE
                                , row["name"]
                                , row["meaning"]
                                , row["origin"])
                            );
                    }

                    return ismList;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader.Dispose();
                        reader = null;
                    }
                    if (command != null)
                        command.Dispose();
                }
            }

            return null;
        }
        /// <inheritdoc />
        public IsmModel GetIsmInfoByName(string name)
        {
            if (IsConnected && name != null && name.Length > 0)
            {
                string selectAllIsmQuery =
                    @"SELECT
                        letter
                        , gender
                        , name
                        , meaning
                        , origin
                    FROM ism
                    WHERE 1=1
                        AND name = @name
                    ";

                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = selectAllIsmQuery;
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SQLiteParameter("@name", name));

                SQLiteDataReader reader = null;

                try
                {
                    reader = command.ExecuteReader();

                    IsmModel ism = null;

                    while (reader.Read())
                    {
                        NameValueCollection row = reader.GetValues();

                        ism = new IsmModel(row["letter"]
                                , row["gender"].Equals("F") ? GENDER.FEMALE : GENDER.MALE
                                , row["name"]
                                , row["meaning"]
                                , row["origin"]);
                    }

                    return ism;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader.Dispose();
                        reader = null;
                    }
                    if (command != null)
                        command.Dispose();
                }
            }

            return null;
        }
        /// <inheritdoc />
        public IList<IsmModel> GetFilteredIsmList(string letter, string name, GENDER gender, string meaning, string origin)
        {
            if (IsConnected)
            {
                string selectAllIsmQuery =
                    @"SELECT
                        letter
                        , gender
                        , name
                        , meaning
                        , origin
                    FROM ism
                    WHERE 1=1
                        AND (@letter IS NULL OR letter LIKE @letter)
                        AND (@gender IS NULL OR gender LIKE @gender)
                        AND (@name IS NULL OR name LIKE @name)
                        AND (@meaning IS NULL OR meaning LIKE @meaning)
                        AND (@origin IS NULL OR origin LIKE @origin)
                    ";

                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = selectAllIsmQuery;
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add(new SQLiteParameter("@letter", letter));
                command.Parameters.Add(new SQLiteParameter("@gender", (gender == GENDER.MALE) ? "M" : "F"));
                command.Parameters.Add(new SQLiteParameter("@name", name));
                command.Parameters.Add(new SQLiteParameter("@meaning", meaning));
                command.Parameters.Add(new SQLiteParameter("@origin", origin));

                SQLiteDataReader reader = null;

                try
                {
                    reader = command.ExecuteReader();

                    IList<IsmModel> ismList = new List<IsmModel>();

                    while (reader.Read())
                    {
                        NameValueCollection row = reader.GetValues();

                        ismList.Add(
                            new IsmModel(row["letter"]
                                , row["gender"].Equals("F") ? GENDER.FEMALE : GENDER.MALE
                                , row["name"]
                                , row["meaning"]
                                , row["origin"])
                            );
                    }

                    return ismList;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader.Dispose();
                        reader = null;
                    }
                    if (command != null)
                        command.Dispose();
                }
            }

            return null;
        }
        /// <inheritdoc />
        public bool CreateIsm(IsmModel ism)
        {
            if (IsConnected && ism != null)
            {
                string insertIsmQuery =
                    @"INSERT INTO ism 
                    (
                        letter
                        , gender
                        , name
                        , meaning
                        , origin
                    ) 
                    VALUES 
                    (
                        @letter
                        , @gender
                        , @name
                        , @meaning
                        , @origin
                    )";

                SQLiteCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = insertIsmQuery;
                // параметры запроса
                command.Parameters.Add(new SQLiteParameter("@letter", ism.Letter));
                command.Parameters.Add(new SQLiteParameter("@gender", (ism.Gender == GENDER.MALE) ? "M" : "F"));
                command.Parameters.Add(new SQLiteParameter("@name", ism.Name));
                command.Parameters.Add(new SQLiteParameter("@meaning", ism.Meaning));
                command.Parameters.Add(new SQLiteParameter("@origin", ism.Origin));

                try
                {
                    command.ExecuteNonQuery();

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    if (command != null)
                        command.Dispose();
                }
            }

            return false;
        }
        /// <inheritdoc />
        public bool DeleteIsm(IsmModel ism)
        {
            if (IsConnected && ism != null)
                return DeleteIsm(ism.Name);

            return false;
        }
        /// <inheritdoc />
        public bool DeleteIsm(string name)
        {
            if (IsConnected && name != null && name.Length > 0)
            {
                string deleteIsmQuery =
                    @"DELETE FROM ism
                    WHERE name = @name";

                SQLiteCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = deleteIsmQuery;
                command.Parameters.Add(new SQLiteParameter("@name", name));

                try
                {
                    command.ExecuteNonQuery();

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    if (command != null)
                        command.Dispose();
                }
            }

            return false;
        }
        /// <inheritdoc />
        public bool DeleteAllIsm()
        {
            if (IsConnected)
            {
                string deleteAllIsmQuery =
                    @"DELETE FROM ism";

                SQLiteCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = deleteAllIsmQuery;
                
                try
                {
                    command.ExecuteNonQuery();

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    if (command != null)
                        command.Dispose();
                }
            }

            return false;
        }
    }
}
