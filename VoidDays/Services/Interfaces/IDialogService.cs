using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace VoidDays.Services.Interfaces
{
    public interface IDialogService
    {
        void OpenDialog(object viewModel);
        void CloseDialog(object viewModel);
        void OpenErrorDialog(string message, string title);
        MessageBoxResult OpenInquiryDialog(string message, string title);
    }
}
