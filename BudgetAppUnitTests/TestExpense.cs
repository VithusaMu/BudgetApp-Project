using System;
using Xunit;
using HomeBudgetAPI;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace BudgetCodeTests
{
    [Collection("Sequential")]
    public class TestExpense
    {
        // ========================================================================

        [Fact]
        public void ExpenseObject_New()
        {

            // Arrange
            DateTime now = DateTime.Now;
            double amount = 24.55;
            string descr = "New Sweater";
            int category = 34;
            int id = 42;

            // Act
            Expense expense = new Expense(id, now, category, amount, descr);

            // Assert 
            Assert.IsType<Expense>(expense);

            Assert.Equal(id, expense.Id);
            Assert.Equal(amount, expense.Amount);
            Assert.Equal(descr, expense.Description);
            Assert.Equal(category, expense.Category);
            Assert.Equal(now, expense.Date);
        }

        // ========================================================================
       


        // ========================================================================

        [Fact]
        public void ExpenseObject_PropertiesAreReadOnly()
        {
            // Arrange
            DateTime now = DateTime.Now;
            double amount = 24.55;
            string descr = "New Sweater";
            int category = 34;
            int id = 42;

            // Act
            Expense expense = new Expense(id, now, category, amount, descr);

            // Assert 
            Assert.IsType<Expense>(expense);
            Assert.True(typeof(Expense).GetProperty("Id").CanWrite == false);
            Assert.True(typeof(Expense).GetProperty("Date").CanWrite == false);
            Assert.True(typeof(Expense).GetProperty("Amount").CanWrite == false);
            Assert.True(typeof(Expense).GetProperty("Description").CanWrite == false);
            Assert.True(typeof(Expense).GetProperty("Category").CanWrite == false);
        }
    }
}
