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
    public class ApplicationOrganizationItem : ApplicationItem
    {
        protected string _organization;
        protected string _division;

        public ApplicationOrganizationItem(long id, string title, string jobId, string jobUrl, string jobType, string status, 
            int priority, string nextAction, string nextActionDate, long organizationId, string organization, string division, 
            string iso2, string city, string contactName, string contactPhone, string contactEmail, string description, string note) :
            base(id, title, jobId, jobUrl, jobType, status, priority, nextAction, nextActionDate, organizationId, iso2, city, 
            contactName, contactPhone, contactEmail, description, note)
        {
            _organization = organization;
            _division = division;
        }

        public string Organization
        {
            get { return _organization; }
        }

        public string Division
        {
            get { return _division; }
        }

        public ApplicationItem GetApplicationItem()
        {
            return new ApplicationItem(_id, _title, _jobId, _jobUrl, _jobType, _status, _priority, _nextAction,
                _nextActionDate, _organizationId, _iso2, _city, _contactName, _contactPhone, _contactEmail,
                _description, _note);
        }
    }
}
