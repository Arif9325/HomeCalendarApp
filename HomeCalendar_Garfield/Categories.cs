using System.Data.Common;
using System.Data.SQLite;

using System.Security.Cryptography;
using System.Data.Entity.ModelConfiguration.Configuration;

using System.Xml;


// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: categories
    //        - A collection of category items,
    //        - Read / write to the database
    //        - etc
    // ====================================================================
    /// <summary>
    /// Represents a collection of category items for a calendar program stored in/ read from a database.
    /// </summary>
    public class Categories
    {
        //private static String DefaultFileName = "calendarCategories.txt";
        //private string? _FileName;
        //private string? _DirName;
        private SQLiteConnection _con;

        // ====================================================================
        // Properties
        // ====================================================================


        // ====================================================================
        // Constructor
        // ====================================================================

        /// <summary>
        /// Sets categories to default in the database when a categories object is made only if connection to a newly created database is passed.
        /// </summary>
        /// <param name="con">The SQLite connection to the database (new or existing)</param>
        /// <param name="isNewDB">Boolean variable specifying if a new database is created</param>
        /// /// <example>
        /// <code>
        ///     <![CDATA[
        ///     Database.existingDatabase("calendar.db");
        ///     SQLiteConnection con = Database.dbConnection;
        ///     conn.Open();
        ///     Categories c = new Categories(con, false);
        /// ]]>
        /// </code>
        /// </example>
        public Categories(SQLiteConnection con, bool isNewDB)
        {
            _con = con;
            if(isNewDB)
                SetCategoriesToDefaults();
        }

        // ====================================================================
        // get a specific category from the categories table in the database where the id is the one specified
        // ====================================================================
        /// <summary>
        /// Get a specific category from the categories table in the database where the id is the one specified.
        /// </summary>
        /// <param name="i">Category Id to look for.</param>
        /// <returns>Category with matching Id</returns>
        /// <exception cref="Exception">Throws if no category with specified Id exists.</exception>
        /// <example>
        /// <code>
        ///     <![CDATA[
        ///     Database.existingDatabase("calendar.db");
        ///     SQLiteConnection con = Database.dbConnection;
        ///     conn.Open();
        ///     Categories c = new Categories(con, false);
        ///     c.GetCategoryFromId(0);
        /// ]]>
        /// </code>
        /// </example>
        public Category GetCategoryFromId(int id)
        {
            Category cat = null;
            try
            {
                using var cmd = new SQLiteCommand(_con);

                cmd.CommandText = $"SELECT * FROM Categories WHERE Id = @Id";

                cmd.Parameters.Add(new SQLiteParameter("@Id", id));
                using SQLiteDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                    cat = new Category(rdr.GetInt32(0), rdr.GetString(1), (Category.CategoryType)rdr.GetInt32(2));

                else
                    throw new Exception("No category with specified Id exists");
            }
            catch (Exception e)
            {
                throw new Exception("Error retrieving category: " + e.Message);
            }


            return cat;
        }


        // ====================================================================
        // set categories to default
        // ====================================================================
        /// <summary>
        /// Sets categories to default values in the database
        /// </summary>
        ///<example>
        /// <code>
        ///     <![CDATA[
        ///     Database.existingDatabase("calendar.db");
        ///     SQLiteConnection con = Database.dbConnection;
        ///     conn.Open();
        ///     Categories c = new Categories(con, false);
        ///     c.SetCategoriesToDefaults();
        /// ]]>
        /// </code>
        /// </example>
        public void SetCategoriesToDefaults()
        {
            try
            {
                // ---------------------------------------------------------------
                // reset any current categories,
                // ---------------------------------------------------------------
                var cmd = new SQLiteCommand(_con);
                //cmd.CommandText = "DELETE FROM events";
                //cmd.ExecuteNonQuery();

                //cmd.CommandText = "DELETE FROM Categories";
                //cmd.ExecuteNonQuery();

                //cmd.CommandText = "DELETE FROM CategoryTypes";
                //cmd.ExecuteNonQuery();
                Database.CreateTables(cmd);

                // ---------------------------------------------------------------
                // Add Defaults
                // ---------------------------------------------------------------
                cmd.CommandText = "INSERT INTO categoryTypes(Description)Values('Event')";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO categoryTypes(Description)Values('Availability')";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO categoryTypes(Description)Values('Holiday')";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO categoryTypes(Description)Values('AllDayEvent')";
                cmd.ExecuteNonQuery();



                cmd.CommandText = "INSERT INTO Categories(Description,TypeId)Values('School',1)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Categories(Description,TypeId)Values('Work',1)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Categories(Description,TypeId)Values('Fun',1)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Categories(Description,TypeId)Values('Medical',1)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Categories(Description,TypeId)Values('Sleep',1)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Categories(Description,TypeId)Values('Working',2)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Categories(Description,TypeId)Values('On call',2)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Categories(Description,TypeId)Values('Canadian Holidays',3)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Categories(Description,TypeId)Values('Vacation',4)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Categories(Description,TypeId)Values('Wellness Days',4)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Categories(Description,TypeId)Values('BirthDays',4)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Categories(Description,TypeId)Values('Non Standard',1)";
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("Uknown error, " + e.Message);
            }

        }


        // ====================================================================
        // Add category
        // ====================================================================
        /// <summary>
        /// Add category to Categories table in the database.
        /// </summary>
        /// <param name="desc">Description of category.</param>
        /// <param name="type">Type of category.</param>
        /// <example>
        /// <code>
        ///     <![CDATA[
        ///     Database.existingDatabase("calendar.db");
        ///     SQLiteConnection con = Database.dbConnection;
        ///     conn.Open();
        ///     Categories c = new Categories(con, false);
        ///     c.Add("Sleep", (Category.CategoryType)5);
        /// ]]>
        /// </code>
        /// </example>
        public void Add(String desc, Category.CategoryType type)
        {
            try
            {
                using var cmd = new SQLiteCommand(_con);
                cmd.CommandText = "INSERT INTO Categories (description, TypeId) VALUES(@description, @TypeId)";

                cmd.Parameters.AddWithValue("@description", desc);
                cmd.Parameters.AddWithValue("@TypeId", (int)type);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("Uknown error, " + e.Message);
            }

        }

        // ====================================================================
        // Delete category
        // ====================================================================
        /// <summary>
        /// Deletes category that matches passed in id from categories table in the database.
        /// </summary>
        /// <param name="Id">Id of category to be deleted</param>
        /// <example>
        /// <code>
        ///     <![CDATA[
        ///     Database.existingDatabase("calendar.db");
        ///     SQLiteConnection con = Database.dbConnection;
        ///     conn.Open();
        ///     Categories c = new Categories(con, false);
        ///     c.Delete(4);
        /// ]]>
        /// </code>
        /// </example>
        public void Delete(int Id)
        {
            try
            {

                using var cmd = new SQLiteCommand(_con);
                cmd.CommandText = "DELETE FROM events where CategoryId = @id";
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM Categories where Id = @id";

                cmd.Parameters.AddWithValue("@id", Id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception("Uknown error, " + e.Message);
            }
            
        }


        // ====================================================================
        // Retrieving all the categories from  categories table
        // ====================================================================

        /// <summary>
        /// Returns copy of categories list.
        /// </summary>
        /// <returns>Copy of categories list.</returns>
        /// <example>
        /// <code>
        ///     <![CDATA[
        ///     Database.existingDatabase("calendar.db");
        ///     SQLiteConnection con = Database.dbConnection;
        ///     conn.Open();
        ///     Categories c = new Categories(con, false);
        ///     c.List();
        /// ]]>
        /// </code>
        /// </example>
        public List<Category> List()
        {
            List<Category> newList = new List<Category>();
            string stm = "SELECT * FROM Categories";

                using var cmd = new SQLiteCommand(stm, _con);
                using SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Category cat = new Category(rdr.GetInt32(0), rdr.GetString(1), (Category.CategoryType)rdr.GetInt32(2));
                    newList.Add(cat);
                }
            return newList;
        }

        // ====================================================================
        // Update category
        // ====================================================================
        /// <summary>
        /// Updates category that matches passed in id from categories table in the database.
        /// </summary>
        /// <param name="Id">Id of category to be updated</param>
        /// <example>
        /// <code>
        ///     <![CDATA[
        ///     Database.existingDatabase("calendar.db");
        ///     SQLiteConnection con = Database.dbConnection;
        ///     conn.Open();
        ///     Categories c = new Categories(con, false);
        ///     c.UpdateProperties(2, "Medical", (Category.CategoryType)4);
        /// ]]>
        /// </code>
        /// </example>
        public void UpdateProperties(int id, string newDescr, Category.CategoryType @event)
        {
            try
            {
                using var cmd = new SQLiteCommand(_con);
                cmd.CommandText = "UPDATE Categories SET Description = @description, TypeId = @TypeId WHERE Id = @id" ;

                cmd.Parameters.AddWithValue("@description", newDescr);
                cmd.Parameters.AddWithValue("@TypeId", (int)@event);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("Error updating category properties: " + e.Message);
            }

        }

    }
}

