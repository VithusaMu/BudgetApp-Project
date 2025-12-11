using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data.SQLite;
using System.Data.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace HomeBudgetAPI
{
    // ====================================================================
    // CLASS: expenses
    //        - A collection of expense items,
    //        - Read / write to file
    //        - etc
    // ====================================================================
    /// <summary>
    /// A collection of required costs. Reads and writes to file.
    /// </summary>
    public class Expenses
    {
        private SQLiteConnection _connection;

        // ====================================================================
        // Properties
        // ====================================================================

        /// <summary>
        /// Gets the sql connection.
        /// </summary>
        /// <returns>
        /// Returns the sql connection.
        /// </returns>
        public SQLiteConnection Connection { private set { _connection = value; } get => _connection; }

        /// <summary>
        /// Creates the expenses object.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="isNewDB">If true, creates a new database from the categories class, if false, uses the existing database.</param>
        public Expenses(SQLiteConnection connection, bool isNewDB)
        {
            Connection = connection;

            if (isNewDB)
            {
                Categories categories = new Categories(connection, isNewDB);
            }
        }


        // ====================================================================
        // Add expense
        // ====================================================================
        private void Add(Expense exp)
        {
            //start the command tool
            SQLiteCommand cmd = new SQLiteCommand(Connection);

            cmd.CommandText = "SELECT Id FROM categories WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", exp.Id);

            SQLiteDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                rdr.Close();
                cmd.CommandText = $"INSERT INTO expenses(Date, Description, Amount, CategoryId) VALUES(@Date, @Description, @Amount, @CategoryId)";
                cmd.Parameters.AddWithValue("@Date", exp.Date);
                cmd.Parameters.AddWithValue("@Description", exp.Description);
                cmd.Parameters.AddWithValue("@Amount", exp.Description);
                cmd.Parameters.AddWithValue("@CategoryId", exp.Category);

                //execute the command
                cmd.ExecuteNonQuery();
            }

            //dispose of the command tool
            cmd.Dispose();
        }

        /// <summary>
        /// Adds an expense to the table of expenses by creating an expense object.
        /// </summary>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses(myDBConnection, isNewDatabase);
        /// expenses.Add(DateTime.Now, (int)Category.CategoryType.Expense, 85.00, "Doctor's appointment");
        /// </code>
        /// </example>
        /// <param name="date">The date of the expense.</param>
        /// <param name="category">The category of the expense.</param>
        /// <param name="amount">The amount for the expense.</param>
        /// <param name="description">The description of the expense.</param>
        public void Add(DateTime date, int category, Double amount, string description)
        {
            SQLiteCommand cmd = new SQLiteCommand(Connection);
            cmd.CommandText = "SELECT Id FROM categories WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", category);

            SQLiteDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                rdr.Close();
                //execute the commands
                cmd.CommandText = $"INSERT INTO expenses(Date, Description, Amount, CategoryId) VALUES(@Date, @Description, @Amount, @CategoryId)";

                cmd.Parameters.AddWithValue("@Date", date);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@CategoryId", category);
                cmd.ExecuteNonQuery();

            }
            cmd.Dispose();
        }

        // ====================================================================
        // Delete expense
        // ====================================================================
        /// <summary>
        /// Deletes the expense from the table of expenses using a specified expense ID.
        /// </summary>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses(myDBConnection, isNewDatabase);
        /// expenses.Add(DateTime.Now, (int)Category.CategoryType.Expense, 85.00, "Doctor's appointment");
        /// expenses.Delete(1);
        /// </code>
        /// </example>
        /// <param name="Id">The expense ID to search for the expense to delete from the table.</param>
        /// <exception cref="Exception">Thrown when the expense to delete doesn't exist in the expenses database, therefore expense is null.</exception>
        public void Delete(int Id)
        {
            Expense? expense = null;
            try
            {
                expense = GetExpenseFromId(Id);
            }
            catch (Exception ex)
            {
                Console.Write("Error deleting Expense:" + ex.Message);
                return;
            }

            SQLiteCommand cmd = new SQLiteCommand(Connection);
            cmd.CommandText = "DELETE FROM expenses WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", expense.Id);
            cmd.ExecuteNonQuery();


            cmd.Dispose();


        }

        // ====================================================================
        // Return list of expenses
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================
        /// <summary>
        /// Copies the list of expenses into a new list to be unchanged by the user.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// Expenses expenses = new Expenses(myDBConnection, isNewDatabase);
        /// expenses.Add(DateTime.Now, (int)Category.CategoryType.Expense, 85.00, "Doctor's appointment");
        /// List<Expense> newList = expenses.List();
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>The new copy of the expenses list.</returns>
        public List<Expense> List()
        {
            List<Expense> newList = new List<Expense>();

            SQLiteCommand cmd = new SQLiteCommand("Select Id, Date, CategoryId, Amount, Description from expenses", Connection);

            SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Expense expense = new Expense(rdr.GetInt32(0), rdr.GetDateTime(1), rdr.GetInt32(2), rdr.GetDouble(3), rdr.GetString(4));
                newList.Add(expense);
            }

            return newList;
        }

        /// <summary>
        /// Gets the expense based on the expense ID.
        /// </summary>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses(myDBConnection, isNewDatabase);
        /// int expenseID = 5;
        /// Expense expense = expenses.GetExpenseFromId(expenseID);
        /// </code>
        /// </example>
        /// <param name="i">The expense ID.</param>
        /// <returns>The expense, if found.</returns>
        /// <exception cref="Exception">Thrown when the expense doesn't exist in the expenses database, therefore expense is null.</exception>
        public Expense GetExpenseFromId(int i)
        {
            Expense? e = null;
           
            SQLiteCommand cmd = new SQLiteCommand("Select Id, Date, CategoryId, Amount, Description from expenses WHERE Id = @id", Connection);

            cmd.Parameters.AddWithValue("@id", i);

            SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Expense expense = new Expense(rdr.GetInt32(0), rdr.GetDateTime(1), rdr.GetInt32(2), rdr.GetDouble(3), rdr.GetString(4));
                e = expense;
            }

            if (e == null)
            {
                throw new Exception("Cannot find expense with id " + i.ToString());
            }

            return e;
        }

        /// <summary>
        /// Updates the expense in the table of expenses.
        /// </summary>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses(myDBConnection, isNewDatabase);
        /// expenses.Add(expense);
        /// expenses.UpdateProperties(expense.Id, "New Description", expense.Type);
        /// </code>
        /// </example>
        /// <param name="id">The id of the expense.</param>
        /// <param name="newDescr">The new description of the expense.</param>
        /// <param name="newDate"></param>
        /// <param name="newAmount"></param>
        /// <param name="categoryId"></param>
        /// <exception cref="Exception">Thrown when the expense to update doesn't exist in the expenses database, therefore expense is null.</exception>
        public void UpdateProperties(int id, DateTime newDate, int categoryId, double newAmount, string newDescr)
        {
            Expense? expense = null;
            try
            {
                //checks whether the expense to update is a valid expense
                expense = GetExpenseFromId(id);
            }
            catch (Exception ex)
            {
                Console.Write("Error updating Expense:" + ex.Message);
                return;
            }

            //opens new command connection
            SQLiteCommand cmd = new SQLiteCommand(Connection);
            cmd.CommandText = "SELECT Id FROM categories WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", categoryId);

            SQLiteDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                rdr.Close();
                //execute commands
                cmd.CommandText = "UPDATE expenses SET Date = @Date, Description = @Description, Amount = @Amount, CategoryId = @CategoryId WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@Date", newDate);
                cmd.Parameters.AddWithValue("@Description", newDescr);
                cmd.Parameters.AddWithValue("@Amount", newAmount);
                cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                cmd.ExecuteNonQuery();

            }
            //disposes of command connection
            cmd.Dispose();

        }

    }
}

