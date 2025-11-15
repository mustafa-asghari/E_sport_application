using DataMangment; // Using your namespace
using DataMangment.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

// Namespace matches your file
namespace E_sport_application
{
    /// <summary>
    /// Interaction logic for Result.xaml
    /// </summary>
    public partial class Result : UserControl
    {
        private readonly DataAdapter _adapter;
        private List<teams_info> _allTeams = new();

        public Result(DataAdapter adapter)
        {
            InitializeComponent();
            _adapter = adapter;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            try
            {
                // Load all data once
                _allTeams = _adapter.GetAllteams_info();
                var events = _adapter.GetAllevents_info();
                var games = _adapter.GetAllgames_info();

                // Bind to combo boxes
                cmbEvent.ItemsSource = events;
                cmbGame.ItemsSource = games;

                // Use separate lists for Team 1 and Team 2 to avoid selection conflicts
                cmbTeam1.ItemsSource = _allTeams;
                cmbTeam2.ItemsSource = new List<teams_info>(_allTeams); // Create a copy
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data for form: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CmbTeam1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_allTeams == null || _allTeams.Count == 0)
                return;

            var selectedTeam1 = cmbTeam1.SelectedItem as teams_info;

            if (selectedTeam1 == null)
            {
                // Reset Team 2 to full list if Team 1 is cleared
                cmbTeam2.ItemsSource = new List<teams_info>(_allTeams);
                return;
            }

            // Filter Team 2 list to exclude the selected Team 1
            var filtered = _allTeams.Where(t => t.Team_id != selectedTeam1.Team_id).ToList();
            cmbTeam2.ItemsSource = filtered;

            // If the current Team 2 selection equals Team 1, clear it
            if (cmbTeam2.SelectedItem is teams_info team2 && team2.Team_id == selectedTeam1.Team_id)
            {
                cmbTeam2.SelectedItem = null;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // --- 1. Validation ---
            if (cmbEvent.SelectedItem == null || cmbGame.SelectedItem == null ||
                cmbTeam1.SelectedItem == null || cmbTeam2.SelectedItem == null)
            {
                MessageBox.Show("Please select an event, game, and two teams.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int eventId = (int)cmbEvent.SelectedValue!;
            int gameId = (int)cmbGame.SelectedValue!;
            int team1Id = (int)cmbTeam1.SelectedValue!;
            int team2Id = (int)cmbTeam2.SelectedValue!;

            if (team1Id == team2Id)
            {
                MessageBox.Show("A team cannot play against itself. Please select two different teams.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string team1Result = "";
            if (rbTeam1Wins.IsChecked == true)
                team1Result = "Win";
            else if (rbTeam2Wins.IsChecked == true)
                team1Result = "Loss";
            else if (rbDraw.IsChecked == true)
                team1Result = "Draw";

            if (string.IsNullOrEmpty(team1Result))
            {
                MessageBox.Show("Please select a result for the match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // --- 2. Execute Logic ---
            try
            {
                // Call the business logic method
                _adapter.AddMatchResult(eventId, gameId, team1Id, team2Id, team1Result);

                MessageBox.Show("Match result saved successfully! Competition points have been updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // --- 3. Clear Form ---
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving match result: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            cmbEvent.SelectedItem = null;
            cmbGame.SelectedItem = null;
            cmbTeam1.SelectedItem = null;
            cmbTeam2.SelectedItem = null;
            rbDraw.IsChecked = true;
        }
    }
}