using DataMangment; // Using your namespace
using DataMangment.Datas;
using System;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

using System.Windows;
using System.Windows.Controls;

// Namespace matches your file
namespace E_sport_application
{
    /// <summary>
    /// Interaction logic for Teams.xaml
    /// </summary>
    public partial class Teams_info : UserControl
    {
        private readonly DataAdapter _adapter;

        public Teams_info(DataAdapter adapter)
        {
            InitializeComponent();
            _adapter = adapter;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTeams();
        }

        private void LoadTeams()
        {
            try
            {
                dgTeams.ItemsSource = _adapter.GetAllteams_info(); // Using your method name
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teams: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            txtTeamId.Text = "";
            txtTeamName.Text = "";
            txtPrimaryContact.Text = "";
            txtContactPhone.Text = "";
            txtContactEmail.Text = "";
            txtCompetitionPoints.Text = "";
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        private void DgTeams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTeams.SelectedItem is teams_info selectedTeam)
            {
                // Manually set textboxes as properties match
                txtTeamId.Text = selectedTeam.Team_id.ToString();
                txtTeamName.Text = selectedTeam.TeamName;
                txtPrimaryContact.Text = selectedTeam.PrimaryContact;
                txtContactPhone.Text = selectedTeam.ContactPhone;
                txtContactEmail.Text = selectedTeam.ContactEmail;
                txtCompetitionPoints.Text = selectedTeam.CompetitionPoints.ToString();

                btnUpdate.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
        }

        private void BtnAddNew_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTeamName.Text))
            {
                MessageBox.Show("Team Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPrimaryContact.Text))
            {
                MessageBox.Show("Primary Contact is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtContactPhone.Text))
            {
                MessageBox.Show("Contact Phone is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var phone = txtContactPhone.Text.Trim();
            if (!Regex.IsMatch(phone, @"^[0-9+\-()\s]{7,}$"))
            {
                MessageBox.Show("Enter a valid phone number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtContactEmail.Text))
            {
                MessageBox.Show("Contact Email is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var email = txtContactEmail.Text.Trim();
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newTeam = new teams_info
                {
                    TeamName = txtTeamName.Text,
                    PrimaryContact = txtPrimaryContact.Text,
                    ContactPhone = txtContactPhone.Text,
                    ContactEmail = txtContactEmail.Text
                    // CompetitionPoints is set to 0 by the DataAdapter
                };

                _adapter.AddNewteams_info(newTeam); // Using your method name
                LoadTeams(); // Reload data
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding team: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgTeams.SelectedItem is teams_info selectedTeam)
            {
                if (string.IsNullOrWhiteSpace(txtTeamName.Text))
                {
                    MessageBox.Show("Team Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPrimaryContact.Text))
                {
                    MessageBox.Show("Primary Contact is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtContactPhone.Text))
                {
                    MessageBox.Show("Contact Phone is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var phone = txtContactPhone.Text.Trim();
                if (!Regex.IsMatch(phone, @"^[0-9+\-()\s]{7,}$"))
                {
                    MessageBox.Show("Enter a valid phone number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtContactEmail.Text))
                {
                    MessageBox.Show("Contact Email is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var email = txtContactEmail.Text.Trim();
                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    // Update the selected team object with form values
                    selectedTeam.TeamName = txtTeamName.Text;
                    selectedTeam.PrimaryContact = txtPrimaryContact.Text;
                    selectedTeam.ContactPhone = txtContactPhone.Text;
                    selectedTeam.ContactEmail = txtContactEmail.Text;

                    _adapter.Updateteams_info(selectedTeam); // Using your method name
                    LoadTeams(); // Reload data
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating team: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgTeams.SelectedItem is teams_info selectedTeam)
            {
                if (MessageBox.Show($"Are you sure you want to delete {selectedTeam.TeamName}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _adapter.Deleteteams_info(selectedTeam.Team_id); // Using your method name
                        LoadTeams(); // Reload data
                    }
                    catch (Exception ex)
                    {
                        if (ex is SqlException se && se.Number == 547)
                        {
                            MessageBox.Show(
                                "Cannot delete this team because it is referenced by match results.\n\n" +
                                "What to do: Open Reports → choose 'Team Results', find matches that include this team, " +
                                "delete those matches (or update them), then try deleting the team again.",
                                "Delete blocked by dependencies", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            MessageBox.Show($"Error deleting team: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
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