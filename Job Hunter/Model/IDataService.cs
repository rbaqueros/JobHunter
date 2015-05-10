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

namespace Job_Hunter.Model
{
    public interface IDataService
    {
        //void InitializeCollections(Action<int> callback);
        //void SaveCollections(Action<int> callback);

        // Application handling
        void ApplicationList(Action<ApplicationOrganizationItem[]> callback);
        int AddApplication(ApplicationItem item);
        int DeleteApplication(ApplicationItem item);
        int UpdateApplication(ApplicationItem item);
        event EventHandler ApplicationModified;

        // Organization handling
        void OrganizationList(Action<OrganizationItem[]> callback);
        int AddOrganization(OrganizationItem item);
        int DeleteOrganization(OrganizationItem item);
        int UpdateOrganization(OrganizationItem item);
        event EventHandler OrganizationModified;

        // Active country handling
        void ActiveCountryList(Action<ActiveCountryItem[]> callback);
        int AddActiveCountry(ActiveCountryItem item);
        int DeleteActiveCountry(ActiveCountryItem item);
        int UpdateActiveCountryNotes(ActiveCountryItem item);
        event EventHandler CountryModified;

        // Next Action handling
        void NextActionList(Action<NextActionItem[]> callback);
        int AddNextAction(NextActionItem item);
        int DeleteNextAction(NextActionItem item);
        int UpdateNextAction(NextActionItem item);
        bool UpdateNextActionList(NextActionItem[] itemList);
        event EventHandler NextActionModified;

        // Job Type handling
        void JobTypeList(Action<JobTypeItem[]> callback);
        int AddJobType(JobTypeItem item);
        int DeleteJobType(JobTypeItem item);
        int UpdateJobType(JobTypeItem item);
        bool UpdateJobTypeList(JobTypeItem[] itemList);
        event EventHandler JobTypeModified;

        // Status handling
        void StatusList(Action<StatusItem[]> callback);
        int AddStatus(StatusItem item);
        int DeleteStatus(StatusItem item);
        int UpdateStatus(StatusItem item);
        bool UpdateStatusList(StatusItem[] itemList);
        event EventHandler StatusModified;
    }
}
