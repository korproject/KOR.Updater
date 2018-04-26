using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace KOR_Updater_UI_WPF_Example.Controllers
{
    /// <summary>
    /// Database controller class
    /// </summary>
    public class DatabaseController
    {
        public static string DatabaseRootPath = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Database file name (with extension)
        /// </summary>
        public static string DatabaseName = "KOR.Updater.db";

        /// <summary>
        /// Database file path
        /// </summary>
        public static string DatabasePath { get; set;}

        /// <summary>
        /// Database connection string
        /// </summary>
        public static string ConnectionString { get; set; }

        public static bool DatabaseCheck()
        {
            // set current database path
            DatabasePath = DatabaseRootPath + DatabaseName;

            // set sqlite conenction string
            ConnectionString = @"Data Source=" + DatabasePath + "; Version=3;";

            // if the database file does not exist
            if (!File.Exists(DatabasePath))
            {
                // create new database file
                SQLiteConnection.CreateFile(DatabaseName);

                // and then check again file exists
                if (File.Exists(DatabasePath))
                {
                    using (var conn = new SQLiteConnection(ConnectionString))
                    {
                        conn.Open();

                        // create new updater table
                        using (var commUpdater = new SQLiteCommand(conn))
                        {
                            commUpdater.CommandText = "CREATE TABLE `updater` ( `name` TEXT, `version` TEXT DEFAULT 0, `bit` TEXT, `deploy` TEXT, `lang` TEXT, `last_update` TEXT, `feedback` INTEGER DEFAULT 0 )";
                            commUpdater.ExecuteNonQuery();
                        }

                        // insert default values
                        using (var commDefault = new SQLiteCommand(conn))
                        {
                            commDefault.CommandText = "INSERT INTO updater (version, feedback) values(?,?)";
                            commDefault.Parameters.Add("version", DbType.String).Value = "0.0.0.1";
                            commDefault.Parameters.Add("feedback", DbType.String).Value = "1";
                            commDefault.ExecuteNonQuery();
                        }

                        // create new updates table
                        using (var commupdates = new SQLiteCommand(conn))
                        {
                            commupdates.CommandText = "CREATE TABLE `updates` ( `updateid` TEXT, `date` TEXT, `version` TEXT, `added_features` TEXT NOT NULL, `removed_features` TEXT NOT NULL, `reason_code` TEXT, `reason_title` TEXT )";
                            commupdates.ExecuteNonQuery();
                        }

                        conn.Close();
                    }
                }

                return false;
            }

            // already setup database
            return true;
        }
    }
}
