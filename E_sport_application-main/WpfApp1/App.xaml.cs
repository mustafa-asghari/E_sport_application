using System.Configuration;
using System.Data;
using System.Windows;
using DataMangment;

namespace E_sport_application
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                Helper.EnsureDatabaseAndSchema("DefaultConnection");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization failed: {ex.Message}", "Database Initialization", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
