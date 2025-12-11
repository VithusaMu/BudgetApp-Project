using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
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
using System;
using System.IO;
using HomeBudgetAPI;

namespace HomeBudgetApp
{
    /// <summary>
    /// Interaction logic for Entry.xaml
    /// </summary>
    public partial class Entry : Window
    {
        private Presenter _presenter;
        private bool _isSelected;

        public Entry(Presenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            ComboBox_Themes.ItemsSource = Enum.GetValues(typeof(ThemeOptions));
            ComboBox_Themes.SelectedIndex = (int)Theme.CurrentTheme;
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "File Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SQLite Data Base files(*.db *.db3) | *.*";
            if (openFileDialog.ShowDialog() == true)
            {
                // set text box to database name
                txtEditor.Text = openFileDialog.FileName;

                var filePath = openFileDialog.FileName;


                // if file is not a database file, prompt user
                if (filePath.EndsWith(".db"))
                {
       
                    if (!String.IsNullOrEmpty(txtEditor.Text))
                    {
                        try
                        {
                            _presenter.ProcessFile(filePath, false);
                            _isSelected = true;
                        }
                        catch (Exception err)
                        {
                            ShowError(err.Message);
                        }
                    }
                }
                else
                {
                    ShowError("Please use a valid database file");
                }
            }
        }


        private void newDb_Click(object sender, RoutedEventArgs e)
        {
            string fileName = string.Empty;

            if (!_isSelected)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "newDb"; // Default file name
                dlg.DefaultExt = ".db"; // Default file extension
                dlg.Filter = "Database File (.db)|*.db"; // Filter files by extension 

                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    fileName = dlg.FileName;
                    _presenter.ProcessFile(fileName, true);
                }

                selectedNewDbName.Text = dlg.FileName.Trim();
                _isSelected = true;
            }
        }

        private void goMainPage_Click(object sender, RoutedEventArgs e)
        {
       
            if (_isSelected)
            {
                this.Close();
            }
            else 
            {
                ShowError("Please create or choose an existing database file");
            }
        }

        private void ComboBox_Themes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Theme.ChangeTheme((ThemeOptions)ComboBox_Themes.SelectedIndex);
        }

    }
}
