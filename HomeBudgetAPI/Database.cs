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


namespace HomeBudgetAPI
{
    /// <summary>
    /// A database that keeps all information of categories, expenses, and category types.
    /// </summary>
    public class Database
    {
        private static SQLiteConnection _connection;

        /// <summary>
        /// Property for Database Connection.
        /// </summary>
        /// <returns>
        /// Gets the database connection.
        /// </returns>
        public static SQLiteConnection dbConnection { get { return _connection; } }

        // ===================================================================
        // create and open a new database
        // ===================================================================
        /// <summary>
        /// Creates a new database. Existing data is overriden.
        /// </summary>
        /// <param name="filename">The database file.</param>
        /// <example>
        ///     <code>
        ///         Database.newDatabase(GetSolutionDir() + "\\" + filename);
        ///         var cmd = new SQLiteCommand(Database.dbConnection);
        ///         cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('Whatever')";
        ///         cmd.ExecuteNonQuery();
        ///         cmd.Dispose();    
        ///     </code>
        /// </example>
        public static void newDatabase(string filename)
        {

            // If there was a database open before, close it and release the lock
            CloseDatabaseAndReleaseFile();

            string cs = $"Data Source={filename}; Foreign Keys=1";

            _connection = new SQLiteConnection(cs);
            dbConnection.Open();
           

            using var cmd = new SQLiteCommand(dbConnection);

            //==============
            //Table deletion
            //==============
            cmd.CommandText = "DROP TABLE IF EXISTS expenses";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE IF EXISTS categories";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE IF EXISTS categoryTypes";
            cmd.ExecuteNonQuery();

            //==============
            //table creation
            //==============
            //creates categoryTypes
            cmd.CommandText = @"CREATE TABLE categoryTypes(
                Id INTEGER PRIMARY KEY,
                Description TEXT)";
            cmd.ExecuteNonQuery();

            //creates categories
            cmd.CommandText = @"CREATE TABLE categories(
                Id INTEGER PRIMARY KEY,
                Description TEXT, 
                TypeId INTEGER,
                FOREIGN KEY(TypeId) REFERENCES categoryTypes(Id))";
            cmd.ExecuteNonQuery();

            //creates expenses
            cmd.CommandText = @"CREATE TABLE expenses(
                Id INTEGER PRIMARY KEY,
                Date TEXT, 
                Description TEXT, 
                Amount REAL, 
                CategoryId INTEGER,
                FOREIGN KEY(CategoryId) REFERENCES categories(Id))";
            cmd.ExecuteNonQuery();

            //closes the cmd
            cmd.Dispose();
        }

        // ===================================================================
        // open an existing database
        // ===================================================================
        /// <summary>
        /// Loads in an already existing Database from the provided file.
        /// </summary>
        /// <param name="filename">The database file of an existing database.</param>
        ///<example>
        ///     <code>
        ///         Database.existingDatabase(GetSolutionDir() + "\\" + filename);
        ///         var cmd = new SQLiteCommand(Database.dbConnection);
        ///         cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('Whatever')";
        ///         cmd.ExecuteNonQuery();
        ///         cmd.Dispose();    
        ///     </code>
        /// </example>
        public static void existingDatabase(string filename)
        {

            CloseDatabaseAndReleaseFile();

            string cs = $"Data Source={filename}; Foreign Keys=1";

            _connection = new SQLiteConnection(cs);
            dbConnection.Open();


        }

        // ===================================================================
        // close existing database, wait for garbage collector to
        // release the lock before continuing
        // ===================================================================
        /// <summary>
        /// Closes an database and releases the file before continuing.
        /// </summary>
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
    }

}
