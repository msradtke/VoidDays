using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VoidDays.ViewModels;
using VoidDays.ViewModels.Interfaces;
namespace VoidDays.Views
{
    /// <summary>
    /// Interaction logic for MainContainer.xaml
    /// </summary>
    public partial class MainContainer : Window
    {
        public MainContainer(IMainContainerViewModel mainContainerViewModel)
        {
            var vm = mainContainerViewModel;
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
