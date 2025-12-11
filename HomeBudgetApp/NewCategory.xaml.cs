using HomeBudgetAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HomeBudgetApp
{
    /// <summary>
    /// Interaction logic for NewCategory.xaml
    /// </summary>
    public partial class NewCategory : Window
    {
        private Presenter _presenter;

        public NewCategory(Presenter presenter)
        {
            InitializeComponent();
            ComboBox_CategoryType.ItemsSource = Enum.GetValues(typeof(Category.CategoryType));
            _presenter = presenter;

            ComboBox_Themes.ItemsSource = Enum.GetValues(typeof(ThemeOptions));
            ComboBox_Themes.SelectedIndex = (int)Theme.CurrentTheme;
        }

        private void CreateCategory_Click(object sender, RoutedEventArgs e)
        {
            Category.CategoryType? categoryType = ComboBox_CategoryType?.SelectedItem as Category.CategoryType?;

            _presenter.AddCategory(TextBox_CategoryName?.Text, categoryType);
            Clear();
        }

        private void Clear()
        {
            TextBox_CategoryName.Clear();
            ComboBox_CategoryType.SelectedItem = null;
        }

        private void Btn_Cancel_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void ComboBox_Themes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Theme.ChangeTheme((ThemeOptions)ComboBox_Themes.SelectedIndex);
        }


    }
}
