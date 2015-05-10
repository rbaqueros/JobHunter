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

namespace Job_Hunter.Model
{
    public class ApplicationItem
    {
        protected long _id;
        protected string _title;
        protected string _jobId;
        protected string _jobUrl;
        protected string _jobType;
        protected string _status;
        protected int _priority;
        protected string _nextAction;
        protected string _nextActionDate;
        protected long _organizationId;
        protected string _iso2;
        protected string _city;
        protected string _contactName;
        protected string _contactPhone;
        protected string _contactEmail;
        protected string _description;
        protected string _note;

        public ApplicationItem(long id, string title, string jobId, string jobUrl, string jobType, string status, 
            int priority, string nextAction, string nextActionDate, long organizationId, string iso2, string city, 
            string contactName, string contactPhone, string contactEmail, string description, string note)
        {
            _id = id;
            _title = title;
            _jobId = jobId ?? "";
            _jobUrl = jobUrl ?? "";
            _jobType = jobType ?? "";
            _status = status ?? "";
            _priority = priority;
            _nextAction = nextAction ?? "";
            _nextActionDate = nextActionDate ?? "";
            _organizationId = organizationId;
            _iso2 = iso2;
            _city = city ?? "";
            _contactName = contactName ?? "";
            _contactPhone = contactPhone ?? "";
            _contactEmail = contactEmail ?? "";
            _description = description ?? "";
            _note = note ?? "";
        }

        public long Id
        {
            get { return _id; }
        }

        public string Title
        {
            get { return _title; }
        }

        public string JobId
        {
            get { return _jobId; }
        }

        public string JobUrl
        {
            get { return _jobUrl; }
        }

        public string JobType
        {
            get { return _jobType; }
        }

        public string Status
        {
            get { return _status; }
        }

        public int Priority
        {
            get { return _priority; }
        }

        public string NextAction
        {
            get { return _nextAction; }
        }

        public string NextActionDate
        {
            get { return _nextActionDate; }
        }

        public long OrganizationId
        {
            get { return _organizationId; }
        }

        public string Iso2
        {
            get { return _iso2; }
        }

        public string City
        {
            get { return _city; }
        }

        public string ContactName
        {
            get { return _contactName; }
        }

        public string ContactPhone
        {
            get { return _contactPhone; }
        }

        public string ContactEmail
        {
            get { return _contactEmail; }
        }

        public string Description
        {
            get { return _description; }
        }

        public string Note
        {
            get { return _note; }
        }
    }
}
