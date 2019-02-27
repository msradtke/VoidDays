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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VoidDays
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int _count = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (((dynamic)DataContext).MoveSwitch == true)
            {
                ((dynamic)DataContext).MoveSwitch = false;
                return;
            }
            //Console.WriteLine("Mouse moved " + _count++);
            if (DataContext != null)
                ((dynamic)DataContext).MouseMoved();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.D && Keyboard.Modifiers == ModifierKeys.Control)
            {
                ((dynamic)DataContext).SecretCodeEntered();
            }
            
        }
    }
}
