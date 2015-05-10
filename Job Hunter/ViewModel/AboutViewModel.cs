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

using GalaSoft.MvvmLight;
using System.Reflection;

namespace Job_Hunter.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class AboutViewModel : ViewModelBase
    {
        private string _appName;
        private string _appCompany;
        private string _appDescription;
        private string _appCopyright;
        private string _appVersion;
        private string _appLicense;

        /// <summary>
        /// Initializes a new instance of the AboutViewModel class.
        /// </summary>
        public AboutViewModel() 
        {
            // Get assembly info
            var assemblyInfo = new AssemblyName(Assembly.GetExecutingAssembly().FullName);

            // Get assembly name
            _appName = assemblyInfo.Name.ToString();

            // Get assembly version
            _appVersion = assemblyInfo.Version.ToString();

            // Get assembly company
            var companyAttrib = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            _appCompany = ((AssemblyCompanyAttribute)companyAttrib[0]).Company;

            // Get assembly description
            var descAttrib = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            _appDescription = ((AssemblyDescriptionAttribute)descAttrib[0]).Description;

            // Get assembly copyright
            var copyAttrib = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            _appCopyright = ((AssemblyCopyrightAttribute)copyAttrib[0]).Copyright;

            // Set assembly license
            _appLicense = "You are free to use this program under the terms of the Apache License, Version 2.0 (the \"License\").\n"
                + "You may obtain a copy of the License at:\n\n\t\t http://www.apache.org/licenses/LICENSE-2.0 \n\n";
        }


        // Get assembly name
        public string AppName
        {
            get { return _appName; }
            protected set { _appName = value; }
        }

        // Get assembly company
        public string AppCompany
        {
            get { return _appCompany; }
            protected set { _appCompany = value; }
        }

        // Get assembly description
        public string AppDescription
        {
            get { return _appDescription; }
            protected set { _appDescription = value; }
        }

        // Get assembly copyright
        public string AppCopyright
        {
            get { return _appCopyright; }
            protected set { _appCopyright = value; }
        }

        // Get assembly version
        public string AppVersion
        {
            get { return _appVersion; }
            protected set { _appVersion = value; }
        }

        // Get assembly license
        public string AppLicense
        {
            get { return _appLicense; }
            protected set { _appLicense = value; }
        }
    }
}