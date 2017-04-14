using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Services.Interfaces;
using System.Windows;
using VoidDays.Views;
namespace VoidDays.Services
{
    public class DialogService : IDialogService
    {
        private Dictionary<object, Window> DialogViewModelDict;
        public DialogService()
        {
            DialogViewModelDict = new Dictionary<object, Window>();
        }
        public void OpenDialog(object viewModel)
        {
            
            DialogView dialog = new DialogView();
            dialog.Owner = Application.Current.MainWindow;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.DataContext = viewModel;
            DialogViewModelDict.Add(viewModel,dialog);
            dialog.ShowDialog();
        }
        public void CloseDialog(object viewModel)
        {
            if (DialogViewModelDict.ContainsKey(viewModel))
            {
                DialogViewModelDict[viewModel].Close();
                DialogViewModelDict.Remove(viewModel);
            }

        }

        public void OpenErrorDialog(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public MessageBoxResult OpenInquiryDialog(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            
        }
    }
}
