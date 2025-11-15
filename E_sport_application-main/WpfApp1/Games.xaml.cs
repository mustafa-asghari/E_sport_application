using DataMangment; // Using your namespace
using DataMangment.Datas;
using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

// Namespace matches your file
namespace E_sport_application
{
    public partial class Games_info : UserControl
    {
        private readonly DataAdapter _adapter;

        public Games_info(DataAdapter adapter)
        {
            InitializeComponent();
            _adapter = adapter;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadGames();
        }

        private void LoadGames()
        {
            try
            {
                dgGames.ItemsSource = _adapter.GetAllgames_info();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading games: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            txtGameId.Text = "";
            txtGameName.Text = "";
            cmbGameType.SelectedItem = null;
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        private void DgGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgGames.SelectedItem is games_info selectedGame)
            {
                txtGameId.Text = selectedGame.GameID.ToString();
                txtGameName.Text = selectedGame.GameName;
                cmbGameType.SelectedItem = selectedGame.GameType;

                btnUpdate.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
        }

        private void BtnAddNew_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtGameName.Text))
            {
                MessageBox.Show("Game Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbGameType.SelectedItem == null)
            {
                MessageBox.Show("Please select a game type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newGame = new games_info
                {
                    GameName = txtGameName.Text,
                    GameType = cmbGameType.SelectedItem.ToString()
                };

                _adapter.AddNewgames_info(newGame);
                LoadGames();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding game: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgGames.SelectedItem is games_info selectedGame)
            {
                if (string.IsNullOrWhiteSpace(txtGameName.Text))
                {
                    MessageBox.Show("Game Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cmbGameType.SelectedItem == null)
                {
                    MessageBox.Show("Please select a game type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    selectedGame.GameName = txtGameName.Text;
                    selectedGame.GameType = cmbGameType.SelectedItem.ToString();

                    _adapter.Updategames_info(selectedGame);
                    LoadGames();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating game: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgGames.SelectedItem is games_info selectedGame)
            {
                if (MessageBox.Show($"Are you sure you want to delete {selectedGame.GameName}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _adapter.Deletegames_info(selectedGame.GameID);
                        LoadGames();
                    }
                    catch (Exception ex)
                    {
                        if (ex is SqlException se && se.Number == 547)
                        {
                            MessageBox.Show(
                                "Cannot delete this game because there are match results linked to it.\n\n" +
                                "What to do: Open Reports → choose 'Team Results', filter by this game, " +
                                "delete those matches (or update them), then try deleting the game again.",
                                "Delete blocked by dependencies", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            MessageBox.Show($"Error deleting game: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
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