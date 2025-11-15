using DataMangment; // Using your namespace
using DataMangment.Datas;
using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

// Namespace matches your file
namespace E_sport_application
{
    /// <summary>
    /// Interaction logic for Events.xaml
    /// </summary>
    public partial class Events : UserControl
    {
        // This holds the reference to the DataAdapter created in MainWindow
        private readonly DataAdapter _adapter;

        public Events(DataAdapter adapter)
        {
            InitializeComponent();
            _adapter = adapter;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadEvents();
        }

        private void LoadEvents()
        {
            try
            {
                dgEvents.ItemsSource = _adapter.GetAllevents_info();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading events: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            txtEventId.Text = "";
            txtEventName.Text = "";
            txtEventLocation.Text = "";
            dpEventDate.SelectedDate = null;
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        private void DgEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgEvents.SelectedItem is events_info selectedEvent)
            {
                txtEventId.Text = selectedEvent.EventID.ToString();
                txtEventName.Text = selectedEvent.EventName;
                txtEventLocation.Text = selectedEvent.EventLocation;
                dpEventDate.SelectedDate = selectedEvent.EventDate;

                btnUpdate.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
        }

        private void BtnAddNew_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEventName.Text))
            {
                MessageBox.Show("Event Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEventLocation.Text))
            {
                MessageBox.Show("Event Location is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!dpEventDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select an event date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newEvent = new events_info
                {
                    EventName = txtEventName.Text,
                    EventLocation = txtEventLocation.Text,
                    EventDate = dpEventDate.SelectedDate.Value
                };

                _adapter.SaveNewevents_info(newEvent); // Using your method name
                LoadEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding event: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgEvents.SelectedItem is events_info selectedEvent)
            {
                if (string.IsNullOrWhiteSpace(txtEventName.Text))
                {
                    MessageBox.Show("Event Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtEventLocation.Text))
                {
                    MessageBox.Show("Event Location is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!dpEventDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("Please select an event date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    selectedEvent.EventName = txtEventName.Text;
                    selectedEvent.EventLocation = txtEventLocation.Text;
                    selectedEvent.EventDate = dpEventDate.SelectedDate.Value;

                    _adapter.Updateevents_info(selectedEvent); // Using new method
                    LoadEvents();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating event: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgEvents.SelectedItem is events_info selectedEvent)
            {
                if (MessageBox.Show($"Are you sure you want to delete {selectedEvent.EventName}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _adapter.Deleteevents_info(selectedEvent.EventID); // Using new method
                        LoadEvents();
                    }
                    catch (Exception ex)
                    {
                        if (ex is SqlException se && se.Number == 547)
                        {
                            MessageBox.Show(
                                "Cannot delete this event because there are match results linked to it.\n\n" +
                                "What to do: Open Reports → choose 'Team Results', filter by this event, " +
                                "delete those matches (or update them), then try deleting the event again.",
                                "Delete blocked by dependencies", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            MessageBox.Show($"Error deleting event: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }
    }
}