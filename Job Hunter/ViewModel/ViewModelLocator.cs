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

/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:Job_Hunting.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Job_Hunter.Model;

namespace Job_Hunter.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ApplicationViewModel>();
            SimpleIoc.Default.Register<StatusViewModel>();
            SimpleIoc.Default.Register<OrganizationViewModel>();
            SimpleIoc.Default.Register<NextActionViewModel>();
            SimpleIoc.Default.Register<JobTypeViewModel>();
            SimpleIoc.Default.Register<CountryViewModel>();
            SimpleIoc.Default.Register<TabsViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<AboutViewModel>();
        }

        /// <summary>
        /// Dialog service property
        /// </summary>
        public IDialogService DialogService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDialogService>();
            }
            set
            {
                SimpleIoc.Default.Register<IDialogService>(() => value);
            }
        }

        /// <summary>
        /// Gets the data service
        /// </summary>
        public IDataService DataService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDataService>();
            }
        }

        /// <summary>
        /// Gets the Application property.
        /// </summary>
        public ApplicationViewModel Application
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ApplicationViewModel>();
            }
        }

        /// <summary>
        /// Gets the Status view model.
        /// </summary>
        public StatusViewModel Status
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StatusViewModel>();
            }
        }

        /// <summary>
        /// Gets the Job Type view model.
        /// </summary>
        public JobTypeViewModel JobType
        {
            get
            {
                return ServiceLocator.Current.GetInstance<JobTypeViewModel>();
            }
        }

        /// <summary>
        /// Gets the Next Action view model.
        /// </summary>
        public OrganizationViewModel Organization
        {
            get
            {
                return ServiceLocator.Current.GetInstance<OrganizationViewModel>();
            }
        }

        /// <summary>
        /// Gets the Next Action view model.
        /// </summary>
        public NextActionViewModel NextAction
        {
            get
            {
                return ServiceLocator.Current.GetInstance<NextActionViewModel>();
            }
        }

        /// <summary>
        /// Gets the Countries property.
        /// </summary>
        public CountryViewModel Country
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CountryViewModel>();
            }
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        /// <summary>
        /// Gets the Tabs property.
        /// </summary>
        public TabsViewModel Tabs
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TabsViewModel>();
            }
        }

        /// <summary>
        /// Gets the Settings property.
        /// </summary>
        public SettingsViewModel Settings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        /// <summary>
        /// Gets the Settings property.
        /// </summary>
        public AboutViewModel About
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AboutViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}
