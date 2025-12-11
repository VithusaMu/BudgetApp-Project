using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using HomeBudgetAPI;
using System.Data.SQLite;

namespace BudgetCodeTests
{
    [Collection("Sequential")]
    public class TestExpenses
    {
        int numberOfExpensesInFile = TestConstants.numberOfExpensesInFile;
        String testInputFile = TestConstants.testExpensesInputFile;
        int maxIDInExpenseFile = TestConstants.maxIDInExpenseFile;
        Expense firstExpenseInFile = new Expense(1, new DateTime(2021, 1, 10), 10, 12, "hat (on credit)");


        // ========================================================================

        [Fact]
        public void ExpensesObject_New()
        {

            // Arrange
            String folder = TestConstants.GetSolutionDir();
            string newDB = $"{folder}\\newDB.db";
            Database.newDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;

            // Act
            Expenses expenses = new Expenses(conn, true);

            // Assert
            Assert.IsType<Expenses>(expenses);
        }

        // ========================================================================
        
        [Fact]
        public void ExpensesMethod_ReadFromDatabase_ValidateCorrectDataWasRead()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String existingDB = $"{folder}\\{TestConstants.testDBInputFile}";   
            Database.existingDatabase(existingDB);
            SQLiteConnection conn = Database.dbConnection;


            // Act
            Expenses expenses = new Expenses(conn, false);
            List<Expense> list = expenses.List();
            Expense firstExpense = list[0];

            // Assert
            Assert.Equal(numberOfExpensesInFile, list.Count);
            Assert.Equal(firstExpenseInFile.Id, firstExpense.Id);
            Assert.Equal(firstExpenseInFile.Amount, firstExpense.Amount);
            Assert.Equal(firstExpenseInFile.Description, firstExpense.Description);
            Assert.Equal(firstExpenseInFile.Category, firstExpense.Category);
        }

        // ========================================================================
          
        [Fact]
        public void ExpensesMethod_List_ReturnsListOfExpenses()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String existingDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(existingDB);
            SQLiteConnection conn = Database.dbConnection;

            Expenses expenses = new Expenses(conn, false);

            // Act
            List<Expense> list = expenses.List();

            // Assert
            Assert.Equal(numberOfExpensesInFile, list.Count);

        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_Add()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn, false);
         
            int category = 1;
            double amount = 98.1;

            // Act
            expenses.Add(DateTime.Now,category,amount,"new expense");
            List<Expense> expensesList = expenses.List();
            int sizeOfList = expenses.List().Count;

            // Assert
            Assert.Equal(numberOfExpensesInFile+1, sizeOfList);
            Assert.Equal(maxIDInExpenseFile + 1, expensesList[sizeOfList - 1].Id);
            Assert.Equal(amount, expensesList[sizeOfList - 1].Amount);
        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_AddInvalidCategory()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn, false);

            int category = 57;
            double amount = 98.1;

            // Act
            try
            {
                expenses.Add(DateTime.Now, category, amount, "new expense");
                List<Expense> expensesList = expenses.List();
                int sizeOfList = expenses.List().Count;
                Assert.Equal(sizeOfList, expenses.List().Count);
            }

            // Assert
            catch
            {
                Assert.True(false, "Invalid ID causes Delete to break");
            }
        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_Delete()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn, false);
            int idToDelete = 3;

            // Act
            expenses.Delete(idToDelete);
            List<Expense> expensesList = expenses.List();
            int sizeOfList = expensesList.Count;

            // Assert
            Assert.Equal(numberOfExpensesInFile - 1, sizeOfList);
            Assert.False(expensesList.Exists(e => e.Id == idToDelete), "correct expense item deleted");
        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_Delete_InvalidIDDoesntCrash()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messyDB";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn, false);
            int idToDelete = 1006;
            int sizeOfList = expenses.List().Count;

            // Act
            try
            {
                expenses.Delete(idToDelete);
                Assert.Equal(sizeOfList, expenses.List().Count);
            }

            // Assert
            catch
            {
                Assert.True(false, "Invalid ID causes Delete to break");
            }
        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_UpdateExpense()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            Database.newDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn, true);
            String newExpenseDesc = "new expense";
            int id = 1;
            int category = 2;
            double amount = 98.1;

            // Act
            expenses.Add(DateTime.Now, category, amount, "old expense");
            expenses.UpdateProperties(id, DateTime.Now, category, amount, newExpenseDesc);
           
            Expense expense = expenses.GetExpenseFromId(id);

            // Assert 
            Assert.Equal(newExpenseDesc, expense.Description);
            Assert.Equal(category, expense.Category);
        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_UpdateExpense_InvalidIDDoesntCrash()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messyDB";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn, false);
            int idToDelete = 1006;
            String newExpenseDesc = "new expense";
            int category = 2;
            double amount = 98.1;
            int sizeOfList = expenses.List().Count;

            // Act
            try
            {
                expenses.UpdateProperties(idToDelete, DateTime.Now, category, amount, newExpenseDesc);
                Assert.Equal(sizeOfList, expenses.List().Count);
            }

            // Assert
            catch
            {
                Assert.True(false, "Invalid ID causes Update to break");
            }
        }

        // -------------------------------------------------------
        // helpful functions, ... they are not tests
        // -------------------------------------------------------

        // source taken from: https://www.dotnetperls.com/file-equals

        private bool FileEquals(string path1, string path2)
        {
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);
            if (file1.Length == file2.Length)
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i] != file2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
