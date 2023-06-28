using System.Data.Common;
using System.Data.SQLite;
using System.Reflection;

namespace ForesterAPI.Data.Connection
{
    public class SQLManager //ver. 2.0
    {
        string path;

        SQLiteConnection connection;

        public SQLManager() 
        { 
            path = "Data Source=./ForesterDatabase/Database.db;Version=3;";
            OpenConnection();
        }

        void OpenConnection()
        {
            connection = new SQLiteConnection(path);
            connection.Open();  
        }

        void CloseConnection() => connection.Close();

        public  T[] SelectMany<T>(string query) where T : struct
        {
            var command = new SQLiteCommand(query, connection);
            SQLiteDataReader reader = command.ExecuteReader();

            List<T> final = new List<T>();

            var columns = reader.GetColumnSchema();
            var fields = typeof(T).GetProperties();

            while (reader.Read())
            {
                T item = new T();

                for(int i = 0; i < reader.FieldCount; i++)
                {
                    DbColumn currentColumn = columns[i];

                    if (!fields.Any(x => x.Name == currentColumn.ColumnName))
                        continue;

                    PropertyInfo field = fields.Single(x=>x.Name == currentColumn.ColumnName);
                    field.SetValue(item, Convert.ChangeType(reader.GetValue(i), field.PropertyType));
                }

                final.Add(item);
            }

            return final.ToArray();
        }

        public T SelectSingle<T>(string query) where T : struct
        {
            var objects = SelectMany<T>(query);

            if (objects.Length > 0)
                throw new Exception("Detected more than one value");

            return objects[0];
        }

        public void ExecuteNonQuery(string command)
        {
            var command = new SQLiteCommand(command, connection);
            command.ExecuteNonQuery();
        }
    }
}
