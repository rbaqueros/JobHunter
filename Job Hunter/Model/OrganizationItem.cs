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
    public class OrganizationItem
    {
        protected long _id;
        protected string _organization;
        protected string _division;
        protected string _iso2Code;
        protected string _city;
        protected string _contactName;
        protected string _contactPhone;
        protected string _contactEmail;
        protected string _url;
        protected string _jobsUrl;
        protected string _note;

        public OrganizationItem(long id, string organization, string division, string iso2Code, 
            string city, string contactName, string contactPhone, string contactEmail, 
            string url, string jobs_url, string note)
        {
            // Initialize fields
            _id = id;
            _organization = organization;
            _division = division;
            _iso2Code = iso2Code;
            _city = city;
            _contactName = contactName;
            _contactPhone = contactPhone;
            _contactEmail = contactEmail;
            _url = url;
            _jobsUrl = jobs_url;
            _note = note;
        }

        public long Id
        {
            get { return _id; }
        }

        public string Organization
        {
            get { return _organization; }
        }

        public string Division
        {
            get { return _division; }
        }

        public string Iso2Code
        {
            get { return _iso2Code; }
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

        public string Url
        {
            get { return _url; }
        }

        public string JobsUrl
        {
            get { return _jobsUrl; }
        }

        public string Note
        {
            get { return _note; }
        }
    }
}
