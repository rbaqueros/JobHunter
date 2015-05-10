/*
 * Copyright (c) 2015 Rafael Baquero
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace Job_Hunter.Model
{
    public class DataService : IDataService
    {
        // Event declaration
        public event EventHandler StatusModified;
        public event EventHandler NextActionModified;
        public event EventHandler JobTypeModified;
        public event EventHandler CountryModified;
        public event EventHandler OrganizationModified;
        public event EventHandler ApplicationModified;

        // Table names
        private const string tableActiveCountry = "active_country";
        private const string tableJobType = "job_type";
        private const string tableNextAction = "next_action";
        private const string tableStatus = "status";
        private const string tableOrganization = "organization";
        private const string tableApplication = "application";

        // Database path and connection string
        private readonly string appDataPath;
        private readonly string dbPath;
        private readonly string strConnection;

        public DataService()
        {
            appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Baqsoft", "JobHunter");
            dbPath = Path.Combine(appDataPath, "jobhunter.db");
            strConnection = "Data Source=" + dbPath + ";Version=3;Foreign keys=true";

            CreateDataBase();
        }

        /*
         * 
         *Application handling
         * 
         */
        public void ApplicationList(Action<ApplicationOrganizationItem[]> callback)
        {
            List<ApplicationOrganizationItem> listApplication = new List<ApplicationOrganizationItem>();

            string queryString = "select " + tableApplication + ".*, organization, division from " 
                + tableApplication + " inner join " + tableOrganization + " on " + tableApplication 
                + ".organization_id = " + tableOrganization + ".id order by priority, organization.organization"
                + ", division, title";

            // Get active country list from database.
            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand command = new SQLiteCommand(queryString, connection))
            {
                connection.Open();

                SQLiteDataReader reader = command.ExecuteReader();
                int idColumn = reader.GetOrdinal("id");
                int titleColumn = reader.GetOrdinal("title");
                int jobIdColumn = reader.GetOrdinal("job_id");
                int jobUrlColumn = reader.GetOrdinal("job_url");
                int jobTypeColumn = reader.GetOrdinal("job_type");
                int statusColumn = reader.GetOrdinal("status");
                int priorityColumn = reader.GetOrdinal("priority");
                int nextActionColumn = reader.GetOrdinal("next_action");
                int nextActionDateColumn = reader.GetOrdinal("next_action_date");
                int organizationIdColumn = reader.GetOrdinal("organization_id");
                int organizationColumn = reader.GetOrdinal("organization");
                int divisionColumn = reader.GetOrdinal("division");
                int iso2Column = reader.GetOrdinal("iso2");
                int cityColumn = reader.GetOrdinal("city");
                int contactNameColumn = reader.GetOrdinal("contact_name");
                int contactPhoneColumn = reader.GetOrdinal("contact_phone");
                int contactEmailColumn = reader.GetOrdinal("contact_email");
                int descriptionColumn = reader.GetOrdinal("description");
                int noteColumn = reader.GetOrdinal("note");
                while (reader.Read())
                {
                    long id = reader.GetInt64(idColumn);
                    string title = reader.GetString(titleColumn);
                    string jobId = reader.GetString(jobIdColumn);
                    string jobUrl = reader.GetString(jobUrlColumn);
                    string jobType = reader.GetString(jobTypeColumn);
                    string status = reader.GetString(statusColumn);
                    int priority = reader.GetInt32(priorityColumn);
                    string nextAction = reader.GetString(nextActionColumn);
                    string nextActionDate = reader.GetString(nextActionDateColumn);
                    long organizationId = reader.GetInt64(organizationIdColumn);
                    string organization = reader.GetString(organizationColumn);
                    string division = reader.GetString(divisionColumn);
                    string iso2 = reader.GetString(iso2Column);
                    string city = reader.GetString(cityColumn);
                    string contactName = reader.GetString(contactNameColumn);
                    string contactPhone = reader.GetString(contactPhoneColumn);
                    string contactEmail = reader.GetString(contactEmailColumn);
                    string description = reader.GetString(descriptionColumn);
                    string note = reader.GetString(noteColumn);
                    listApplication.Add(new ApplicationOrganizationItem(id, title, jobId, jobUrl, 
                        jobType, status, priority, nextAction, nextActionDate, organizationId, 
                        organization, division, iso2, city, contactName, contactPhone, contactEmail, 
                        description, note));
                }
            }

            callback(listApplication.ToArray());
        }

        public int AddApplication(ApplicationItem item)
        {
            string queryString = "insert into " + tableApplication + " values(null, @title, @job_id, @job_url, @job_type, @status, @priority, @next_action, @next_action_date, @organization, "
                + "@iso2, @city, @contact_name, @contact_phone, @contact_email, @description, @note)";
            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@title", item.Title);
                dbCommand.Parameters.AddWithValue("@job_id", item.JobId);
                dbCommand.Parameters.AddWithValue("@job_url", item.JobUrl);
                dbCommand.Parameters.AddWithValue("@job_type", item.JobType);
                dbCommand.Parameters.AddWithValue("@status", item.Status);
                dbCommand.Parameters.AddWithValue("@priority", item.Priority);
                dbCommand.Parameters.AddWithValue("@next_action", item.NextAction);
                dbCommand.Parameters.AddWithValue("@next_action_date", item.NextActionDate);
                dbCommand.Parameters.AddWithValue("@organization", item.OrganizationId);
                dbCommand.Parameters.AddWithValue("@iso2", item.Iso2);
                dbCommand.Parameters.AddWithValue("@city", item.City);
                dbCommand.Parameters.AddWithValue("@contact_name", item.ContactName);
                dbCommand.Parameters.AddWithValue("@contact_phone", item.ContactPhone);
                dbCommand.Parameters.AddWithValue("@contact_email", item.ContactEmail);
                dbCommand.Parameters.AddWithValue("@description", item.Description);
                dbCommand.Parameters.AddWithValue("@note", item.Note);

                // Add new organization to table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.ApplicationModified != null)
                {
                    this.ApplicationModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        public int DeleteApplication(ApplicationItem item)
        {
            string queryString = "delete from " + tableApplication + " where id=@id";

            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@id", item.Id);

                // Delete organization from table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.ApplicationModified != null)
                {
                    this.ApplicationModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        public int UpdateApplication(ApplicationItem item)
        {
            string queryString = "update " + tableApplication + " set "
                + "title=@title, "
                + "job_id=@job_id, "
                + "job_url=@job_url, "
                + "job_type=@job_type, "
                + "status=@status, "
                + "priority=@priority, "
                + "next_action=@next_action, "
                + "next_action_date=@next_action_date, "
                + "organization_id=@organization_id, "
                + "iso2=@iso2, "
                + "city=@city, "
                + "contact_name=@contact_name, "
                + "contact_phone=@contact_phone, "
                + "contact_email=@contact_email, "
                + "description=@description, "
                + "note=@note"
                + " where id=@id";

            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@id", item.Id);
                dbCommand.Parameters.AddWithValue("@title", item.Title);
                dbCommand.Parameters.AddWithValue("@job_id", item.JobId);
                dbCommand.Parameters.AddWithValue("@job_url", item.JobUrl);
                dbCommand.Parameters.AddWithValue("@job_type", item.JobType);
                dbCommand.Parameters.AddWithValue("@status", item.Status);
                dbCommand.Parameters.AddWithValue("@priority", item.Priority);
                dbCommand.Parameters.AddWithValue("@next_action", item.NextAction);
                dbCommand.Parameters.AddWithValue("@next_action_date", item.NextActionDate);
                dbCommand.Parameters.AddWithValue("@organization_id", item.OrganizationId);
                dbCommand.Parameters.AddWithValue("@iso2", item.Iso2);
                dbCommand.Parameters.AddWithValue("@city", item.City);
                dbCommand.Parameters.AddWithValue("@contact_name", item.ContactName);
                dbCommand.Parameters.AddWithValue("@contact_phone", item.ContactPhone);
                dbCommand.Parameters.AddWithValue("@contact_email", item.ContactEmail);
                dbCommand.Parameters.AddWithValue("@description", item.Description);
                dbCommand.Parameters.AddWithValue("@note", item.Note);

                // Update organization in table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.ApplicationModified != null)
                {
                    this.ApplicationModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        /*
         * 
         * Organization handling
         * 
         */
        public void OrganizationList(Action<OrganizationItem[]> callback)
        {
            List<OrganizationItem> listOrganization = new List<OrganizationItem>();

            string queryString = "select * from " + tableOrganization + " order by organization";

            // Get active country list from database.
            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand command = new SQLiteCommand(queryString, connection))
            {
                connection.Open();

                SQLiteDataReader reader = command.ExecuteReader();
                int idColumn = reader.GetOrdinal("id");
                int organizationColumn = reader.GetOrdinal("organization");
                int divisionColumn = reader.GetOrdinal("division");
                int iso2Column = reader.GetOrdinal("iso2");
                int cityColumn = reader.GetOrdinal("city");
                int contactNameColumn = reader.GetOrdinal("contact_name");
                int contactPhoneColumn = reader.GetOrdinal("contact_phone");
                int contactEmailColumn = reader.GetOrdinal("contact_email");
                int jobsUrlColumn = reader.GetOrdinal("jobs_url");
                int organizationUrlColumn = reader.GetOrdinal("organization_url");
                int noteColumn = reader.GetOrdinal("note");
                while (reader.Read())
                {
                    long id = reader.GetInt64(idColumn);
                    string organization = reader.GetString(organizationColumn);
                    string division = reader.GetString(divisionColumn);
                    string iso2 = reader.GetString(iso2Column);
                    string city = reader.GetString(cityColumn);
                    string contactName = reader.GetString(contactNameColumn);
                    string contactPhone = reader.GetString(contactPhoneColumn);
                    string contactEmail = reader.GetString(contactEmailColumn);
                    string jobsUrl = reader.GetString(jobsUrlColumn);
                    string url = reader.GetString(organizationUrlColumn);
                    string note = reader.GetString(noteColumn);
                    listOrganization.Add(new OrganizationItem(id, organization, division, iso2, city, contactName, contactPhone, contactEmail, url, jobsUrl, note));
                }
            }

            callback(listOrganization.ToArray());
        }

        public int AddOrganization(OrganizationItem item)
        {
            string queryString = "insert into " + tableOrganization + " values(null, @organization, "
                + "@division, @iso2, @city, @contact_name, @contact_phone, @contact_email, @jobs_url, @url, "
                + "@note)";
            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@organization", item.Organization);
                dbCommand.Parameters.AddWithValue("@division", item.Division);
                dbCommand.Parameters.AddWithValue("@iso2", item.Iso2Code);
                dbCommand.Parameters.AddWithValue("@city", item.City);
                dbCommand.Parameters.AddWithValue("@contact_name", item.ContactName);
                dbCommand.Parameters.AddWithValue("@contact_phone", item.ContactPhone);
                dbCommand.Parameters.AddWithValue("@contact_email", item.ContactEmail);
                dbCommand.Parameters.AddWithValue("@url", item.Url);
                dbCommand.Parameters.AddWithValue("@jobs_url", item.JobsUrl);
                dbCommand.Parameters.AddWithValue("@note", item.Note);

                // Add new organization to table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.OrganizationModified != null)
                {
                    this.OrganizationModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        public int DeleteOrganization(OrganizationItem item)
        {
            string queryString = "delete from " + tableOrganization + " where id=@id";

            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@id", item.Id);

                // Delete organization from table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.OrganizationModified != null)
                {
                    this.OrganizationModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        public int UpdateOrganization(OrganizationItem item)
        {
            string queryString = "update " + tableOrganization + " set " 
                + "organization=@organization, "
                + "division=@division, "
                + "iso2=@iso2, "
                + "city=@city, "
                + "contact_name=@contact_name, "
                + "contact_phone=@contact_phone, "
                + "contact_email=@contact_email, "
                + "organization_url=@url, "
                + "jobs_url=@jobs_url, "
                + "note=@note where id=@id";

            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@id", item.Id);
                dbCommand.Parameters.AddWithValue("@organization", item.Organization);
                dbCommand.Parameters.AddWithValue("@division", item.Division);
                dbCommand.Parameters.AddWithValue("@iso2", item.Iso2Code);
                dbCommand.Parameters.AddWithValue("@city", item.City);
                dbCommand.Parameters.AddWithValue("@contact_name", item.ContactName);
                dbCommand.Parameters.AddWithValue("@contact_phone", item.ContactPhone);
                dbCommand.Parameters.AddWithValue("@contact_email", item.ContactEmail);
                dbCommand.Parameters.AddWithValue("@url", item.Url);
                dbCommand.Parameters.AddWithValue("@jobs_url", item.JobsUrl);
                dbCommand.Parameters.AddWithValue("@note", item.Note);

                // Update organization in table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.OrganizationModified != null)
                {
                    this.OrganizationModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        /*
         * 
         * Active country handling
         * 
         */
        public void ActiveCountryList(Action<ActiveCountryItem[]> callback)
        {
            List<ActiveCountryItem> listNextAction = new List<ActiveCountryItem>();

            string queryString = "select * from " + tableActiveCountry;

            // Get active country list from database.
            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand command = new SQLiteCommand(queryString, connection))
            {
                connection.Open();

                SQLiteDataReader reader = command.ExecuteReader();
                int idColumn = reader.GetOrdinal("id");
                int iso2Column = reader.GetOrdinal("iso2");
                int noteColumn = reader.GetOrdinal("note");
                while (reader.Read())
                {
                    long id = reader.GetInt64(idColumn);
                    string iso2 = reader.GetString(iso2Column);
                    string note = reader.GetString(noteColumn);
                    listNextAction.Add(new ActiveCountryItem(id, iso2, note));
                }
            }

            callback(listNextAction.ToArray());
        }

        public int AddActiveCountry(ActiveCountryItem newItem)
        {
            string queryString = "insert into " + tableActiveCountry + " values(null, @iso2, @note)";
            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@iso2", newItem.Iso2Code);
                dbCommand.Parameters.AddWithValue("@note", newItem.Note);

                // Add new active country to table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.CountryModified != null)
                {
                    this.CountryModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        public int DeleteActiveCountry(ActiveCountryItem newItem)
        {
            string queryString = "delete from " + tableActiveCountry + " where id=@id";

            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@id", newItem.Id);

                // Delete active country from table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.CountryModified != null)
                {
                    this.CountryModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        public int UpdateActiveCountryNotes(ActiveCountryItem newItem)
        {
            string queryString = "update " + tableActiveCountry
                + " set note=@note where id=@id";

            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@id", newItem.Id);
                dbCommand.Parameters.AddWithValue("@note", newItem.Note);

                // Update active ocuntry in table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.CountryModified != null)
                {
                    this.CountryModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        /*
         *
         * Next action handling
         *
         */
        public void NextActionList(Action<NextActionItem[]> callback)
        {
            List<NextActionItem> listNextAction = new List<NextActionItem>();

            string queryString = "select * from " + tableNextAction + " order by position";

            // Get status list from database.
            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand command = new SQLiteCommand(queryString, connection))
            {
                connection.Open();

                SQLiteDataReader reader = command.ExecuteReader();
                int idColumn = reader.GetOrdinal("id");
                int positionColumn = reader.GetOrdinal("position");
                int nextActionColumn = reader.GetOrdinal("next_action");
                int noteColumn = reader.GetOrdinal("note");
                while (reader.Read())
                {
                    long id = reader.GetInt64(idColumn);
                    long position = reader.GetInt64(positionColumn);
                    string nextAction = reader.GetString(nextActionColumn);
                    string note = reader.GetString(noteColumn);
                    listNextAction.Add(new NextActionItem(id, position, nextAction, note));
                }
            }

            callback(listNextAction.ToArray());
        }

        public int AddNextAction(NextActionItem newItem)
        {
            string queryString = "insert into " + tableNextAction + " values(null,@position,@next_action,@note)";
            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@position", newItem.Position);
                dbCommand.Parameters.AddWithValue("@next_action", newItem.NextAction);
                dbCommand.Parameters.AddWithValue("@note", newItem.Note);

                // Add new status to table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.NextActionModified != null)
                {
                    this.NextActionModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        public int DeleteNextAction(NextActionItem newItem)
        {
            string queryString = "delete from " + tableNextAction + " where id=@id";

            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@id", newItem.Id);

                // Delete status from table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.NextActionModified != null)
                {
                    this.NextActionModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        public int UpdateNextAction(NextActionItem newItem)
        {
            string queryString = "update " + tableNextAction
                + " set position=@position, next_action=@next_action, note=@note where id=@id";

            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@id", newItem.Id);
                dbCommand.Parameters.AddWithValue("@position", newItem.Position);
                dbCommand.Parameters.AddWithValue("@next_action", newItem.NextAction);
                dbCommand.Parameters.AddWithValue("@note", newItem.Note);

                // Update status from table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.NextActionModified != null)
                {
                    this.NextActionModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        public bool UpdateNextActionList(NextActionItem[] itemList)
        {
            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            {
                // Open database
                connection.Open();

                // Update status entries
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    foreach (NextActionItem item in itemList)
                    {
                        string queryString = "update " + tableNextAction
                            + " set position=@position, next_action=@next_action, note=@note where id=@id";
                        using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
                        {
                            // Set command parameters.
                            dbCommand.Parameters.AddWithValue("@id", item.Id);
                            dbCommand.Parameters.AddWithValue("@position", item.Position);
                            dbCommand.Parameters.AddWithValue("@next_action", item.NextAction);
                            dbCommand.Parameters.AddWithValue("@note", item.Note);

                            // Update item in table.
                            if (dbCommand.ExecuteNonQuery() != 1)
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }

                    // Commit changes
                    transaction.Commit();

                    // Notify changes.
                    if (this.NextActionModified != null)
                    {
                        this.NextActionModified(this, null);
                    }

                    // Return success
                    return true;
                }
            }
        }

        /*
         * 
         * Job type handling
         * 
         */
        public void JobTypeList(Action<JobTypeItem[]> callback)
        {
            List<JobTypeItem> listJobType = new List<JobTypeItem>();

            string queryString = "select * from " + tableJobType + " order by position";

            // Get status list from database.
            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand command = new SQLiteCommand(queryString, connection))
            {
                connection.Open();

                SQLiteDataReader reader = command.ExecuteReader();
                int idColumn = reader.GetOrdinal("id");
                int positionColumn = reader.GetOrdinal("position");
                int jobTypeColumn = reader.GetOrdinal("job_type");
                int noteColumn = reader.GetOrdinal("note");
                while (reader.Read())
                {
                    long id = reader.GetInt64(idColumn);
                    long position = reader.GetInt64(positionColumn);
                    string jobType = reader.GetString(jobTypeColumn);
                    string note = reader.GetString(noteColumn);
                    listJobType.Add(new JobTypeItem(id, position, jobType, note));
                }
            }

            callback(listJobType.ToArray());
        }

        public int AddJobType(JobTypeItem newItem)
        {
            string queryString = "insert into " + tableJobType + " values(null,@position,@job_type,@note)";
            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@position", newItem.Position);
                dbCommand.Parameters.AddWithValue("@job_type", newItem.JobType);
                dbCommand.Parameters.AddWithValue("@note", newItem.Note);

                // Add new status to table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.JobTypeModified != null)
                {
                    this.JobTypeModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        public int DeleteJobType(JobTypeItem newItem)
        {
            string queryString = "delete from " + tableJobType + " where id=@id";

            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@id", newItem.Id);

                // Delete status from table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.JobTypeModified != null)
                {
                    this.JobTypeModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        public int UpdateJobType(JobTypeItem newItem)
        {
            string queryString = "update " + tableJobType
                + " set position=@position, job_type=@job_type, note=@note where id=@id";

            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@id", newItem.Id);
                dbCommand.Parameters.AddWithValue("@position", newItem.Position);
                dbCommand.Parameters.AddWithValue("@job_type", newItem.JobType);
                dbCommand.Parameters.AddWithValue("@note", newItem.Note);

                // Update status from table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.JobTypeModified != null)
                {
                    this.JobTypeModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        public bool UpdateJobTypeList(JobTypeItem[] itemList)
        {
            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            {
                // Open database
                connection.Open();

                // Update status entries
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    foreach (JobTypeItem item in itemList)
                    {
                        string queryString = "update " + tableJobType
                            + " set position=@position, job_type=@job_type, note=@note where id=@id";
                        using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
                        {
                            // Set command parameters.
                            dbCommand.Parameters.AddWithValue("@id", item.Id);
                            dbCommand.Parameters.AddWithValue("@position", item.Position);
                            dbCommand.Parameters.AddWithValue("@job_type", item.JobType);
                            dbCommand.Parameters.AddWithValue("@note", item.Note);

                            // Update item in table.
                            if (dbCommand.ExecuteNonQuery() != 1)
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }

                    // Commit changes
                    transaction.Commit();

                    // Notify changes.
                    if (this.JobTypeModified != null)
                    {
                        this.JobTypeModified(this, null);
                    }

                    // Return success
                    return true;
                }
            }
        }

        /*
         * 
         * Status handling
         * 
         */
        public void StatusList(Action<StatusItem[]> callback)
        {
            List<StatusItem> listStatus = new List<StatusItem>();

            string queryString = "select * from " + tableStatus + " order by position";

            // Get status list from database.
            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand command = new SQLiteCommand(queryString, connection))
            {
                connection.Open();

                SQLiteDataReader reader = command.ExecuteReader();
                int idColumn = reader.GetOrdinal("id");
                int positionColumn = reader.GetOrdinal("position");
                int statusColumn = reader.GetOrdinal("status");
                int noteColumn = reader.GetOrdinal("note");
                while (reader.Read())
                {
                    long id = reader.GetInt64(idColumn);
                    long position = reader.GetInt64(positionColumn);
                    string status = reader.GetString(statusColumn);
                    string note = reader.GetString(noteColumn);
                    listStatus.Add(new StatusItem(id, position, status, note));
                }
            }

            callback(listStatus.ToArray());
        }

        public int AddStatus(StatusItem newItem)
        {
            string queryString = "insert into " + tableStatus + " values(null,@position,@status,@note)";
            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@position", newItem.Position);
                dbCommand.Parameters.AddWithValue("@status", newItem.Status);
                dbCommand.Parameters.AddWithValue("@note", newItem.Note);

                // Add new status to table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.StatusModified != null)
                {
                    this.StatusModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        public int DeleteStatus(StatusItem newItem)
        {
            string queryString = "delete from " + tableStatus + " where id=@id";

            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@id", newItem.Id);

                // Delete status from table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.StatusModified != null)
                {
                    this.StatusModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        public int UpdateStatus(StatusItem newItem)
        {
            string queryString = "update " + tableStatus 
                + " set position=@position, status=@status, note=@note where id=@id";

            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@id", newItem.Id);
                dbCommand.Parameters.AddWithValue("@position", newItem.Position);
                dbCommand.Parameters.AddWithValue("@status", newItem.Status);
                dbCommand.Parameters.AddWithValue("@note", newItem.Note);

                // Update status from table.
                connection.Open();
                int result = dbCommand.ExecuteNonQuery();

                // Notify changes.
                if (this.StatusModified != null)
                {
                    this.StatusModified(this, null);
                }

                // Return the result.
                return result;
            }
        }

        public bool UpdateStatusList(StatusItem[] itemList)
        {
            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            {
                // Open database
                connection.Open();

                // Update status entries
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    foreach (StatusItem item in itemList)
                    {
                        string queryString = "update " + tableStatus
                            + " set position=@position, status=@status, note=@note where id=@id";
                        using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
                        {
                            // Set command parameters.
                            dbCommand.Parameters.AddWithValue("@id", item.Id);
                            dbCommand.Parameters.AddWithValue("@position", item.Position);
                            dbCommand.Parameters.AddWithValue("@status", item.Status);
                            dbCommand.Parameters.AddWithValue("@note", item.Note);

                            // Update status in table.
                            if(dbCommand.ExecuteNonQuery() != 1)
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }

                    // Commit changes
                    transaction.Commit();

                    // Notify changes.
                    if (this.StatusModified != null)
                    {
                        this.StatusModified(this, null);
                    }

                    // Return success
                    return true;
                }
            }
        }

        /*
         * 
         * Database creation
         * 
         */
        private void CreateDataBase()
        {
            // Table drop and create strings.
            string createActiveCountry = "create table if not exists " + tableActiveCountry + @"( 
                id integer primary key,
                iso2 text unique not null,
                note text not null
                )";

            string createJobType = "create table if not exists " + tableJobType + @"(
                id integer primary key,
                position integer not null,
                job_type text unique not null,
                note text not null
                )";

            string createNextAction = "create table if not exists " + tableNextAction + @"(
                id integer primary key,
                position integer not null,
                next_action unique not null,
                note text not null
                )";

            string createStatus = "create table if not exists " + tableStatus + @"(
                id integer primary key,
                position integer not null,
                status unique not null,
                note text not null
                )";

            string createOrganization = "create table if not exists " + tableOrganization + @"(
                id integer primary key,
                organization text not null,
                division text not null,
                iso2 text not null,
                city text not null,
                contact_name text not null,
                contact_phone text not null,
                contact_email text not null,
                jobs_url text not null,
                organization_url text not null,
                note text not null,
                foreign key(iso2) references " + tableActiveCountry + @"(iso2)
                )";

            string createApplication = "create table if not exists " + tableApplication + @"(
                id integer primary key,
                title text not null,
                job_id text not null,
                job_url text not null,
                job_type text not null,
                status text not null,
                priority integer not null,
                next_action text not null,
                next_action_date text not null,
                organization_id integer not null,
                iso2 text not null,
                city text not null,
                contact_name text not null,
                contact_phone text not null,
                contact_email text not null,
                description text not null,
                note text not null,
                foreign key(organization_id) references " + tableOrganization + @"(id), 
                foreign key(iso2) references " + tableActiveCountry +@"(iso2)
                )";

            // If database file does not exist create database and tables.
            if (!File.Exists(dbPath))
            {
                // Create database file.
                Directory.CreateDirectory(appDataPath);
                SQLiteConnection.CreateFile(dbPath);

                // Create and tables.
                using (SQLiteConnection dbConnection = new SQLiteConnection(strConnection))
                {
                    dbConnection.Open();

                    ExecuteDBNonQuery(createActiveCountry, dbConnection);
                    Console.Out.WriteLine("Active country table created");
                    ExecuteDBNonQuery(createJobType, dbConnection);
                    Console.Out.WriteLine("Job type table created");
                    ExecuteDBNonQuery(createNextAction, dbConnection);
                    Console.Out.WriteLine("Next action table created");
                    ExecuteDBNonQuery(createStatus, dbConnection);
                    Console.Out.WriteLine("Status table created");
                    ExecuteDBNonQuery(createOrganization, dbConnection);
                    Console.Out.WriteLine("Organization table created");
                    ExecuteDBNonQuery(createApplication, dbConnection);
                    Console.Out.WriteLine("Application table created");

                }

                // Fill tables with default data.
                InitializeDataBase();
            }
        }

        private void InitializeDataBase()
        {
            string cleanActiveCountry = "delete from " + tableActiveCountry;
            string activeCountryInsertString = "insert into " + tableActiveCountry + " values";
            string[] activeCountryParams = { "(null, 'CA','')", "(null,'CZ','')", 
                                            "(null, 'DE', '')", "(null, 'FR', '')", 
                                            "(null, 'MX', '')" };
            string cleanJobType = "delete from " + tableJobType;
            string jobTypeInsertString = "insert into " + tableJobType + " values";
            string[] jobTypeParams = { "(null, 0, 'Developer','')", "(null, 1, 'Sr. Developer','')", 
                                         "(null, 2, 'Teacher', '')", "(null, 3, 'Researcher', '')", 
                                         "(null, 4, 'Teacher/Reseacher', '')" };
            string cleanNextAction = "delete from " + tableNextAction;
            string nextActionInsertString = "insert into " + tableNextAction + " values";
            string[] nextActionParams = { "(null, 0, 'Deliver resume','')", "(null, 1, 'Follow up email','')", 
                                        "(null, 2, 'Follow up call','')", "(null, 3, 'Prepare for interview','')", 
                                        "(null, 4, 'No further action','')" };
            string cleanStatus = "delete from " + tableStatus;
            string statusInsertString = "insert into " + tableStatus + " values";
            string[] StatusParams = { "(null, 0, 'No Action Yet','')", "(null, 1, 'Active','')", 
                                        "(null, 2, 'Resume sent','')", "(null, 3, 'Interview scheduled','')", 
                                        "(null, 4, 'Concluded','')" };

            using (SQLiteConnection dbConnection = new SQLiteConnection(strConnection))
            {
                dbConnection.Open();

                // Clean and fill tables
                ExecuteDBNonQuery(cleanActiveCountry, dbConnection);
                foreach (string query in activeCountryParams)
                {
                    ExecuteDBNonQuery(activeCountryInsertString + query, dbConnection);
                }
                Console.Out.WriteLine("Active country table populated");
                ExecuteDBNonQuery(cleanJobType, dbConnection);
                foreach (string query in jobTypeParams)
                {
                    ExecuteDBNonQuery(jobTypeInsertString + query, dbConnection);
                }
                Console.Out.WriteLine("Job type table populated");
                ExecuteDBNonQuery(cleanNextAction, dbConnection);
                foreach (string query in nextActionParams)
                {
                    ExecuteDBNonQuery(nextActionInsertString + query, dbConnection);
                }
                Console.Out.WriteLine("Next action table populated");
                ExecuteDBNonQuery(cleanStatus, dbConnection);
                foreach (string query in StatusParams)
                {
                    ExecuteDBNonQuery(statusInsertString + query, dbConnection);
                }
                Console.Out.WriteLine("Status table populated");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private int ExecuteDBNonQuery(string queryString, SQLiteConnection connection)
        {
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                return dbCommand.ExecuteNonQuery();
            }
        }
    }
}
