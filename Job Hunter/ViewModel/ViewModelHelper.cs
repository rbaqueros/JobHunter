using GalaSoft.MvvmLight.Ioc;
using System;
using System.Diagnostics;

namespace Job_Hunter.ViewModel
{
    internal class ViewModelHelper
    {
        public static async void OpenUrl(string urlString)
        {
            urlString = urlString.StartsWith("http://") || urlString.StartsWith("https://") ? urlString : "http://" + urlString;
            bool urlError = false;
            try
            {
                Process.Start(new ProcessStartInfo(urlString));
            }
            catch (Exception e)
            {
                urlError = true;
            }
            if (urlError)
            {
                IDialogService dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
                string[] buttonText = new string[] { "Ok" };

                int dialogResult = await dialogService.ShowMessageDialog("An error occurred while opening " + urlString + "\nInvalid Url?", "Error", buttonText);
            }
        }
    }
}