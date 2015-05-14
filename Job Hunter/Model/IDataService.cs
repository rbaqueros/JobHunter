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
        void NextActionList(Action<SimpleItem[]> callback);
        int AddNextAction(string title, string note);
        int DeleteNextAction(long id);
        int UpdateNextAction(long id, long position, string title, string note);
        bool NextActionDown(long position);
        bool NextActionUp(long position);
        event EventHandler NextActionModified;

        // Job Type handling
        void JobTypeList(Action<SimpleItem[]> callback);
        int AddJobType(string title, string note);
        int DeleteJobType(long id);
        int UpdateJobType(long id, long position, string title, string note);
        bool JobTypeDown(long position);
        bool JobTypeUp(long position);
        event EventHandler JobTypeModified;

        // Status handling
        void StatusList(Action<SimpleItem[]> callback);
        int AddStatus(string title, string note);
        int DeleteStatus(long id);
        int UpdateStatus(long id, long position, string title, string note);
        bool StatusDown(long position);
        bool StatusUp(long position);
        event EventHandler StatusModified;
    }
}
