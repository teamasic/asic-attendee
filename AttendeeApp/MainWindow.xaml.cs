using AttendeeApp.Handler;
using AttendeeApp.Utils;
using CefSharp;
using CefSharp.Wpf;
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

namespace AttendeeWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChromiumWebBrowser browser;
        private Config config;
        private const string CACHE_PATH = "BrowserCache";

        public MainWindow()
        {
            InitializeComponent();
            config = Utils.GetConfig();

            InitBrowser();
        }

        private void InitBrowser()
        {
            CefSettings settings = new CefSettings();
            settings.CachePath = System.IO.Path.GetFullPath(CACHE_PATH);

            Cef.Initialize(settings);

            browser = new ChromiumWebBrowser(config.Url);
            browser.RequestHandler = new CustomRequestHandler();

            grid.Children.Add(browser);
        }

    }

}
