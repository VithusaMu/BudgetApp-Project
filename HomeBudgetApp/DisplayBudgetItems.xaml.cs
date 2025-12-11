using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HomeBudgetAPI;

namespace HomeBudgetApp
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class DisplayBudgetItems : Window
    {
        private Presenter _presenter;

        private DateTime? _startDate = DateTime.Today.AddYears(-1);
        private DateTime? _endDate = DateTime.Today.AddDays(1);
        private bool _filterSelected;
        private int _categoryId;
        private bool _sortByMonth;
        private bool _sortByCategory;
        private MainWindow _mainWindow;

        public DisplayBudgetItems(Presenter presenter, MainWindow main)
        {
            InitializeComponent();
            StartDate.SelectedDate = _startDate;
            EndDate.SelectedDate = _endDate;

            _presenter = presenter;
            _mainWindow = main;

            ComboBox_Themes.ItemsSource = Enum.GetValues(typeof(ThemeOptions));
            ComboBox_Themes.SelectedIndex = (int)Theme.CurrentTheme;
        }

        private void ComboBox_Themes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Theme.ChangeTheme((ThemeOptions)ComboBox_Themes.SelectedIndex);
        }

        private void StartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker? datePicker = sender as DatePicker;

            _startDate = datePicker!.SelectedDate;
            
            if (_presenter != null) 
                _presenter.SetBudgetItems(_startDate, _endDate, _filterSelected, _categoryId, _sortByCategory, _sortByMonth, false);

        }

        private void EndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker? datePicker = sender as DatePicker;
   
            _endDate = datePicker!.SelectedDate;

            if (_presenter != null)
                _presenter.SetBudgetItems(_startDate, _endDate, _filterSelected, _categoryId, _sortByCategory, _sortByMonth, false);
        }


        private void Filter_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox? checkBox = sender as CheckBox;
            bool? isChecked = checkBox!.IsChecked;
            if (isChecked.HasValue)
            {
                _filterSelected = (bool)isChecked;
                cmb_Categories.IsEnabled = true;    
            }

            _presenter.SetBudgetItems(_startDate, _endDate, _filterSelected, _categoryId, _sortByCategory, _sortByMonth, false);
        }

        private void Filter_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox? checkBox = sender as CheckBox;
            bool? isChecked = checkBox!.IsChecked;

            if (isChecked.HasValue)
            {  
                _filterSelected = (bool)isChecked;
                cmb_Categories.IsEnabled = false;

                cmb_Categories.SelectedItem = null;
            }


            _presenter.SetBudgetItems(_startDate, _endDate, _filterSelected, _categoryId, _sortByCategory, _sortByMonth, false);
        }


        private void cmb_Categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox? comboBox = sender as ComboBox;
            _categoryId = comboBox!.SelectedIndex + 1;

            _presenter.SetBudgetItems(_startDate, _endDate, _filterSelected, _categoryId, _sortByCategory, _sortByMonth, false);
        }

        private void Month_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox? checkBox = sender as CheckBox;
            bool? isChecked = checkBox!.IsChecked;
            if (isChecked.HasValue)
                _sortByMonth = (bool)isChecked;

            if (_sortByMonth == true && _sortByCategory == true)
                PieChart.IsEnabled = true;
            else
                PieChart.IsEnabled = false;

            TogglePieChartCheckBox();

            _presenter.SetBudgetItems(_startDate, _endDate, _filterSelected, _categoryId, _sortByCategory, _sortByMonth, false);
        }

        private void Month_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox? checkBox = sender as CheckBox;
            bool? isChecked = checkBox!.IsChecked;
            if (isChecked.HasValue)
                _sortByMonth = (bool)isChecked;

            TogglePieChartCheckBox();

            _presenter.SetBudgetItems(_startDate, _endDate, _filterSelected, _categoryId, _sortByCategory, _sortByMonth, false);
        }

        private void Category_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox? checkBox = sender as CheckBox;
            bool? isChecked = checkBox!.IsChecked;
            if (isChecked.HasValue)
                _sortByCategory = (bool)isChecked;

            TogglePieChartCheckBox();

            _presenter.SetBudgetItems(_startDate, _endDate, _filterSelected, _categoryId, _sortByCategory, _sortByMonth, false);
        }

        private void Category_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox? checkBox = sender as CheckBox;
            bool? isChecked = checkBox!.IsChecked;
            if (isChecked.HasValue)
                _sortByCategory = (bool)isChecked;

            TogglePieChartCheckBox();

            _presenter.SetBudgetItems(_startDate, _endDate, _filterSelected, _categoryId, _sortByCategory, _sortByMonth, false);
        }

        private void Add_Expense_Clicked(object sender, RoutedEventArgs e)
        {
            _mainWindow.ExpenseMenuWindow = new NewExpense(_presenter);
            _presenter.SetCategories();
            _mainWindow.ExpenseMenuWindow.ShowDialog();
            _presenter.SetBudgetItems(_startDate, _endDate, _filterSelected, _categoryId, _sortByCategory, _sortByMonth, false);
            ComboBoxFilter_Checked.IsChecked = false;
        }

        private void ContextMenu_EditExpense_Click(object sender, RoutedEventArgs e)
        {
            var selected = DG_BudgetItems.SelectedItem as BudgetItem;
            int selectedIndex = DG_BudgetItems.SelectedIndex;
            if (selected != null)
            {
                _mainWindow.EditExpenseMenuWindow = new EditExpense(_presenter, selected);
                _presenter.SetCategories();
                _mainWindow.EditExpenseMenuWindow.cmb_Categories.SelectedIndex = selected.CategoryID - 1;
                _mainWindow.EditExpenseMenuWindow.ShowDialog();
                _presenter.SetBudgetItems(_startDate, _endDate, _filterSelected, _categoryId, _sortByCategory, _sortByMonth, false);
                ComboBoxFilter_Checked.IsChecked = false;
            }



            DG_BudgetItems.SelectedItem = DG_BudgetItems.Items[selectedIndex];



            _mainWindow.NextSearch(DG_BudgetItems.SelectedItem);
        }

        private void CM_DeleteExpense_Click(object sender, RoutedEventArgs e)
        {
            var selected = DG_BudgetItems.SelectedItem as BudgetItem;
            int selectedIndex = DG_BudgetItems.SelectedIndex;

            if (selected != null)
            {
                _presenter.DeleteExpense(selected.ExpenseID);
                _presenter.SetBudgetItems(_startDate, _endDate, _filterSelected, _categoryId, _sortByCategory, _sortByMonth, false);
            }

            if (selectedIndex < DG_BudgetItems.Items.Count)
            {
                DG_BudgetItems.SelectedItem = DG_BudgetItems.Items[selectedIndex];
            }
            else
            {
                DG_BudgetItems.SelectedItem = DG_BudgetItems.Items[DG_BudgetItems.Items.Count - 1];
            }

            _mainWindow.NextSearch(DG_BudgetItems.SelectedItem);
        }

        private void TogglePieChartCheckBox()
        {
            if (_sortByMonth == true && _sortByCategory == true)
                PieChart.IsEnabled = true;
            else
            {
                PieChart.IsEnabled = false;
                PieChart.IsChecked = false;
            }
        }

        private void PieChart_Unchecked(object sender, RoutedEventArgs e)
        {
            PieSeriesChart.Visibility = Visibility.Collapsed;
            DG_Border.Visibility = Visibility.Visible;
            _presenter.SetBudgetItems(_startDate, _endDate, _filterSelected, _categoryId, _sortByCategory, _sortByMonth, false);
        }

        private void PieChart_Checked(object sender, RoutedEventArgs e)
        {
            PieSeriesChart.Visibility = Visibility.Visible;
            DG_Border.Visibility = Visibility.Collapsed;
            _presenter.SetBudgetItems(_startDate, _endDate, _filterSelected, _categoryId, _sortByCategory, _sortByMonth, true);
        }

        private void cbMonths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _presenter.SetBudgetItems(_startDate, _endDate, _filterSelected, _categoryId, _sortByCategory, _sortByMonth, true);
        }


        private void Btn_Search_Clicked(object sender, RoutedEventArgs e)
        {
            string searchedInput = tb_search.Text;

            List<BudgetItem>? budgetItems = DG_BudgetItems.ItemsSource as List<BudgetItem>;

            int totalRowIndex = DG_BudgetItems.Items.Count;

            int selectedIndex = DG_BudgetItems.SelectedIndex;


            if (budgetItems != null )
            {
                _presenter.Search(searchedInput, budgetItems, selectedIndex);
            }

        }

    }
}
