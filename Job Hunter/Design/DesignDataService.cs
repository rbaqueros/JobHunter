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
using Job_Hunter.Model;
using System.Collections.Generic;

namespace Job_Hunter.Design
{
    public class DesignDataService : IDataService
    {
        // Event declaration
        public event EventHandler StatusModified;
        public event EventHandler NextActionModified;
        public event EventHandler JobTypeModified;
        public event EventHandler CountryModified;
        public event EventHandler OrganizationModified;
        public event EventHandler ApplicationModified;

        // Organization handling
        public void ApplicationList(Action<ApplicationOrganizationItem[]> callback)
        {
            ApplicationOrganizationItem[] applicationArray = new ApplicationOrganizationItem[]{
                new ApplicationOrganizationItem(1, "CS Dept. Teacher/Researcher", "0101", "www.uam.mx", "Teacher/Researcher", "Active", 0, 
                    "Follow up call", "18/04/2014", 1, "UAM", "Azcapotzalco", "MX", "Mexico, D.F.", "Contact Name", "244 103 7292", 
                    "ggrace@uam.mx", "Description of job offering 1", "Notes on job offering 1"),
                new ApplicationOrganizationItem(2, "CS Researcher", "0202", "www.inaoe.mx", "Researcher", "Active", 1, 
                    "Follow up email", "27/03/2014", 2, "INAOE", "", "MX", "Puebla", "", "", "", "Description of job offering 2", 
                    "Notes on job offering 2"),
                new ApplicationOrganizationItem(3, "CS Dept. Teacher", "0303", "www.buap.mx", "Teacher", "No action yet", 2, 
                    "Deliver resume", "20/03/2014", 3, "BUAP", "", "MX", "Puebla", "Buap HM", "123 456 7890", "hmbuap@buap.mx", 
                    "Description on job offering 3", "Notes on job offering 3"),
                new ApplicationOrganizationItem(4, "EE Dept. TEacher", "", "www.tecpue.mx", "Teacher", "No action yet", 3, 
                    "Deliver resume", "03/04/2014", 4, "Tecnologico de Puebla", "EE Dept.", "MX", "Puebla", "Bill Johnson", 
                    "587 258 9510", "bjohnson@tecpue.mx", "Description for job offering 4", "Notes on job offering 4"),
                new ApplicationOrganizationItem(5, "Applied Technology Researcher", "05123", "www.ericsson.com", "Researcher", "Rejected", 5, 
                    "No further action", "", 2, "Ericsson", "Research", "SE", "Stockholm", "Albert Einstein", "687 357 8520", "aeinstein@ericsson.com", 
                    "Description for job offering 6", "Notes on job offering 6"),
                new ApplicationOrganizationItem(6, "PhD Developer", "9087", "www.google.com", "Researcher", "Resume sent", 0, 
                    "Follow up email", "08/04/2014", 3, "Google", "Research", "US", "San Francisco, CA", "Sergei Bin", "325 658 9856", "sbin@google.com", 
                    "Description for Google job offering", "Notes on job offering 7")
            };

            callback(applicationArray);
        }
        public int AddApplication(ApplicationItem item)
        {
            return 0;
        }
        public int DeleteApplication(ApplicationItem item)
        {
            return 0;
        }
        public int UpdateApplication(ApplicationItem item)
        {
            return 0;
        }

        // Organization handling
        public void OrganizationList(Action<OrganizationItem[]> callback)
        {
            OrganizationItem[] organizationArray = new OrganizationItem[]{
                new OrganizationItem(1, "UAM", "Azcapotzalco", "MX", "Mexico, D.F.", "Maricela Claudia Bravo Contreras", "(55) 5555-1555", "mbravo@uam.mx", "www.uam.mx", "www.uam.mx", "Note for organization UAM"),
                new OrganizationItem(2, "INAOE", "", "MX", "Puebla, Pue.", "Eduardo Morales", "(244) 107-1815", "emorales@inaoe.mx", "www.inaoe.mx", "www.inaoe.mx/jobs", "Note for organization INAOE"),
                new OrganizationItem(3, "BUAP", "", "MX", "Puebla, Pue.", "", "", "", "www.buap,mx", "www.buap.mx/jobs", "Note for organization BUAP"),
                new OrganizationItem(4, "Tecnologico Puebla", "", "MX", "Puebla, Pue.", "", "", "", "www.tecpuebla.mx", "www.tecpuebla.mx/jobs", "Note for organization Tecnologico Puebla"),
                new OrganizationItem(5, "Tec de Monterrey", "Camppus Puebla", "MX", "Puebla, Pue.", "Armando Rodriguez", "(222) 212-1212", "arodriguez@tec.edu.mx", "www.tecmonterrey.mx", "www.tecmonterrey.mx/jobs", "Note for organization Tecnologico de Monterrey"),
                new OrganizationItem(6, "Ericsson", "Research", "SW", "Some city", "Ralph Peterson", "(451) 567-2341", "peterson@ericsson.com", "www.ericsson.com", "www.ericsson.com/jobs", "Notes for organization Ericsson"),
                new OrganizationItem(7, "Google", "Research", "MX", "Mexico, D.F.", "Larry Page", "(467) 287-5612", "page@google.com", "www.google.com", "www.google.com/jobs", "Notes for organization Google")
            };

            callback(organizationArray);
        }
        public int AddOrganization(OrganizationItem item)
        {
            return 0;
        }
        public int DeleteOrganization(OrganizationItem item)
        {
            return 0;
        }
        public int UpdateOrganization(OrganizationItem item)
        {
            return 0;
        }

        // Active country handling
        public void ActiveCountryList(Action<ActiveCountryItem[]> callback)
        {
            ActiveCountryItem[] countryArray = new ActiveCountryItem[] {
                new ActiveCountryItem(1, "AU", "Notes for Australia"),
                new ActiveCountryItem(2, "BR", "Notes for Brazil"),
                new ActiveCountryItem(3, "CA", "Notes for Canada"),
                new ActiveCountryItem(4, "DE", "Notes for Germany"),
                new ActiveCountryItem(5, "MX", "Notes for Mexico"),
                new ActiveCountryItem(6, "NO", "Notes for Norway"),
                new ActiveCountryItem(7, "NZ", "Notes for New Zealand")
            };

            callback(countryArray);
        }

        public int AddActiveCountry(ActiveCountryItem item)
        {
            return 0;
        }

        public int DeleteActiveCountry(ActiveCountryItem item)
        {
            return 0;
        }

        public int UpdateActiveCountryNotes(ActiveCountryItem item)
        {
            return 0;
        }

        // Next action handling
        public void NextActionList(Action<NextActionItem[]> callback)
        {
            List<NextActionItem> nextActionList = new List<NextActionItem>();

            nextActionList.Add(new NextActionItem(0, 0, "Deliver resume", "Notes on Deliver resume next action"));
            nextActionList.Add(new NextActionItem(1, 1, "Follow up email", "Notes on Follow up email next action"));
            nextActionList.Add(new NextActionItem(2, 2, "Follow up call", "Notes on Follow up call next action"));
            nextActionList.Add(new NextActionItem(3, 3, "Prepare for interview", "Notes on Prepare for interview next action"));
            nextActionList.Add(new NextActionItem(4, 4, "No further action", "Notes on No further action next action"));

            callback(nextActionList.ToArray());
        }

        public int AddNextAction(NextActionItem item)
        {
            return 0;
        }

        public int DeleteNextAction(NextActionItem item)
        {
            return 0;
        }

        public int UpdateNextAction(NextActionItem item)
        {
            return 0;
        }

        public bool UpdateNextActionList(NextActionItem[] itemList)
        {
            return true;
        }

        // Job type handling
        public void JobTypeList(Action<JobTypeItem[]> callback)
        {
            List<JobTypeItem> jobTypeList = new List<JobTypeItem>();

            jobTypeList.Add(new JobTypeItem(0, 0, "Academic Research", "Notes on Academic Research job type"));
            jobTypeList.Add(new JobTypeItem(1, 1, "University", "Notes on University job type"));
            jobTypeList.Add(new JobTypeItem(2, 2, "Private Research", "Notes on Private Research job type"));
            jobTypeList.Add(new JobTypeItem(3, 3, "Developer", "Notes on Developer job type"));

            callback(jobTypeList.ToArray());
        }

        public int AddJobType(JobTypeItem item)
        {
            return 0;
        }

        public int DeleteJobType(JobTypeItem item)
        {
            return 0;
        }

        public int UpdateJobType(JobTypeItem item)
        {
            return 0;
        }

        public bool UpdateJobTypeList(JobTypeItem[] itemList)
        {
            return true;
        }

        // Status handling
        public void StatusList(Action<StatusItem[]> callback)
        {
            List<StatusItem> statusList = new List<StatusItem>();

            statusList.Add(new StatusItem(0, 0, "No action yet", "Notes on no action yet status"));
            statusList.Add(new StatusItem(1, 1, "Active", "Notes on active status"));
            statusList.Add(new StatusItem(2, 2, "Resume sent", "Notes on resume sent status"));
            statusList.Add(new StatusItem(3, 3, "Interview scheduled", "Notes on interview scheduled status"));
            statusList.Add(new StatusItem(4, 4, "Concluded", "Notes on concluded status"));

            callback(statusList.ToArray());
        }

        public int AddStatus(StatusItem item)
        {
            return 0;
        }

        public int DeleteStatus(StatusItem item)
        {
            return 0;
        }

        public int UpdateStatus(StatusItem item)
        {
            return 0;
        }

        public bool UpdateStatusList(StatusItem[] itemList)
        {
            return true;
        }
    }
}