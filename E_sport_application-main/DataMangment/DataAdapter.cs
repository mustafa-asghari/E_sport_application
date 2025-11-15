using Dapper;
using DataMangment.Datas; // Using your namespace
using System;
using System.Collections.Generic;
using System.Data.SqlClient; // Can use this or Microsoft.Data.SqlClient
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMangment
{
    public class DataAdapter
    {
        private readonly string _connectionStringName = "DefaultConnection";

        #region teams_info (Team Table)
        /// <summary>
        /// Gets all teams from the 'Team' table, mapping them to the 'teams_info' class.
        /// </summary>
        public List<teams_info> GetAllteams_info()
        {
            // Uses SQL aliases (e.g., Team_Name AS TeamName) to match your class properties.
            string query = @"SELECT Team_id, Team_Name AS TeamName, primary_contact AS PrimaryContact, 
                                    ContactPhone, ContactEmail, CompetitionPoints 
                             FROM Team ORDER BY TeamName";

            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                return connection.Query<teams_info>(query).ToList();
            }
        }

        /// <summary>
        /// Gets a single team by its ID.
        /// </summary>
        public teams_info Getteams_infoById(int id)
        {
            string query = @"SELECT Team_id, Team_Name AS TeamName, primary_contact AS PrimaryContact, 
                                    ContactPhone, ContactEmail, CompetitionPoints 
                             FROM Team WHERE Team_id = @Id";

            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                return connection.QuerySingle<teams_info>(query, new { Id = id });
            }
        }

        /// <summary>
        /// Adds a new team. CompetitionPoints defaults to 0.
        /// </summary>
        public void AddNewteams_info(teams_info teams_infoEntry)
        {
            // Maps class properties (@TeamName) to SQL columns (Team_Name).
            string query = @"INSERT INTO Team (Team_Name, primary_contact, ContactPhone, ContactEmail, CompetitionPoints) 
                             VALUES (@TeamName, @PrimaryContact, @ContactPhone, @ContactEmail, 0)";

            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                connection.Execute(query, teams_infoEntry);
            }
        }

        /// <summary>
        /// Updates an existing team's contact details.
        /// </summary>
        public void Updateteams_info(teams_info teams_infoEntry)
        {
            string query = @"UPDATE Team 
                             SET Team_Name = @TeamName, 
                                 primary_contact = @PrimaryContact, 
                                 ContactPhone = @ContactPhone, 
                                 ContactEmail = @ContactEmail
                             WHERE Team_id = @Team_id";

            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                connection.Execute(query, teams_infoEntry);
            }
        }

        /// <summary>
        /// Deletes a team.
        /// </summary>
        public void Deleteteams_info(int id)
        {
            string query = "DELETE FROM Team WHERE Team_id = @Id";

            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                connection.Execute(query, new { Id = id });
            }
        }
        #endregion

        #region events_info (Events Table)
        /// <summary>
        /// Gets all events.
        /// </summary>
        public List<events_info> GetAllevents_info()
        {
            // Your class and table columns match, so no aliases needed.
            string query = "SELECT * FROM Events ORDER BY EventDate DESC";

            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                return connection.Query<events_info>(query).ToList();
            }
        }

        /// <summary>
        /// Adds a new event.
        /// </summary>
        public void SaveNewevents_info(events_info currentevents_info)
        {
            string query = @"INSERT INTO Events (EventName, EventLocation, EventDate) 
                             VALUES (@EventName, @EventLocation, @EventDate)";

            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                connection.Execute(query, currentevents_info);
            }
        }

        /// <summary>
        /// Updates an existing event.
        /// </summary>
        public void Updateevents_info(events_info eventEntry)
        {
            string query = @"UPDATE Events 
                             SET EventName = @EventName, 
                                 EventLocation = @EventLocation, 
                                 EventDate = @EventDate
                             WHERE EventID = @EventID";

            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                connection.Execute(query, eventEntry);
            }
        }

        /// <summary>
        /// Deletes an event.
        /// </summary>
        public void Deleteevents_info(int id)
        {
            string query = "DELETE FROM Events WHERE EventID = @Id";

            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                connection.Execute(query, new { Id = id });
            }
        }
        #endregion

        #region games_info (Games Table)
        /// <summary>
        /// Gets all games. (This was missing from your barebone).
        /// </summary>
        public List<games_info> GetAllgames_info()
        {
            string query = "SELECT * FROM Games ORDER BY GameName";

            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                return connection.Query<games_info>(query).ToList();
            }
        }

        /// <summary>
        /// Adds a new game. (This was missing).
        /// </summary>
        public void AddNewgames_info(games_info gameEntry)
        {
            string query = @"INSERT INTO Games (GameName, GameType) 
                             VALUES (@GameName, @GameType)";

            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                connection.Execute(query, gameEntry);
            }
        }

        /// <summary>
        /// Updates an existing game. (This was missing).
        /// </summary>
        public void Updategames_info(games_info gameEntry)
        {
            string query = @"UPDATE Games 
                             SET GameName = @GameName, 
                                 GameType = @GameType
                             WHERE GameID = @GameID";

            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                connection.Execute(query, gameEntry);
            }
        }

        /// <summary>
        /// Deletes a game. (This was missing).
        /// </summary>
        public void Deletegames_info(int id)
        {
            string query = "DELETE FROM Games WHERE GameID = @Id";

            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                connection.Execute(query, new { Id = id });
            }
        }
        #endregion

        #region result_info (TeamResults Table) & Business Logic
        /// <summary>
        /// Gets all raw results.
        /// </summary>
        public List<result_info> GetAllresult_info()
        {
            string query = "SELECT * FROM TeamResults";

            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                return connection.Query<result_info>(query).ToList();
            }
        }

        /// <summary>
        /// Deletes a result.
        /// </summary>
        public void delete_result(int ResultID)
        {
            string query = "DELETE FROM TeamResults WHERE ResultID = @ResultID";

            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                connection.Execute(query, new { ResultID });
            }
        }

        /// <summary>
        /// This is the main business logic method from the requirements.
        /// It adds results for BOTH teams and updates their points inside a transaction.
        /// </summary>
        public void AddMatchResult(int eventId, int gameId, int team1Id, int team2Id, string team1Result)
        {
            string team2Result = "";
            int team1Points = 0;
            int team2Points = 0;

            // Determine opposing result and points
            if (team1Result == "Win")
            {
                team2Result = "Loss";
                team1Points = 2;
                team2Points = 0;
            }
            else if (team1Result == "Loss")
            {
                team2Result = "Win";
                team1Points = 0;
                team2Points = 2;
            }
            else // Draw
            {
                team2Result = "Draw";
                team1Points = 1;
                team2Points = 1;
            }

            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert Result for Team 1
                        string query1 = @"INSERT INTO TeamResults (EventID, GameID, TeamID, OpposingTeamID, Result) 
                                          VALUES (@EventID, @GameID, @TeamID, @OpposingTeamID, @Result)";
                        connection.Execute(query1, new { EventID = eventId, GameID = gameId, TeamID = team1Id, OpposingTeamID = team2Id, Result = team1Result }, transaction);

                        // 2. Insert Result for Team 2 (note the swapped TeamID and OpposingTeamID)
                        string query2 = @"INSERT INTO TeamResults (EventID, GameID, TeamID, OpposingTeamID, Result) 
                                          VALUES (@EventID, @GameID, @TeamID, @OpposingTeamID, @Result)";
                        connection.Execute(query2, new { EventID = eventId, GameID = gameId, TeamID = team2Id, OpposingTeamID = team1Id, Result = team2Result }, transaction);

                        // 3. Update Team 1 Points
                        if (team1Points > 0)
                        {
                            string query3 = "UPDATE Team SET CompetitionPoints = CompetitionPoints + @Points WHERE Team_id = @Id";
                            connection.Execute(query3, new { Points = team1Points, Id = team1Id }, transaction);
                        }

                        // 4. Update Team 2 Points
                        if (team2Points > 0)
                        {
                            string query4 = "UPDATE Team SET CompetitionPoints = CompetitionPoints + @Points WHERE Team_id = @Id";
                            connection.Execute(query4, new { Points = team2Points, Id = team2Id }, transaction);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw; // Re-throw the exception to notify the UI
                    }
                }
            }
        }
        #endregion

        #region Report Queries (New)
        /// <summary>
        /// Gets team details, ordered by points.
        /// </summary>
        public List<teams_info> GetTeamDetailsReport()
        {
            string query = @"SELECT Team_id, Team_Name AS TeamName, primary_contact AS PrimaryContact, 
                                    ContactPhone, ContactEmail, CompetitionPoints 
                             FROM Team 
                             ORDER BY CompetitionPoints DESC";
            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                return connection.Query<teams_info>(query).ToList();
            }
        }

        private string GetBaseResultsReportQuery()
        {
            // This complex query joins all tables to get the names needed for the report.
            return @"SELECT 
                        tr.ResultID,
                        tr.EventID,
                        tr.GameID,
                        tr.TeamID,
                        tr.OpposingTeamID,
                        e.EventName, e.EventDate,
                        g.GameName,
                        t1.Team_Name AS TeamName,
                        t2.Team_Name AS OpposingTeamName,
                        tr.Result
                     FROM TeamResults tr
                     JOIN Events e ON tr.EventID = e.EventID
                     JOIN Games g ON tr.GameID = g.GameID
                     JOIN Team t1 ON tr.TeamID = t1.Team_id
                     JOIN Team t2 ON tr.OpposingTeamID = t2.Team_id";
        }

        /// <summary>
        /// Gets results ordered by event.
        /// </summary>
        public List<TeamResultReport> GetTeamResultsReportByEvent()
        {
            string query = GetBaseResultsReportQuery() + " ORDER BY e.EventName, TeamName";
            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                return connection.Query<TeamResultReport>(query).ToList();
            }
        }

        /// <summary>
        /// Gets results ordered by team.
        /// </summary>
        public List<TeamResultReport> GetTeamResultsReportByTeam()
        {
            string query = GetBaseResultsReportQuery() + " ORDER BY TeamName, e.EventName";
            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                return connection.Query<TeamResultReport>(query).ToList();
            }
        }

        /// <summary>
        /// Updates a match result (both sides) and adjusts team points inside a transaction.
        /// </summary>
        public void UpdateMatchResult(int resultId, string newResult)
        {
            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var existing = connection.QuerySingle<result_info>(
                            "SELECT TOP 1 * FROM TeamResults WHERE ResultID = @ResultID",
                            new { ResultID = resultId }, tx);

                        var opposingId = connection.ExecuteScalar<int?>(
                            @"SELECT TOP 1 ResultID FROM TeamResults
                              WHERE EventID = @EventID AND GameID = @GameID
                                AND TeamID = @OpposingTeamID AND OpposingTeamID = @TeamID",
                            new { existing.EventID, existing.GameID, existing.TeamID, existing.OpposingTeamID }, tx);

                        int OldPoints(string r) => r == "Win" ? 2 : (r == "Draw" ? 1 : 0);
                        var oldTeam1Pts = OldPoints(existing.Result);
                        var oldTeam2Pts = OldPoints(existing.Result == "Win" ? "Loss" : existing.Result == "Loss" ? "Win" : "Draw");

                        string oppNewResult = newResult == "Win" ? "Loss" : newResult == "Loss" ? "Win" : "Draw";
                        int NewPoints(string r) => r == "Win" ? 2 : (r == "Draw" ? 1 : 0);
                        var newTeam1Pts = NewPoints(newResult);
                        var newTeam2Pts = NewPoints(oppNewResult);

                        connection.Execute("UPDATE TeamResults SET Result = @R WHERE ResultID = @Id", new { R = newResult, Id = resultId }, tx);
                        if (opposingId.HasValue)
                        {
                            connection.Execute("UPDATE TeamResults SET Result = @R WHERE ResultID = @Id", new { R = oppNewResult, Id = opposingId.Value }, tx);
                        }

                        int deltaT1 = newTeam1Pts - oldTeam1Pts;
                        int deltaT2 = newTeam2Pts - oldTeam2Pts;
                        if (deltaT1 != 0)
                        {
                            connection.Execute("UPDATE Team SET CompetitionPoints = CompetitionPoints + @P WHERE Team_id = @Id",
                                new { P = deltaT1, Id = existing.TeamID }, tx);
                        }
                        if (deltaT2 != 0)
                        {
                            connection.Execute("UPDATE Team SET CompetitionPoints = CompetitionPoints + @P WHERE Team_id = @Id",
                                new { P = deltaT2, Id = existing.OpposingTeamID }, tx);
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Deletes both sides of a match result and reverts team points inside a transaction.
        /// </summary>
        public void DeleteMatchResultCascade(int resultId)
        {
            using (var connection = Helper.CreateSQLServerConnection(_connectionStringName))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var existing = connection.QuerySingle<result_info>(
                            "SELECT TOP 1 * FROM TeamResults WHERE ResultID = @ResultID",
                            new { ResultID = resultId }, tx);

                        var opposingId = connection.ExecuteScalar<int?>(
                            @"SELECT TOP 1 ResultID FROM TeamResults
                              WHERE EventID = @EventID AND GameID = @GameID
                                AND TeamID = @OpposingTeamID AND OpposingTeamID = @TeamID",
                            new { existing.EventID, existing.GameID, existing.TeamID, existing.OpposingTeamID }, tx);

                        int Points(string r) => r == "Win" ? 2 : (r == "Draw" ? 1 : 0);
                        var t1Pts = Points(existing.Result);
                        var t2Pts = Points(existing.Result == "Win" ? "Loss" : existing.Result == "Loss" ? "Win" : "Draw");

                        if (t1Pts != 0)
                        {
                            connection.Execute("UPDATE Team SET CompetitionPoints = CompetitionPoints - @P WHERE Team_id = @Id",
                                new { P = t1Pts, Id = existing.TeamID }, tx);
                        }
                        if (t2Pts != 0)
                        {
                            connection.Execute("UPDATE Team SET CompetitionPoints = CompetitionPoints - @P WHERE Team_id = @Id",
                                new { P = t2Pts, Id = existing.OpposingTeamID }, tx);
                        }

                        connection.Execute("DELETE FROM TeamResults WHERE ResultID = @Id", new { Id = resultId }, tx);
                        if (opposingId.HasValue)
                        {
                            connection.Execute("DELETE FROM TeamResults WHERE ResultID = @Id", new { Id = opposingId.Value }, tx);
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
        #endregion
    }
}



