using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeBudgetAPI;
using HomeBudgetApp;

namespace HomeBudgetApp_PresenterUnitTest
{
    public class PresenterTests
    {
        private readonly MockView _mockView;
        private readonly Presenter _presenter;
        private readonly string _testDbPath = "./newDb.db";

        public PresenterTests()
        {
            _mockView = new MockView();
            _presenter = new Presenter(_mockView);
            _presenter.ProcessFile(_testDbPath, true);
        }

        [Fact]
        public void AddCategory_ValidInput_SetsCorrectFlags()
        {
            // Arrange
            _mockView.Reset();

            // Act
            _presenter.AddCategory("Test", Category.CategoryType.Expense);

            // Assert
            Assert.True(_mockView.ShowCompletionCalled);
            Assert.True(_mockView.ValidInputReceived);
            Assert.False(_mockView.ShowErrorCalled);
        }

        [Fact]
        public void AddCategory_NullInput_SetsErrorFlags()
        {
            // Arrange
            _mockView.Reset();

            // Act
            _presenter.AddCategory(null, null);

            // Assert
            Assert.True(_mockView.ShowErrorCalled);
            Assert.True(_mockView.InvalidInputReceived);
            Assert.False(_mockView.ShowCompletionCalled);
        }

        [Fact]
        public void AddExpense_ValidInput_SetsCorrectFlags()
        {
            // Arrange
            _mockView.Reset();
            _presenter.AddCategory("Test", Category.CategoryType.Expense);
            _mockView.Reset();

            // Act
            _presenter.AddExpense(DateTime.Now, 1, "50.00", "Test Expense");

            // Assert
            Assert.True(_mockView.ShowCompletionCalled);
            Assert.True(_mockView.ValidInputReceived);
            Assert.False(_mockView.ShowErrorCalled);
        }

        [Fact]
        public void AddExpense_InvalidInput_SetsErrorFlags()
        {
            // Arrange
            _mockView.Reset();

            // Act
            _presenter.AddExpense(null, null, null, null);

            // Assert
            Assert.True(_mockView.ShowErrorCalled);
            Assert.True(_mockView.InvalidInputReceived);
            Assert.False(_mockView.ShowCompletionCalled);
        }

        [Fact]
        public void AddCategory_SingleValidCategory_SetsValidFlags()
        {
            // Arrange
            _mockView.Reset();

            // Act
            _presenter.AddCategory("Test", Category.CategoryType.Expense);

            // Assert
            Assert.True(_mockView.ShowCompletionCalled);
            Assert.True(_mockView.ValidInputReceived);
        }


        [Fact]
        public void GetCategories_SetsDisplayFlag()
        {
            // Arrange
            _mockView.Reset();

            // Act
            _presenter.SetCategories();

            // Assert
            Assert.True(_mockView.DisplayCategoriesCalled);
        }

        [Fact]
        public void AddExpense_NegativeAmount_SetsValidFlags()
        {
            // Arrange
            _mockView.Reset();
            _presenter.AddCategory("Test", Category.CategoryType.Expense);
            _mockView.Reset();

            // Act
            _presenter.AddExpense(DateTime.Now, 1, "-100.00", "Refund");

            // Assert
            Assert.True(_mockView.ShowCompletionCalled);
            Assert.True(_mockView.ValidInputReceived);
            Assert.False(_mockView.ShowErrorCalled);
        }

        [Fact]
        public void AddExpense_FutureDate_SetsValidFlags()
        {
            // Arrange
            _mockView.Reset();
            _presenter.AddCategory("Test", Category.CategoryType.Expense);
            _mockView.Reset();

            // Act
            _presenter.AddExpense(DateTime.Now.AddYears(1), 1, "100.00", "Future Payment");

            // Assert
            Assert.True(_mockView.ShowCompletionCalled);
            Assert.True(_mockView.ValidInputReceived);
            Assert.False(_mockView.ShowErrorCalled);
        }

        [Fact]
        public void AddCategory_MultipleValidCategories_SetsCorrectFlags()
        {
            // Arrange
            _mockView.Reset();

            // Act
            _presenter.AddCategory("Utilities", Category.CategoryType.Expense);
            _presenter.AddCategory("Income", Category.CategoryType.Income);
            _presenter.AddCategory("Savings", Category.CategoryType.Credit);

            // Assert
            Assert.True(_mockView.ShowCompletionCalled);
            Assert.True(_mockView.ValidInputReceived);
            Assert.False(_mockView.ShowErrorCalled);
        }

        [Fact]
        public void AddCategoryandExpense_SetsCorrectFlags()
        {
            // Arrange
            _mockView.Reset();

            // Act
            _presenter.AddCategory("Test", Category.CategoryType.Expense);
            _presenter.AddExpense(DateTime.Now, 1, "50.00", "Test Expense");
            _presenter.SetCategories();

            // Assert
            Assert.True(_mockView.ShowCompletionCalled);
            Assert.True(_mockView.DisplayCategoriesCalled);
            Assert.False(_mockView.ShowErrorCalled);
        }

        [Fact]
        public void ProcessFile_NewDatabase_AddCategory_AddExpense()
        {
            // Arrange
            _mockView.Reset();
            _presenter.ProcessFile("./newDb.db", true);

            // Act 
            _presenter.AddCategory("Test", Category.CategoryType.Expense);
            _presenter.AddExpense(DateTime.Now, 1, "100.00", "Test Expense");
            _presenter.SetCategories();

            // Assert
            Assert.True(_mockView.ShowCompletionCalled);
            Assert.True(_mockView.DisplayCategoriesCalled);
            Assert.False(_mockView.ShowErrorCalled);
        }


        [Fact]
        public void UpdateExpense_ValidInput_SetsCorrectFlags()
        {
            // Arrange
            _mockView.Reset();
            DateTime testDate = DateTime.Now;
            string testAmount = "75.00";
            string testDescription = "Updated Test Expense";
            _presenter.AddCategory("Test", Category.CategoryType.Expense);
            _presenter.AddExpense(testDate, 1, "50.00", "Original Test Expense");

            // Act
            _presenter.UpdateExpense(1, testDate, 1, testAmount, testDescription);

            // Assert
            Assert.True(_mockView.ShowCompletionCalled);
            Assert.True(_mockView.ValidInputReceived);
            Assert.False(_mockView.ShowErrorCalled);
        }

        [Fact]
        public void UpdateExpense_MultipleFieldsInvalid_SetsErrorFlags()
        {
            // Arrange
            _mockView.Reset();
            int expenseId = 1;
            DateTime? nullDate = null;
            int? nullCategoryId = null;
            string invalidAmount = "invalid_amount";
            string emptyDescription = "";

            _presenter.AddCategory("Test", Category.CategoryType.Expense);
            _presenter.AddExpense(DateTime.Now, 1, "50.00", "Test Expense");

            _mockView.Reset();

            // Act
            _presenter.UpdateExpense(expenseId, nullDate, nullCategoryId, invalidAmount, emptyDescription);

            // Assert
            Assert.True(_mockView.ShowErrorCalled, "ShowError should be called for invalid fields");
            Assert.True(_mockView.InvalidInputReceived, "InvalidInput flag should be set for invalid fields");
            Assert.False(_mockView.ShowCompletionCalled, "ShowCompletion should not be called when fields are invalid");
            Assert.False(_mockView.ValidInputReceived, "ValidInput flag should not be set when fields are invalid");
        }

        [Fact]
        public void UpdateExpense_InvalidAmount_SetsErrorFlags()
        {
            // Arrange
            _mockView.Reset();
            DateTime testDate = DateTime.Now;
            int categoryId = 1;
            int expenseId = 1;
            string invalidAmount = "";
            string description = "Valid Description";

            _presenter.AddCategory("Test", Category.CategoryType.Expense);
            _presenter.AddExpense(testDate, categoryId, "50.00", "Test Expense");

            _mockView.Reset();

            // Act
            _presenter.UpdateExpense(expenseId, testDate, categoryId, invalidAmount, description);

            // Assert
            Assert.True(_mockView.ShowErrorCalled, "ShowError should be called for invalid amount");
            Assert.True(_mockView.InvalidInputReceived, "InvalidInput flag should be set for invalid amount");
            Assert.False(_mockView.ShowCompletionCalled, "ShowCompletion should not be called for invalid amount");
            Assert.False(_mockView.ValidInputReceived, "ValidInput flag should not be set for invalid amount");
        }


        [Fact]
        public void UpdateExpense_NegativeAmount_SetsValidFlags()
        {
            // Arrange
            _mockView.Reset();
            DateTime testDate = DateTime.Now;
            string negativeAmount = "-75.00";
            _presenter.AddCategory("Test", Category.CategoryType.Expense);
            _presenter.AddExpense(testDate, 1, "50.00", "Test Expense");

            // Act
            _presenter.UpdateExpense(1, testDate, 1, negativeAmount, "Updated Test Expense");

            // Assert
            Assert.True(_mockView.ShowCompletionCalled);
            Assert.True(_mockView.ValidInputReceived);
            Assert.False(_mockView.ShowErrorCalled);
        }

        [Fact]
        public void UpdateExpense_FutureDate_SetsValidFlags()
        {
            // Arrange
            _mockView.Reset();
            DateTime futureDate = DateTime.Now.AddYears(1);
            _presenter.AddCategory("Test", Category.CategoryType.Expense);
            _presenter.AddExpense(DateTime.Now, 1, "50.00", "Test Expense");

            // Act
            _presenter.UpdateExpense(1, futureDate, 1, "75.00", "Future Updated Expense");

            // Assert
            Assert.True(_mockView.ShowCompletionCalled);
            Assert.True(_mockView.ValidInputReceived);
            Assert.False(_mockView.ShowErrorCalled);
        }

        [Fact]
        public void UpdateExpense_LargeAmount()
        {
            // Arrange
            _mockView.Reset();
            DateTime testDate = DateTime.Now;
            string largeAmount = "999999999.99";
            _presenter.AddCategory("Test", Category.CategoryType.Expense);
            _presenter.AddExpense(testDate, 1, "50.00", "Test Expense");

            // Act
            _presenter.UpdateExpense(1, testDate, 1, largeAmount, "Large Amount Expense");

            // Assert
            Assert.True(_mockView.ShowCompletionCalled);
            Assert.True(_mockView.ValidInputReceived);
            Assert.False(_mockView.ShowErrorCalled);
        }

        [Fact]
        public void SetBudgetItems_NoSorting_WithDateRange()
        {
            // Arrange
            _mockView.Reset();
            DateTime startDate = DateTime.Now.AddMonths(-1);
            DateTime endDate = DateTime.Now.AddMonths(1);
            
            //Act
            _presenter.SetBudgetItems(startDate, endDate, false, 1, false, false, false);

            // Assert
            Assert.True(_mockView.DisplayBudgetItemsCalled);
            Assert.False(_mockView.DisplayBudgetItemsByCategoryCalled);
            Assert.False(_mockView.DisplayBudgetItemsByMonthCalled);
            Assert.False(_mockView.DisplayBudgetItemsByCategoryMonthCalled);
        }

        [Fact]
        public void SetBudgetItems_SortByCategory_WithFilter()
        {
            // Arrange
            _mockView.Reset();
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddMonths(1);
            bool filterFlag = true;
            int specificCategoryId = 1;

            // Act
            _presenter.SetBudgetItems(startDate, endDate, filterFlag, specificCategoryId, true, false, false);

            // Assert
            Assert.True(_mockView.DisplayBudgetItemsByCategoryCalled);
            Assert.False(_mockView.DisplayBudgetItemsCalled);
            Assert.False(_mockView.DisplayBudgetItemsByMonthCalled);
            Assert.False(_mockView.DisplayBudgetItemsByCategoryMonthCalled);
        }

        [Fact]
        public void SetBudgetItems_SortByMonth_FullYearRange()
        {
            // Arrange
            _mockView.Reset();
            DateTime startDate = DateTime.Now.AddYears(-1);
            DateTime endDate = DateTime.Now;

            // Act
            _presenter.SetBudgetItems(startDate, endDate, false, 1, false, true, false);

            // Assert
            Assert.True(_mockView.DisplayBudgetItemsByMonthCalled);
            Assert.False(_mockView.DisplayBudgetItemsCalled);
            Assert.False(_mockView.DisplayBudgetItemsByCategoryCalled);
            Assert.False(_mockView.DisplayBudgetItemsByCategoryMonthCalled);
        }

        [Fact]
        public void SetBudgetItems_SortByBoth_WithFilterAndDateRange()
        {
            // Arrange
            _mockView.Reset();
            DateTime startDate = DateTime.Now.AddMonths(-3);
            DateTime endDate = DateTime.Now;
            bool filterFlag = true;
            int specificCategoryId = 1;

            // Act
            _presenter.SetBudgetItems(startDate, endDate, filterFlag, specificCategoryId, true, true, false);

            // Assert
            Assert.True(_mockView.DisplayBudgetItemsByCategoryMonthCalled);
            Assert.False(_mockView.DisplayBudgetItemsCalled);
            Assert.False(_mockView.DisplayBudgetItemsByCategoryCalled);
            Assert.False(_mockView.DisplayBudgetItemsByMonthCalled);
        }

        [Fact]
        public void SetBudgetItems_NullDates_WithCategoryFilter()
        {
            // Arrange
            _mockView.Reset();
            int specificCategoryId = 1;
            bool filterFlag = true;

            // Act
            _presenter.SetBudgetItems(null, null, filterFlag, specificCategoryId, false, false, false);

            // Assert
            Assert.True(_mockView.DisplayBudgetItemsCalled);
            Assert.False(_mockView.DisplayBudgetItemsByCategoryCalled);
            Assert.False(_mockView.DisplayBudgetItemsByMonthCalled);
            Assert.False(_mockView.DisplayBudgetItemsByCategoryMonthCalled);
        }



        [Fact]
        public void Search_EmptyInput_SetsErrorFlags()
        {
            // Arrange
            _mockView.Reset();
            List<BudgetItem> budgetItems = new List<BudgetItem>();

            // Act
            _presenter.Search("", budgetItems, 0);

            // Assert
            Assert.True(_mockView.ShowErrorCalled);
            Assert.False(_mockView.NextSearchCalled);
        }

        [Fact]
        public void SetBudgetItems_GraphDisplay_SetsCorrectFlags()
        {
            // Arrange
            _mockView.Reset();
            DateTime startDate = DateTime.Now.AddMonths(-1);
            DateTime endDate = DateTime.Now;

            // Act
            _presenter.SetBudgetItems(startDate, endDate, false, 1, true, true, true);

            // Assert
            Assert.True(_mockView.DisplayBudgetItemsByCategoryMonthAsGraphCalled);
            Assert.False(_mockView.DisplayBudgetItemsByCategoryMonthCalled);
            Assert.False(_mockView.DisplayBudgetItemsCalled);
        }

        [Fact]
        public void SetBudgetItems_GraphDisplay_WithFilter()
        {
            // Arrange
            _mockView.Reset();
            DateTime startDate = DateTime.Now.AddMonths(-1);
            DateTime endDate = DateTime.Now;

            // Act
            _presenter.SetBudgetItems(startDate, endDate, true, 1, true, true, true);

            // Assert
            Assert.True(_mockView.DisplayBudgetItemsByCategoryMonthAsGraphCalled);
            Assert.False(_mockView.DisplayBudgetItemsByCategoryMonthCalled);
        }



        [Fact]
        public void SetBudgetItems_GraphDisplay_EmptyData()
        {
            // Arrange
            _mockView.Reset();
            DateTime startDate = DateTime.Now.AddYears(1);
            DateTime endDate = DateTime.Now.AddYears(2);

            // Act
            _presenter.SetBudgetItems(startDate, endDate, false, 1, true, true, true);

            // Assert
            Assert.True(_mockView.DisplayBudgetItemsByCategoryMonthAsGraphCalled);
        }


        [Fact]
        public void SetBudgetItems_AllDisplayModes_Validation()
        {
            // Arrange
            _mockView.Reset();
            DateTime startDate = DateTime.Now.AddMonths(-1);
            DateTime endDate = DateTime.Now;

            // Test all display modes
            // Normal display
            _presenter.SetBudgetItems(startDate, endDate, false, 1, false, false, false);
            Assert.True(_mockView.DisplayBudgetItemsCalled);

            _mockView.Reset();
            // Category display
            _presenter.SetBudgetItems(startDate, endDate, false, 1, true, false, false);
            Assert.True(_mockView.DisplayBudgetItemsByCategoryCalled);

            _mockView.Reset();
            // Month display
            _presenter.SetBudgetItems(startDate, endDate, false, 1, false, true, false);
            Assert.True(_mockView.DisplayBudgetItemsByMonthCalled);

            _mockView.Reset();
            // Category and Month display
            _presenter.SetBudgetItems(startDate, endDate, false, 1, true, true, false);
            Assert.True(_mockView.DisplayBudgetItemsByCategoryMonthCalled);

            _mockView.Reset();
            // Graph display
            _presenter.SetBudgetItems(startDate, endDate, false, 1, true, true, true);
            Assert.True(_mockView.DisplayBudgetItemsByCategoryMonthAsGraphCalled);
        }

     


        [Fact]
        public void Search_ValidInput_SetsCorrectFlags()
        {
            // Arrange
            _mockView.Reset();
            List<BudgetItem> budgetItems = new List<BudgetItem>
            {
                new BudgetItem
                {
                    ExpenseID = 1,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    Category = "Test",
                    ShortDescription = "Test Expense",
                    Amount = 50.00,
                    Balance = 50.00
                }
            };

            // Act
            _presenter.Search("Test", budgetItems, 0);

            // Assert
            Assert.True(_mockView.NextSearchCalled);
            Assert.False(_mockView.ShowErrorCalled);
        }

        [Fact]
        public void Search_NoMatchFound_SetsErrorFlags()
        {
            // Arrange
            _mockView.Reset();
            List<BudgetItem> budgetItems = new List<BudgetItem>
            {
                new BudgetItem
                {
                    ExpenseID = 1,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    Category = "Test",
                    ShortDescription = "Test Expense",
                    Amount = 50.00,
                    Balance = 50.00
                }
            };

            // Act
            _presenter.Search("NonexistentTerm", budgetItems, 0);

            // Assert
            Assert.True(_mockView.ShowErrorCalled);
            Assert.False(_mockView.NextSearchCalled);
        }

        [Fact]
        public void Search_MultipleMatches_ReturnsFirstMatch()
        {
            // Arrange
            _mockView.Reset();
            List<BudgetItem> budgetItems = new List<BudgetItem>
            {
                new BudgetItem
                {
                    ExpenseID = 1,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    Category = "Test",
                    ShortDescription = "Test Expense 1",
                    Amount = 50.00,
                    Balance = 50.00
                },
                new BudgetItem
                {
                    ExpenseID = 2,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    Category = "Test",
                    ShortDescription = "Test Expense 2",
                    Amount = 60.00,
                    Balance = 110.00
                }
            };

            // Act
            _presenter.Search("Test", budgetItems, 0);

            // Assert
            Assert.True(_mockView.NextSearchCalled);
            Assert.False(_mockView.ShowErrorCalled);
        }

        [Fact]
        public void Search_WithSelectedIndex_ReturnsCorrectMatch()
        {
            // Arrange
            _mockView.Reset();
            List<BudgetItem> budgetItems = new List<BudgetItem>
                {
                    new BudgetItem
                    {
                        ExpenseID = 1,
                        Date = DateTime.Now,
                        CategoryID = 1,
                        Category = "Test",
                        ShortDescription = "First Test",
                        Amount = 50.00,
                        Balance = 50.00
                    },
                    new BudgetItem
                    {
                        ExpenseID = 2,
                        Date = DateTime.Now,
                        CategoryID = 1,
                        Category = "Test",
                        ShortDescription = "Second Test",
                        Amount = 60.00,
                        Balance = 110.00
                    }
                };

            // Act
            _presenter.Search("Test", budgetItems, 1);

            // Assert
            Assert.True(_mockView.NextSearchCalled);
            Assert.False(_mockView.ShowErrorCalled);
        }

        [Fact]
        public void Search_MultipleSearches_ResetsBetweenSearches()
        {
            // Arrange
            _mockView.Reset();
            List<BudgetItem> budgetItems = new List<BudgetItem>
            {
                new BudgetItem
                {
                    ExpenseID = 1,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    Category = "Test",
                    ShortDescription = "First Test",
                    Amount = 50.00,
                    Balance = 50.00
                },
                new BudgetItem
                {
                    ExpenseID = 2,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    Category = "Test",
                    ShortDescription = "Second Test",
                    Amount = 60.00,
                    Balance = 110.00
                }
            };

            // First search
            _presenter.Search("First", budgetItems, 0);
            Assert.True(_mockView.NextSearchCalled);

            // Reset for second search
            _mockView.Reset();

            // Second search
            _presenter.Search("Second", budgetItems, 0);
            Assert.True(_mockView.NextSearchCalled);
        }

        [Fact]

        public void Search_InvalidSelectedIndex_HandlesGracefully()
        {
            // Arrange
            _mockView.Reset();
            List<BudgetItem> budgetItems = new List<BudgetItem>
            {
                new BudgetItem
                {
                    ExpenseID = 1,
                    Date = DateTime.Now,
                    CategoryID = 1,
                    Category = "Test",
                    ShortDescription = "Test Expense",
                    Amount = 50.00,
                    Balance = 50.00
                }
            };

            // Act
            _presenter.Search("Test", budgetItems, 999); 

            // Assert
            Assert.True(_mockView.NextSearchCalled);
            Assert.False(_mockView.ShowErrorCalled);
        }

        [Fact]
        public void Search_EmptyBudgetItems_SetsErrorFlags()
        {
            // Arrange
            _mockView.Reset();
            List<BudgetItem> budgetItems = new List<BudgetItem>();

            // Act
            _presenter.Search("test", budgetItems, 0);

            // Assert
            Assert.True(_mockView.ShowErrorCalled);
            Assert.False(_mockView.NextSearchCalled);
        }

    }
}
