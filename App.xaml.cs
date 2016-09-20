using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Lift
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _mainWindow = new MainWindow();
            Console.WriteLine("OnStartup was called");
            _mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //_mainWindow.Close();
            base.OnExit(e);
            Console.WriteLine("OnExit was called");
        }
    }
}
