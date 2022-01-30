using System.Data.SQLite;

namespace ForesterAPI
{
    public static class SQLDatabase
    {
        static string path;

        static SQLDatabase()
        {
            path = "Data Source=./Database/database.db;Version=3;";
        }

        public static T[] Select<T>(string query) where T : struct
        {
            SQLiteConnection connection = new SQLiteConnection(path);
            connection.Open();
            var cmd = new SQLiteCommand(query, connection);
            SQLiteDataReader rdr = cmd.ExecuteReader();

            var fields = typeof(T).GetProperties();

            List<T> final = new List<T>();
            while (rdr.Read())
            {
                final.Add(new T());

                for (int i = 0; i < fields.Length; i++)
                {
                    object setvalueobject = final[^1];
                    fields[i].SetValue(setvalueobject, Convert.ChangeType(rdr.GetValue(i), fields[i].PropertyType));
                    final[^1] = (T)setvalueobject;
                }
            }
            return final.ToArray();
        }

        public static void NoReturnQuery(string query)
        {
            using var con = new SQLiteConnection(path);
            con.Open();
            using var cmd = new SQLiteCommand(con);
            cmd.CommandText = query;
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

    }
}
