using System.DirectoryServices.ActiveDirectory;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataMangment;

namespace E_sport_application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DataAdapter _adapter;

        public MainWindow()
        {
            InitializeComponent();
            // Ensure a usable database exists; on Windows, will auto-create LocalDB if needed.
            if (Helper.EnsureLocalDatabase(out var _, out bool created))
            {
                if (created)
                {
                    MessageBox.Show("Creating the database, please wait...", "Initializing", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Opening the application...", "Starting", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            _adapter = new DataAdapter();
            conMain.Content = new Teams_info(_adapter);
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            conMain.Content = new Teams_info(_adapter);
        }

        private void btn_Evenets(object sender, RoutedEventArgs e)
        {
            conMain.Content = new Events(_adapter);
        }

        private void Btn_result(object sender, RoutedEventArgs e)
        {
            conMain.Content = new Result(_adapter);
        }

        private void Btn_games(object sender, RoutedEventArgs e)
        {
            conMain.Content = new Games_info(_adapter);
        }

        private void Btn_reports_Click(object sender, RoutedEventArgs e)
        {
            conMain.Content = new ReportsView(_adapter);
        }

        private void btnExpenses_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnCategories_Click(object sender, RoutedEventArgs e)
        {
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}