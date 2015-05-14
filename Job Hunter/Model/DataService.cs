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
        public const string tableActiveCountry = "active_country";
        public const string tableJobType = "job_type";
        public const string tableNextAction = "next_action";
        public const string tableStatus = "status";
        public const string tableOrganization = "organization";
        public const string tableApplication = "application";

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
        public void NextActionList(Action<SimpleItem[]> callback)
        {
            callback(SimpleItemList(tableNextAction));
        }

        public int AddNextAction(string title, string note)
        {
            int result = AddSimpleItem(title, note, tableNextAction);

            if (result > 0)
            {
                // Notify changes.
                if (this.NextActionModified != null)
                {
                    this.NextActionModified(this, null);
                }
            }

            return result;
        }

        public int DeleteNextAction(long id)
        {
            int result = DeleteSimpleItem(id, tableNextAction);
            if (result > 0)
            {
                // Notify changes.
                if (this.NextActionModified != null)
                {
                    this.NextActionModified(this, null);
                }
            }

            return result;
        }

        public int UpdateNextAction(long id, long position, string title, string note)
        {
            // Update status
            int result = UpdateSimpleItem(id, position, title, note, tableNextAction);

            // If table changed notify
            if (result > 0)
            {
                // Notify changes.
                if (this.NextActionModified != null)
                {
                    this.NextActionModified(this, null);
                }
            }

            // Return the result
            return result;
        }

        public bool NextActionDown(long position)
        {
            // Switch item positions
            if (SwitchPosition(position, tableNextAction, false))
            {
                // Notify changes.
                if (this.NextActionModified != null)
                {
                    this.NextActionModified(this, null);
                }

                // Return success
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool NextActionUp(long position)
        {
            // Switch item positions
            if (SwitchPosition(position, tableNextAction, true))
            {
                // Notify changes.
                if (this.NextActionModified != null)
                {
                    this.NextActionModified(this, null);
                }

                // Return success
                return true;
            }
            else
            {
                return false;
            }
        }

        /*
         * 
         * Job type handling
         * 
         */
        public void JobTypeList(Action<SimpleItem[]> callback)
        {
            callback(SimpleItemList(tableJobType));
        }

        public int AddJobType(string title, string note)
        {
            int result = AddSimpleItem(title, note, tableJobType);

            if (result > 0)
            {
                // Notify changes.
                if (this.JobTypeModified != null)
                {
                    this.JobTypeModified(this, null);
                }
            }

            return result;
        }

        public int DeleteJobType(long id)
        {
            int result = DeleteSimpleItem(id, tableJobType);
            if (result > 0)
            {
                // Notify changes.
                if (this.JobTypeModified != null)
                {
                    this.JobTypeModified(this, null);
                }
            }

            return result;
        }

        public int UpdateJobType(long id, long position, string title, string note)
        {
            // Update status
            int result = UpdateSimpleItem(id, position, title, note, tableJobType);

            // If table changed notify
            if (result > 0)
            {
                // Notify changes.
                if (this.JobTypeModified != null)
                {
                    this.JobTypeModified(this, null);
                }
            }

            // Return the result
            return result;
        }

        public bool JobTypeDown(long position)
        {
            // Switch item positions
            if (SwitchPosition(position, tableJobType, false))
            {
                // Notify changes.
                if (this.JobTypeModified != null)
                {
                    this.JobTypeModified(this, null);
                }

                // Return success
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool JobTypeUp(long position)
        {
            // Switch item positions
            if (SwitchPosition(position, tableJobType, true))
            {
                // Notify changes.
                if (this.JobTypeModified != null)
                {
                    this.JobTypeModified(this, null);
                }

                // Return success
                return true;
            }
            else
            {
                return false;
            }
        }

        /*
         * 
         * Status handling
         * 
         */
        public void StatusList(Action<SimpleItem[]> callback)
        {
            callback(SimpleItemList(tableStatus));
        }

        public int AddStatus(string title, string note)
        {
            int result = AddSimpleItem(title, note, tableStatus);

            if (result > 0)
            {
                // Notify changes.
                if (this.StatusModified != null)
                {
                    this.StatusModified(this, null);
                }
            }

            return result;
        }

        public int DeleteStatus(long id)
        {
            int result = DeleteSimpleItem(id, tableStatus);
            Console.Out.WriteLine("Rows afected: " + result);
            if (result > 0)
            {
                // Notify changes.
                if (this.StatusModified != null)
                {
                    this.StatusModified(this, null);
                }
            }

            return result;
        }

        public int UpdateStatus(long id, long position, string title, string note)
        {
            // Update status
            int result = UpdateSimpleItem(id, position, title, note, tableStatus);

            // If table changed notify
            if (result > 0)
            {
                // Notify changes.
                if (this.StatusModified != null)
                {
                    this.StatusModified(this, null);
                }
            }

            // Return the result
            return result;
        }

        public bool StatusDown(long position)
        {
            // Switch item positions
            if (SwitchPosition(position, tableStatus, false))
            {
                // Notify changes.
                if (this.StatusModified != null)
                {
                    this.StatusModified(this, null);
                }

                // Return success
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool StatusUp(long position)
        {
            // Switch item positions
            if (SwitchPosition(position, tableStatus, true))
            {
                // Notify changes.
                if (this.StatusModified != null)
                {
                    this.StatusModified(this, null);
                }

                // Return success
                return true;
            }
            else
            {
                return false;
            }
        }

/*
 * 
 * Simple item methods and classes
 * 
 */
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private SimpleItem[] SimpleItemList(string table)
        {
            List<SimpleItem> itemList = new List<SimpleItem>();

            string queryString = "select * from " + table + " order by position";

            // Get status list from database.
            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand command = new SQLiteCommand(queryString, connection))
            {
                connection.Open();

                SQLiteDataReader reader = command.ExecuteReader();
                int idColumn = reader.GetOrdinal("id");
                int positionColumn = reader.GetOrdinal("position");
                int titleColumn = reader.GetOrdinal(table);
                int noteColumn = reader.GetOrdinal("note");
                while (reader.Read())
                {
                    long id = reader.GetInt64(idColumn);
                    long position = reader.GetInt64(positionColumn);
                    string title = reader.GetString(titleColumn);
                    string note = reader.GetString(noteColumn);
                    itemList.Add(new SimpleItem(id, position, title, note));
                }
            }

            return itemList.ToArray();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private int AddSimpleItem(string title, string note, string table)
        {
            string nextPositionQuery = "select max(position)+1 from " + table;
            string queryString = "insert into " + table + " values(null,(" + nextPositionQuery + "),@title,@note)";
            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@title", title);
                dbCommand.Parameters.AddWithValue("@note", note);

                // Add new status to table.
                connection.Open();
                return dbCommand.ExecuteNonQuery();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private int UpdateSimpleItem(long id, long position, string title, string note, string table)
        {
            string queryString = "update " + table
                + " set position=@position, " + table + "=@title, note=@note where id=@id";

            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@id", id);
                dbCommand.Parameters.AddWithValue("@position", position);
                dbCommand.Parameters.AddWithValue("@title", title);
                dbCommand.Parameters.AddWithValue("@note", note);

                // Update status from table.
                connection.Open();
                return dbCommand.ExecuteNonQuery();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private int DeleteSimpleItem(long id, string table)
        {
            string queryString = "delete from " + table + " where id=@id";

            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
            {
                // Set command parameters.
                dbCommand.Parameters.AddWithValue("@id", id);

                // Delete status from table.
                connection.Open();
                return dbCommand.ExecuteNonQuery();
            }
        }

        private class IdPosition
        {
            long _id;
            long _position;

            public IdPosition(long id, long position)
            {
                _id = id;
                _position = position;
            }

            public long Id
            {
                get { return _id; }
            }

            public long Position
            {
                get { return _position; }
                set { _position = value; }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private bool SwitchPosition(SimpleItem item, string table, bool moveUp)
        {
            return SwitchPosition(item.Position, table, moveUp);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private bool SwitchPosition(long pos, string table, bool moveUp)
        {
            string queryString;
            List<IdPosition> listIdPosition = new List<IdPosition>();

            using (SQLiteConnection connection = new SQLiteConnection(strConnection))
            {
                // Open database
                connection.Open();

                // Select query string depending on direction
                if (moveUp)
                {
                    // Move up string
                    queryString = "select id,position from (select id,position from " + table
                        + " where position=@position limit 1)"
                        + " union "
                        + "select id,position from (select id,position from " + table + " where position<@position "
                        + "order by position desc limit 1) order by position";
                }
                else
                {
                    // Move down string
                    queryString = "select id,position from (select id,position from " + table
                        + " where position=@position limit 1)"
                        + " union "
                        + "select id,position from (select id,position from " + table + " where position>@position "
                        + "order by position limit 1) order by position";
                }

                Console.Out.WriteLine("Moving item in position: " + pos + (moveUp ? " up" : " down"));
                using (SQLiteCommand command = new SQLiteCommand(queryString, connection))
                {
                    // Set command parameters.
                    command.Parameters.AddWithValue("@position", pos);

                    // Get current and next item
                    SQLiteDataReader reader = command.ExecuteReader();
                    int idColumn = reader.GetOrdinal("id");
                    int positionColumn = reader.GetOrdinal("position");
                    while (reader.Read())
                    {
                        long id = reader.GetInt64(idColumn);
                        long position = reader.GetInt64(positionColumn);
                        listIdPosition.Add(new IdPosition(id, position));
                    }
                }
                if (listIdPosition.Count != 2) { return false; }

                // Exchange positions
                long tmpPosition = listIdPosition[0].Position;
                listIdPosition[0].Position = listIdPosition[1].Position;
                listIdPosition[1].Position = tmpPosition;

                // Update table entries
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    foreach (IdPosition idPosition in listIdPosition)
                    {
                        queryString = "update " + table + " set position=@position where id=@id";
                        using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, connection))
                        {
                            // Set command parameters.
                            dbCommand.Parameters.AddWithValue("@id", idPosition.Id);
                            dbCommand.Parameters.AddWithValue("@position", idPosition.Position);

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
            // For simple item tables (e.g. next action, job type and status) table name MUST
            // be the same as the title column name. This convention is used in several
            // methods.

            // Table drop and create strings.
            string createActiveCountry = "create table if not exists " + tableActiveCountry + @"( 
                id integer primary key,
                iso2 text unique not null,
                note text not null
                )";

            string createJobType = "create table if not exists " + tableJobType + @"(
                id integer primary key,
                position integer not null,"
                + tableJobType + @" text unique not null,
                note text not null
                )";

            string createNextAction = "create table if not exists " + tableNextAction + @"(
                id integer primary key,
                position integer not null,"
                + tableNextAction +@" text unique not null,
                note text not null
                )";

            string createStatus = "create table if not exists " + tableStatus + @"(
                id integer primary key,
                position integer not null,"
                 + tableStatus + @" text unique not null,
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
                    ExecuteDBNonQuery(createJobType, dbConnection);
                    ExecuteDBNonQuery(createNextAction, dbConnection);
                    ExecuteDBNonQuery(createStatus, dbConnection);
                    ExecuteDBNonQuery(createOrganization, dbConnection);
                    ExecuteDBNonQuery(createApplication, dbConnection);

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
                ExecuteDBNonQuery(cleanJobType, dbConnection);
                foreach (string query in jobTypeParams)
                {
                    ExecuteDBNonQuery(jobTypeInsertString + query, dbConnection);
                }
                ExecuteDBNonQuery(cleanNextAction, dbConnection);
                foreach (string query in nextActionParams)
                {
                    ExecuteDBNonQuery(nextActionInsertString + query, dbConnection);
                }
                ExecuteDBNonQuery(cleanStatus, dbConnection);
                foreach (string query in StatusParams)
                {
                    ExecuteDBNonQuery(statusInsertString + query, dbConnection);
                }
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
