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

using Job_Hunter.Model;

namespace Job_Hunter.ViewModel
{
    public class ApplicationOrganizationVMItem : ApplicationOrganizationItem
    {
        public ApplicationOrganizationVMItem(ApplicationOrganizationItem item) : 
            base(item.Id, item.Title, item.JobId, item.JobUrl, item.JobType, item.Status, 
            item.Priority, item.NextAction, item.NextActionDate, item.OrganizationId, 
            item.Organization, item.Division, item.Iso2, item.City, item.ContactName, 
            item.ContactPhone, item.ContactEmail, item.Description, item.Note)
        {

        }

        public string OrganizationDivision
        {
            get
            {
                return (_division != null && _division.Length > 0) ? _organization + " - " + _division : _organization;
            }
        }
    }
}
