using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data.SQLite;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace HomeBudgetAPI
{
    /// <summary>
    /// A collection of a group of items. Reads and writes to a Database.
    /// </summary>
    public class Categories
    {
        private SQLiteConnection _connection;

        #region Properties
        /// <summary>
        /// Gets the sql connection.
        /// </summary>
        /// <returns>
        /// Returns the sql connection.
        /// </returns>
        public SQLiteConnection Connection { private set { _connection = value; } get => _connection; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the categories object.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="isNewDB">If true, creates a new database, if false, uses an existing database.</param>
        /// <exception cref="ArgumentException">Thrown when the enum type is not an enum.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the enum type is null.</exception>
        public Categories(SQLiteConnection connection, bool isNewDB)
        {
            Connection = connection;

            if (isNewDB)
            {
                SQLiteCommand cmd = new SQLiteCommand(connection);
                var values = Enum.GetValues(typeof(Category.CategoryType));

                foreach (var value in values)
                {
                    cmd.CommandText = $"INSERT INTO categoryTypes(Description) VALUES('{value.ToString()}')";
                    cmd.ExecuteNonQuery();
                }

                SetCategoriesToDefaults();
                cmd.Dispose();
            }

        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the category based on the category ID.
        /// </summary>
        /// <example>
        /// <code>
        /// Categories categories = new Categories(myDBConnection, isNewDatabase);
        /// int categoryID = 5;
        /// Category category = categories.GetCategoryFromId(categoryID);
        /// </code>
        /// </example>
        /// <param name="i">The category ID.</param>
        /// <returns>The category, if found.</returns>
        /// <exception cref="Exception">Thrown when the category doesn't exist in the category database, therefore category is null.</exception>
        public Category GetCategoryFromId(int i)
        {
            Category? c = null;

            SQLiteCommand cmd = new SQLiteCommand("Select Id, Description, TypeId from categories WHERE Id = @id", Connection);

            cmd.Parameters.AddWithValue("@id", i);

            SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Category category = new Category(rdr.GetInt32(0), rdr.GetString(1), (Category.CategoryType)rdr.GetInt32(2));
                c = category;
            }

            if (c == null)
            {
                throw new Exception("Cannot find category with id " + i.ToString());
            }
            
            return c;
        }

        /// <summary>
        /// Sets and populates the table of categories to default categories
        /// </summary>
        /// <example>
        /// <code>
        /// Categories categories = new Categories(myDBConnection, isNewDatabase);
        /// categories.Add("Subscriptions", Category.CategoryType.Expense);
        /// categories.SetCategoriesToDefaults();
        /// </code>
        /// </example>
        /// <remarks>Please note that the categories are set to DEFAULT! Meaning existing ones are overriden!</remarks>
        public void SetCategoriesToDefaults()
        {
            SQLiteCommand cmd = new SQLiteCommand(Connection);

            cmd.CommandText = "SELECT Id FROM categories";

            SQLiteDataReader rdr = cmd.ExecuteReader();
            
            while (rdr.Read())
            {
                Delete(rdr.GetInt32(0));
            }

            cmd.Dispose();

            //adds all the default categories
            Add("Utilities", Category.CategoryType.Expense);
            Add("Rent", Category.CategoryType.Expense);
            Add("Food", Category.CategoryType.Expense);
            Add("Entertainment", Category.CategoryType.Expense);
            Add("Education", Category.CategoryType.Expense);
            Add("Miscellaneous", Category.CategoryType.Expense);
            Add("Medical Expenses", Category.CategoryType.Expense);
            Add("Vacation", Category.CategoryType.Expense);
            Add("Credit Card", Category.CategoryType.Credit);
            Add("Clothes", Category.CategoryType.Expense);
            Add("Gifts", Category.CategoryType.Expense);
            Add("Insurance", Category.CategoryType.Expense);
            Add("Transportation", Category.CategoryType.Expense);
            Add("Eating Out", Category.CategoryType.Expense);
            Add("Savings", Category.CategoryType.Savings);
            Add("Income", Category.CategoryType.Income);
        }

        /// <summary>
        /// Adds a category to the database.
        /// </summary>
        /// <example>
        /// <code>
        /// Categories categories = new Categories(myDBConnection, isNewDatabase);
        /// categories.Add(category);
        /// </code>
        /// </example>
        /// <param name="cat">The provided category to add to the database.</param>
        private void Add(Category cat)
        {
            //start the command tool
            SQLiteCommand cmd = new SQLiteCommand(Connection);  

            cmd.CommandText = "SELECT Id FROM categoryTypes WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", (int)cat.Type);

            SQLiteDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                rdr.Close();
                //execute the commands
                cmd.CommandText = $"INSERT INTO categories(Description, TypeId) VALUES(@Description, @TypeId)";
                cmd.Parameters.AddWithValue("@Description", cat.Description);
                cmd.Parameters.AddWithValue("@TypeId", cat.Id);
                cmd.ExecuteNonQuery();

            }

            //dispose of the command tool
            cmd.Dispose();
        }

        /// <summary>
        /// Adds a category to the table of categories by creating a category object.
        /// </summary>
        /// <example>
        /// <code>
        /// Categories categories = new Categories(myDBConnection, isNewDatabase);
        /// categories.Add("Subscriptions", Category.CategoryType.Expense);
        /// </code>
        /// </example>
        /// <param name="desc">A description of the category.</param>
        /// <param name="type">The type of category.</param>
        public void Add(String desc, Category.CategoryType type)
        {

            SQLiteCommand cmd = new SQLiteCommand(Connection);
            cmd.CommandText = "SELECT Id FROM categoryTypes WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", (int)type);

            SQLiteDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                rdr.Close();
                cmd.CommandText = "INSERT INTO categories(Description, TypeId) VALUES(@description, @typeId)";
                cmd.Parameters.AddWithValue("@description", desc);

                cmd.Parameters.AddWithValue("@typeId", (int)type);
                cmd.ExecuteNonQuery();
            }

            cmd.Dispose();
        }

        /// <summary>
        /// Deletes the category from the table of categories using a specified category ID.
        /// </summary>
        /// <example>
        /// <code>
        /// Categories categories = new Categories(myDBConnection, isNewDatabase);
        /// categories.Add("Subscriptions", Category.CategoryType.Expense);
        /// categories.Delete(17);
        /// </code>
        /// </example>
        /// <param name="Id">The category ID to search for the category to delete from the table.</param>
        /// <exception cref="Exception">Thrown when the category to delete doesn't exist in the category database, therefore category is null.</exception>
        public void Delete(int Id)
        {
            Category? category = null;
            try
            {
                 category = GetCategoryFromId(Id);
            }
            catch (Exception ex)
            {
                Console.Write("Error deleting Category:" + ex.Message);
                return;
            }

            SQLiteCommand cmd = new SQLiteCommand(Connection);

            cmd.CommandText = "SELECT COUNT(Id) FROM expenses WHERE CategoryId = @id";
            cmd.Parameters.AddWithValue("@id", Id);

            SQLiteDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                if (rdr.GetInt32(0) == 0)
                {
                    rdr.Close();
                    cmd.CommandText = "DELETE FROM categories WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", category.Id);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    rdr.Close();
                }
            }

            cmd.Dispose();
        }

        /// <summary>
        /// Copies the list of categories into a new list to be unchanged by the user.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// Categories categories = new Categories(myDBConnection, isNewDatabase);
        /// categories.Add(category);
        /// List<Category> newList = categories.List();
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>The new copy of the categories list.</returns>
        public List<Category> List()
        {
            List<Category> newList = new List<Category>();

            SQLiteCommand cmd = new SQLiteCommand("Select Id, Description, TypeId from categories", Connection);

            SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Category category = new Category(rdr.GetInt32(0), rdr.GetString(1), (Category.CategoryType)rdr.GetInt32(2));
                newList.Add(category);
            }

            return newList;
        }

        /// <summary>
        /// Updates the category in the table of categories.
        /// </summary>
        /// <example>
        /// <code>
        /// Categories categories = new Categories(myDBConnection, isNewDatabase);
        /// categories.Add(category);
        /// categories.UpdateProperties(category.Id, "New Description", category.Type);
        /// </code>
        /// </example>
        /// <param name="id">The id of the category.</param>
        /// <param name="newDescr">The new description of the category.</param>
        /// <param name="income">The new category type of the category.</param>
        /// <exception cref="Exception">Thrown when the category to update doesn't exist in the category database, therefore category is null.</exception>
        public void UpdateProperties(int id, string newDescr, Category.CategoryType income)
        {
            //checks whether the category to update is a valid category
            Category? category = null;

            try
            {
                category = GetCategoryFromId(id);
            }
            catch (Exception ex)
            {
                Console.Write("Error updating Category:" + ex.Message);
                return;
            }

            //opens new command connection
            SQLiteCommand cmd = new SQLiteCommand(Connection);
            cmd.CommandText = "SELECT Id FROM categoryTypes WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", (int)income);

            SQLiteDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                rdr.Close();
                //execute commands
                cmd.CommandText = "UPDATE categories SET Description = @description, TypeId = @typeid WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@description", newDescr);
                cmd.Parameters.AddWithValue("@typeid", income);
                cmd.ExecuteNonQuery();

            }

            //disposes of command connection
            cmd.Dispose();

        }
        #endregion
    }
}

