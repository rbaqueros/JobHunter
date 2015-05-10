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
using System.Windows.Media.Imaging;
using Baqsoft.CountryInfo;
using Job_Hunter.Model;

namespace Job_Hunter.ViewModel
{
    public class OrganizationViewModelItem : OrganizationItem
    {
        private BitmapImage _countryFlag;

        public OrganizationViewModelItem(long id, string organization, string division, string iso2Code, 
            string city, string contactName, string contactPhone, string contactEmail, string url, 
            string jobs_url, string note) :
            base(id, organization, division, iso2Code, city, contactName, contactPhone, contactEmail, url, 
            jobs_url, note)
        {
            // Get country flag
            _countryFlag = CountryArray.GetCountry(_iso2Code).Flag;
        }

        public OrganizationViewModelItem(OrganizationItem item): base(item.Id, item.Organization, item.Division, 
            item.Iso2Code, item.City, item.ContactName, item.ContactPhone, item.ContactEmail, item.Url, 
            item.JobsUrl, item.Note)
        {
            // Get country flag
            _countryFlag = CountryArray.GetCountry(_iso2Code).Flag;
        }

        public string OrganizationDivision
        {
            get { return _organization + (String.IsNullOrWhiteSpace(_division) ? "" : " - " + _division); }
        }

        public BitmapImage CountryFlag
        {
            get { return _countryFlag; }
            private set
            {
                if (value == _countryFlag) { return; }
                _countryFlag = value;
            }
        }

        public OrganizationItem GetOrganizationItem()
        {
            return new OrganizationItem(_id, _organization, _division, _iso2Code, _city, _contactName, 
                _contactPhone, _contactEmail, _url, _jobsUrl, _note);
        }
    }
}
