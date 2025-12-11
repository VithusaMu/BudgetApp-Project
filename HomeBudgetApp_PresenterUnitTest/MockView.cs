using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeBudgetAPI;
using HomeBudgetApp;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace HomeBudgetApp_PresenterUnitTest
{
    public class MockView : IViewInterface
    {
        public bool ShowCompletionCalled { get; private set; } = false;
        public bool ShowErrorCalled { get; private set; } = false;
        public bool DisplayCategoriesCalled { get; private set; } = false;
        public bool DisplayBudgetItemsCalled { get; private set; } = false;
        public bool DisplayBudgetItemsByCategoryCalled { get; private set; } = false;
        public bool DisplayBudgetItemsByMonthCalled { get; private set; } = false;
        public bool DisplayBudgetItemsByCategoryMonthCalled { get; private set; } = false;
        public bool ValidInputReceived { get; private set; } = false;
        public bool InvalidInputReceived { get; private set; } = false;
        public bool DisplayBudgetItemsByCategoryMonthAsGraphCalled { get; private set; } = false;
        public bool NextSearchCalled { get; private set; } = false;

        public void Reset()
        {
            ShowCompletionCalled = false;
            ShowErrorCalled = false;
            DisplayCategoriesCalled = false;
            DisplayBudgetItemsCalled = false;
            DisplayBudgetItemsByCategoryCalled = false;
            DisplayBudgetItemsByMonthCalled = false;
            DisplayBudgetItemsByCategoryMonthCalled = false;
            ValidInputReceived = false;
            InvalidInputReceived = false;
            DisplayBudgetItemsByCategoryMonthAsGraphCalled = false;
            NextSearchCalled = false;
        }

        public void ShowCompletion(string message)
        {
            ShowCompletionCalled = true;
            ValidInputReceived = true;
        }

        public void ShowError(string message)
        {
            ShowErrorCalled = true;
            InvalidInputReceived = true;
        }

        public void DisplayCategories(List<Category> categories)
        {
            DisplayCategoriesCalled = true;
        }

        public void DisplayBudgetItems(List<BudgetItem> budgetItems)
        {
            DisplayBudgetItemsCalled = true;
        }

        public void DisplayBudgetItemsByCategory(List<BudgetItemsByCategory> budgetItemsByCategory)
        {
            DisplayBudgetItemsByCategoryCalled = true;
        }

        public void DisplayBudgetItemsByMonth(List<BudgetItemsByMonth> budgetItemsByMonth)
        {
            DisplayBudgetItemsByMonthCalled = true;
        }

        public void DisplayBudgetItemsByCategoryMonth(List<Dictionary<string, object>> budgetItemsByCategoryMonth)
        {
            DisplayBudgetItemsByCategoryMonthCalled = true;
        }

        public void DisplayBudgetItemsByCategoryMonthAsGraph(List<Dictionary<string, object>> budgetItemsByCategoryMonth)
        {
            DisplayBudgetItemsByCategoryMonthAsGraphCalled = true;
        }

        public void NextSearch(object searchItem)
        {
            NextSearchCalled = true;
        }
    }
}
