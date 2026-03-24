using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RootControl
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ShowConfigurationRequiredState()
        {
            StateTitleText.Text = "Configuration requise";
            StateDescriptionText.Text = "Aucune configuration minimale détectée. L'écran Paramètres devra être ouvert au prochain jalon.";
            BrowserView.Visibility = Visibility.Collapsed;
        }

        public void ShowKioskPreview(string url)
        {
            StateTitleText.Text = "Mode kiosk";
            StateDescriptionText.Text = $"Configuration chargée. URL active : {url}";
            BrowserView.Source = new Uri(url);
            BrowserView.Visibility = Visibility.Visible;
        }
    }
}
