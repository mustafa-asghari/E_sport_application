using DataMangment; // Using your namespace
using DataMangment.Datas;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

// Namespace matches your file
namespace E_sport_application
{
    /// <summary>
    /// A new UserControl to handle the reporting requirements.
    /// </summary>
    public partial class ReportsView : UserControl
    {
        private readonly DataAdapter _adapter;
        private IList? _fullReportData; // Holds the raw, unfiltered data
        private TeamResultReport? _selectedResult; // currently selected result row (if applicable)

        public ReportsView(DataAdapter adapter)
        {
            InitializeComponent();
            _adapter = adapter;
            cmbReportType.SelectedIndex = 0; // Default selection
        }

        private void DgReport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgReport.SelectedItem is TeamResultReport r)
            {
                _selectedResult = r;
                lblSelectedSummary.Text = $"Event: {r.EventName} | Game: {r.GameName} | {r.TeamName} vs {r.OpposingTeamName}";
                rbEditTeam1Win.IsChecked = r.Result == "Win";
                rbEditTeam2Win.IsChecked = r.Result == "Loss";
                rbEditDraw.IsChecked = r.Result == "Draw";
            }
            else
            {
                _selectedResult = null;
                lblSelectedSummary.Text = "";
                rbEditTeam1Win.IsChecked = false;
                rbEditTeam2Win.IsChecked = false;
                rbEditDraw.IsChecked = false;
            }
        }

        private void BtnUpdateSelected_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedResult == null)
            {
                MessageBox.Show("Select a result row in the grid first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string newResult = rbEditTeam1Win.IsChecked == true ? "Win" : rbEditTeam2Win.IsChecked == true ? "Loss" : "Draw";
            try
            {
                _adapter.UpdateMatchResult(_selectedResult.ResultID, newResult);
                MessageBox.Show("Result updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnRunReport_Click(sender, e); // refresh
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating result: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedResult == null)
            {
                MessageBox.Show("Select a result row in the grid first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show("Delete the selected match (both sides) and revert points?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }
            try
            {
                _adapter.DeleteMatchResultCascade(_selectedResult.ResultID);
                MessageBox.Show("Result deleted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnRunReport_Click(sender, e); // refresh
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting result: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnRunReport_Click(object sender, RoutedEventArgs e)
        {
            if (cmbReportType.SelectedItem == null)
            {
                MessageBox.Show("Please select a report type.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string reportType = ((ComboBoxItem)cmbReportType.SelectedItem).Content?.ToString() ?? string.Empty;

            try
            {
                // Basic UI reset
                dgReport.ItemsSource = null;
                dgReport.Columns.Clear();
                _selectedResult = null;
                lblSelectedSummary.Text = "";
                rbEditTeam1Win.IsChecked = false;
                rbEditTeam2Win.IsChecked = false;
                rbEditDraw.IsChecked = false;

                btnRunReport.IsEnabled = false;
                Mouse.OverrideCursor = Cursors.Wait;

                IList? data = null;

                // Run the data fetch off the UI thread
                await Task.Run(() =>
                {
                    switch (reportType)
                    {
                        case "Team Details (by Points)":
                            data = _adapter.GetTeamDetailsReport();
                            break;

                        case "Team Results (by Event)":
                            data = _adapter.GetTeamResultsReportByEvent();
                            break;

                        case "Team Results (by Team)":
                            data = _adapter.GetTeamResultsReportByTeam();
                            break;
                    }
                });

                _fullReportData = data;

                // Define columns once per report type on the UI thread
                switch (reportType)
                {
                    case "Team Details (by Points)":
                        SetupTeamDetailsColumns();
                        break;
                    case "Team Results (by Event)":
                    case "Team Results (by Team)":
                        SetupResultReportColumns();
                        break;
                }

                // Apply initial filter
                FilterAndDisplayData();
                btnExport.IsEnabled = (_fullReportData != null && _fullReportData.Count > 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error running report: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                btnExport.IsEnabled = false;
            }
            finally
            {
                Mouse.OverrideCursor = null;
                btnRunReport.IsEnabled = true;
            }
        }

        private void SetupTeamDetailsColumns()
        {
            dgReport.Columns.Clear();
            // Manually define columns for 'teams_info'
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Team ID", Binding = new System.Windows.Data.Binding("Team_id") });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Team Name", Binding = new System.Windows.Data.Binding("TeamName"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Contact", Binding = new System.Windows.Data.Binding("PrimaryContact"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Phone", Binding = new System.Windows.Data.Binding("ContactPhone") });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Email", Binding = new System.Windows.Data.Binding("ContactEmail"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Points", Binding = new System.Windows.Data.Binding("CompetitionPoints") });
        }

        private void SetupResultReportColumns()
        {
            dgReport.Columns.Clear();
            // Manually define columns for 'TeamResultReport'
            dgReport.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("ResultID") });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Event", Binding = new System.Windows.Data.Binding("EventName"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Date", Binding = new System.Windows.Data.Binding("EventDate") { StringFormat = "dd/MM/yyyy" } });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Game", Binding = new System.Windows.Data.Binding("GameName"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Team", Binding = new System.Windows.Data.Binding("TeamName"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Opponent", Binding = new System.Windows.Data.Binding("OpposingTeamName"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Result", Binding = new System.Windows.Data.Binding("Result") });
        }

        private void FilterAndDisplayData()
        {
            if (_fullReportData == null)
            {
                dgReport.ItemsSource = null;
                return;
            }

            string searchTerm = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                dgReport.ItemsSource = _fullReportData; // Show all
                return;
            }

            // Filter logic
            if (_fullReportData is List<teams_info> teamList)
            {
                dgReport.ItemsSource = teamList
                    .Where(t => t.TeamName.ToLower().Contains(searchTerm))
                    .ToList();
            }
            else if (_fullReportData is List<TeamResultReport> resultList)
            {
                dgReport.ItemsSource = resultList
                    .Where(r => r.TeamName.ToLower().Contains(searchTerm) ||
                                r.OpposingTeamName.ToLower().Contains(searchTerm))
                    .ToList();
            }
        }

        private void TxtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            // Filter as user types
            FilterAndDisplayData();
        }

        private void CmbReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear old data when report type changes
            _fullReportData = null;
            dgReport.ItemsSource = null;
            dgReport.Columns.Clear();
            txtSearch.Text = "";
            btnExport.IsEnabled = false;
            _selectedResult = null;
            lblSelectedSummary.Text = "";
            rbEditTeam1Win.IsChecked = false;
            rbEditTeam2Win.IsChecked = false;
            rbEditDraw.IsChecked = false;
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            // Get the data currently in the grid (which is the filtered data)
            var dataToExport = dgReport.ItemsSource as IEnumerable;

            if (dataToExport == null)
            {
                MessageBox.Show("No data to export.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV file (*.csv)|*.csv",
                Title = "Save Report as CSV",
                FileName = $"EsportsReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var sb = new StringBuilder();

                    // Create header row from DataGrid columns (null-safe)
                    var headers = dgReport.Columns.Select(c => {
                        var h = c.Header?.ToString() ?? string.Empty;
                        return $"\"{h.Replace("\"", "\"\"")}\"";
                    });
                    sb.AppendLine(string.Join(",", headers));

                    // Get properties from the items
                    var items = dataToExport.Cast<object>().ToList();
                    if (items.Any())
                    {
                        var itemType = items.First().GetType();

                        // Get property names from bindings
                        var propNames = dgReport.Columns
                            .OfType<DataGridBoundColumn>()
                            .Select(c => (c.Binding as System.Windows.Data.Binding)?.Path.Path)
                            .ToList();

                        foreach (var item in items)
                        {
                            var values = propNames.Select(propName =>
                            {
                                if (propName == null) return "";
                                var propInfo = itemType.GetProperty(propName);
                                var value = propInfo?.GetValue(item)?.ToString() ?? "";
                                // Escape for CSV
                                return $"\"{value.Replace("\"", "\"\"")}\"";
                            });
                            sb.AppendLine(string.Join(",", values));
                        }
                    }

                    File.WriteAllText(saveFileDialog.FileName, sb.ToString());
                    MessageBox.Show($"Report exported successfully to:\n{saveFileDialog.FileName}", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting to CSV: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}