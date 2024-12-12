using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Threading;

// ===================================================================
// Very important notes:
// ... To keep everything working smoothly, you should always
//     dispose of EVERY SQLiteCommand even if you recycle a 
//     SQLiteCommand variable later on.
//     EXAMPLE:
//            Database.newDatabase(GetSolutionDir() + "\\" + filename);
//            var cmd = new SQLiteCommand(Database.dbConnection);
//            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('Whatever')";
//            cmd.ExecuteNonQuery();
//            cmd.Dispose();
//
// ... also dispose of reader objects
//
// ... by default, SQLite does not impose Foreign Key Restraints
//     so to add these constraints, connect to SQLite something like this:
//            string cs = $"Data Source=abc.sqlite; Foreign Keys=1";
//            var con = new SQLiteConnection(cs);
//
// ===================================================================


namespace Calendar
{
    public class Database
    {
        public static SQLiteConnection dbConnection { get { return _connection; } }
        private static SQLiteConnection _connection;

        // ===================================================================
        // create and open a new database
        // ===================================================================
        public static void NewDatabase(string filename)
        {

            // If there was a database open before, close it and release the lock
            CloseDatabaseAndReleaseFile();

            // Check if filename is specified
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("ERROR: No file name was specified");
            }

            // Check if the file already exists
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            string connectionString = $"URI=file:{filename}; Foreign Keys=1";
            _connection = new SQLiteConnection(connectionString);
            _connection.Open();
            var cmd = new SQLiteCommand(_connection);
            CreateTables(cmd);
            cmd.Dispose();

        }

       // ===================================================================
       // open an existing database
       // ===================================================================
       public static void existingDatabase(string filename)
        {

            CloseDatabaseAndReleaseFile();

            // Check if filename is specified
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("ERROR: No file name was specified");
            }

            // Check if the file already exists and delete it
            if (!File.Exists(filename))
            {
                throw new InvalidOperationException("A database file with the same name already exists.");

            }

            // Build connection string and open the connection
            string connectionString = $"URI=file:{filename}; Foreign Keys=1";
            _connection = new SQLiteConnection(connectionString);
            _connection.Open();
        }

       // ===================================================================
       // close existing database, wait for garbage collector to
       // release the lock before continuing
       // ===================================================================
        static public void CloseDatabaseAndReleaseFile()
        {
            if (Database.dbConnection != null)
            {
                // close the database connection
                Database.dbConnection.Close();
                

                // wait for the garbage collector to remove the
                // lock from the database file
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        public static void CreateTables(SQLiteCommand cmd)
        {
            // Create Events table
            cmd.CommandText = @"
                DROP TABLE IF EXISTS events;
                CREATE TABLE IF NOT EXISTS events (
                   Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                   StartDateTime TEXT,
                   DurationInMinutes REAL,
                   Details TEXT,
                   CategoryId INTEGER NOT NULL,
                   FOREIGN KEY(CategoryId) REFERENCES categories(Id) 
                )";
            cmd.ExecuteNonQuery();

            // Create categories table
            cmd.CommandText = @"
                DROP TABLE IF EXISTS categories;
                CREATE TABLE IF NOT EXISTS categories (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    Description TEXT,
                    TypeId INTEGER NOT NULL,
                    FOREIGN KEY(TypeId) REFERENCES CategoryTypes(Id)
                 )";
            cmd.ExecuteNonQuery();

            // Create categoryTypes table
            cmd.CommandText = @"
                DROP TABLE IF EXISTS categoryTypes;
                CREATE TABLE IF NOT EXISTS categoryTypes (
                   Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                   Description TEXT
                )";
            cmd.ExecuteNonQuery();
        }
    }

}
