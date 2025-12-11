using HomeBudgetAPI;
using System;
using System.Data.Common;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HomeBudgetApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewInterface
    {
        private Presenter _presenter;
        private List<Category> _categories;
        private NewExpense? expenseMenu = null;
        private EditExpense? editExpenseMenu = null;
        private DisplayBudgetItems? displayBudgetItems = null;

        public NewExpense? ExpenseMenuWindow
        {
            set { expenseMenu = value; }
            get { return expenseMenu; }
        }

        public EditExpense? EditExpenseMenuWindow
        {
            set { editExpenseMenu = value; }
            get { return editExpenseMenu; }
        }

        public DisplayBudgetItems? DisplayBudgetItemsWindow
        {
            set { displayBudgetItems = value; }
            get { return displayBudgetItems; }
        }

        public MainWindow()
        {
            InitializeComponent();

            _presenter = new Presenter(this);

            ComboBox_Themes.ItemsSource = Enum.GetValues(typeof(ThemeOptions));
            ComboBox_Themes.SelectedIndex = (int)Theme.CurrentTheme;
        }

        public void ShowCompletion(string message)
        {
            MessageBox.Show(message, "Completed", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void DisplayCategories(List<Category> categories)
        {
            _categories = categories;

            if (ExpenseMenuWindow != null)
            {
                ExpenseMenuWindow.cmb_Categories.ItemsSource = _categories;
            }

            if (EditExpenseMenuWindow != null)
            {
                EditExpenseMenuWindow.cmb_Categories.ItemsSource = _categories;
            }

            if (DisplayBudgetItemsWindow != null)
            {
                DisplayBudgetItemsWindow.cmb_Categories.ItemsSource = _categories;
            }
        }

        private void Btn_CatButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(chosenDatabaseFile.Text))
            {
                ShowError("Please choose a budget file");
            }
            else
            {
                this.Hide();
                NewCategory categoryMenu = new NewCategory(_presenter);
                categoryMenu.ShowDialog();
                this.Show();
            }

        }

        private void Btn_ExpButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(chosenDatabaseFile.Text))
            {
                ShowError("Please choose a budget file");
            }
            else
            {
                this.Hide();
                ExpenseMenuWindow = new NewExpense(_presenter);
                _presenter.SetCategories(); 
                ExpenseMenuWindow.ShowDialog();
                this.Show();
            }
        }

        private void ComboBox_Themes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Theme.ChangeTheme((ThemeOptions)ComboBox_Themes.SelectedIndex);
        }

        private void Btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Btn_File_Clicked(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Entry _entry = new Entry(_presenter);
            _entry.ShowDialog();

            if (string.IsNullOrEmpty(_entry.selectedNewDbName.Text))
            {
                chosenDatabaseFile.Text = _entry.txtEditor.Text;
            }
            else
            {
                chosenDatabaseFile.Text = _entry.selectedNewDbName.Text;
            }
            this.Show();
        }

        private void ViewItem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(chosenDatabaseFile.Text))
            {
                ShowError("Please choose a budget file");
            }
            else
            {
                this.Hide();
                DisplayBudgetItemsWindow = new DisplayBudgetItems(_presenter, this);
                _presenter.SetCategories();
                _presenter.SetBudgetItems(DateTime.Today.AddYears(-1), DateTime.Today.AddDays(1).AddMinutes(-1), false, 0, false, false, false);
                DisplayBudgetItemsWindow.ShowDialog();
                this.Show();
            }
        }

        public void DisplayBudgetItems(List<BudgetItem> budgetItems)
        {
            //get data grid
            DataGrid dataGrid = displayBudgetItems!.DG_BudgetItems;
            displayBudgetItems!.CM_EditExpense.IsEnabled = true;
            displayBudgetItems!.btn_search.IsEnabled = true;

            //data grid init setup
            dataGrid.ItemsSource = budgetItems;
            dataGrid.Columns.Clear();

            //create columns
            DataGridTextColumn date = new DataGridTextColumn();
            date.Header = "Date";
            date.Binding = new Binding("Date") { StringFormat = "dd/MM/yyyy" };

            Style dateStyle = new Style();
            dateStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
            date.CellStyle = dateStyle;

            dataGrid.Columns.Add(date);

            DataGridTextColumn category = new DataGridTextColumn();
            category.Header = "Category";
            category.Binding = new Binding("Category");
            dataGrid.Columns.Add(category);

            DataGridTextColumn description = new DataGridTextColumn();
            description.Header = "Description";
            description.Binding = new Binding("ShortDescription");
            dataGrid.Columns.Add(description);

            DataGridTextColumn amount = new DataGridTextColumn();
            amount.Header = "Amount";
            amount.Binding = new Binding("Amount")
            {
                StringFormat = "F2" 
            };
            Style amountStyle = new Style();
            amountStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
            amount.CellStyle = amountStyle;
            dataGrid.Columns.Add(amount);

            DataGridTextColumn balance = new DataGridTextColumn();
            balance.Header = "Balance";
            balance.Binding = new Binding("Balance")
            {
                StringFormat = "F2"
            };

            Style balanceStyle = new Style();
            balanceStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
            balance.CellStyle = balanceStyle;


            dataGrid.Columns.Add(balance);
        }

        public void DisplayBudgetItemsByCategory(List<BudgetItemsByCategory> budgetItemsByCategory)
        {
            //get data grid
            DataGrid dataGrid = displayBudgetItems!.DG_BudgetItems;
            displayBudgetItems!.CM_EditExpense.IsEnabled = false;
            displayBudgetItems!.btn_search.IsEnabled = false;

            //data grid init setup
            dataGrid.ItemsSource = budgetItemsByCategory;
            dataGrid.Columns.Clear();

            //create columns
            DataGridTextColumn category = new DataGridTextColumn();
            category.Header = "Category";
            category.Binding = new Binding("Category");
            dataGrid.Columns.Add(category);

            DataGridTextColumn total = new DataGridTextColumn();
            total.Header = "Total";
            total.Binding = new Binding("Total")
            {
                StringFormat = "F2"
            };

            Style totalStyle = new Style();
            totalStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
            total.CellStyle = totalStyle;

            dataGrid.Columns.Add(total);
        }

        public void DisplayBudgetItemsByMonth(List<BudgetItemsByMonth> budgetItemsByMonth)
        {
            //get data grid
            DataGrid dataGrid = displayBudgetItems!.DG_BudgetItems;
            displayBudgetItems!.CM_EditExpense.IsEnabled = false;
            displayBudgetItems!.btn_search.IsEnabled = false;

            //data grid init setup
            dataGrid.ItemsSource = budgetItemsByMonth;
            dataGrid.Columns.Clear();

            //create columns
            DataGridTextColumn month = new DataGridTextColumn();
            month.Header = "Month";
            month.Binding = new Binding("Month");
            dataGrid.Columns.Add(month);

            DataGridTextColumn total = new DataGridTextColumn();
            total.Header = "Total";
            total.Binding = new Binding("Total")
            {
                StringFormat = "F2"
            };

            Style totalStyle = new Style();
            totalStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
            total.CellStyle = totalStyle;

            dataGrid.Columns.Add(total);
        }

        public void DisplayBudgetItemsByCategoryMonth(List<Dictionary<string, object>> budgetItemsByCategoryMonth)
        {
            //get data grid
            DataGrid dataGrid = displayBudgetItems!.DG_BudgetItems;
            displayBudgetItems!.CM_EditExpense.IsEnabled = false;
            displayBudgetItems!.btn_search.IsEnabled = false;

            //get categories
            //_presenter.SetCategories();
            List<Category> categories = _categories;

            //clear existing columns and set the data source
            dataGrid.Columns.Clear();
            dataGrid.ItemsSource = budgetItemsByCategoryMonth;

            //create Month column
            DataGridTextColumn month = new DataGridTextColumn();
            month.Header = "Month";
            month.Binding = new Binding("[Month]");
            dataGrid.Columns.Add(month);

            //create category columns
            foreach (Category category in categories)
            {
                DataGridTextColumn dataCategory = new DataGridTextColumn();
                dataCategory.Header = category.Description;
                dataCategory.Binding = new Binding($"[{category.Description}]");
                dataGrid.Columns.Add(dataCategory);
            }

            //create total column
            DataGridTextColumn total = new DataGridTextColumn();
            total.Header = "Total";
            total.Binding = new Binding("[Total]")
            {
                StringFormat = "F2"
            };

            Style totalStyle = new Style();
            totalStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
            total.CellStyle = totalStyle;

            dataGrid.Columns.Add(total);
        }

        public void DisplayBudgetItemsByCategoryMonthAsGraph(List<Dictionary<string, object>> budgetItemsByCategoryMonth)
        {
            //get data grid
            displayBudgetItems!.CM_EditExpense.IsEnabled = false;

            Chart chPie = displayBudgetItems.chPie;
            ComboBox cbMonths = displayBudgetItems.cbMonths;

            //gets months
            List<string> months = new List<string>();
            foreach (object obj in budgetItemsByCategoryMonth)
            {
                Dictionary<string, object>? item = obj as Dictionary<string, object>;
                if (item != null)
                {
                    if (item["Month"].ToString() != "TOTALS")
                        months.Add(item["Month"].ToString());
                }
            }

            //add the months to the combobox dropdown
            cbMonths.ItemsSource = months;

            //clears it
            ((PieSeries)chPie.Series[0]).ItemsSource = null;

            //exits if needed
            if (cbMonths.Items.Count == 0) return;

            //sets a default to 0
            if (cbMonths.SelectedIndex < 0 || cbMonths.SelectedIndex > cbMonths.Items.Count - 1)
            {
                cbMonths.SelectedIndex = 0;
            }

            //get selected item
            string? selectedMonth = cbMonths.SelectedItem.ToString();

            List<KeyValuePair<string, double>> DisplayData = new List<KeyValuePair<string, double>>();

            foreach (object obj in budgetItemsByCategoryMonth)
            {
                Dictionary<string, object>? item = obj as Dictionary<string, object>;

                //is the item listed in the _dataSource part of the selected month?
                if (item != null && (string)item["Month"] == selectedMonth)
                {
                    //go through each key/value pair in this item (item is a dictionary)
                    foreach (var pair in item)
                    {
                        string category = pair.Key;
                        string? value = pair.Value.ToString();

                        if (category == "Total" || category == "Month" || category.Contains("details:")) continue;

                        double amount = 0.0;
                        double.TryParse(value, out amount);

                        if (amount < 0)
                        {
                            DisplayData.Add(new KeyValuePair<string, double>(category, -amount));
                        }
                        else
                        {
                            DisplayData.Add(new KeyValuePair<string, double>(category, amount));
                        }
                    }

                    //we have the month we wanted...
                    break;
                }
            }

            // set the data for the pie-chart
            ((PieSeries)chPie.Series[0]).ItemsSource = DisplayData;
        }

        public void NextSearch(object budgetItem)
        {

            if (displayBudgetItems != null)
            {
                displayBudgetItems.DG_BudgetItems.SelectedItem = budgetItem;
                displayBudgetItems.DG_BudgetItems.ScrollIntoView(displayBudgetItems.DG_BudgetItems.SelectedItem);
                displayBudgetItems.DG_BudgetItems.Focus();
            }
        }
    }
}