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
using System.Threading.Tasks;
using RootControl.Models;
using RootControl.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RootControl
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly SettingsService _settingsService = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        public void ShowConfigurationRequiredState()
        {
            StateTitleText.Text = "Configuration requise";
            StateDescriptionText.Text = "Aucune configuration minimale détectée. L'écran Paramètres devra être ouvert au prochain jalon.";
            BrowserView.Visibility = Visibility.Collapsed;
            SettingsPanel.Visibility = Visibility.Visible;
            SettingsFeedbackText.Text = string.Empty;
        }

        public void ShowKioskPreview(string url)
        {
            StateTitleText.Text = "Mode kiosk";
            StateDescriptionText.Text = $"Configuration chargée. URL active : {url}";
            BrowserView.Source = new Uri(url);
            BrowserView.Visibility = Visibility.Visible;
            SettingsPanel.Visibility = Visibility.Collapsed;
            SettingsFeedbackText.Text = string.Empty;
        }

        private async void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            string url = WebUrlTextBox.Text.Trim();
            string pin = MasterPinBox.Password;
            string confirmPin = ConfirmMasterPinBox.Password;

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? parsedUri) ||
                (parsedUri.Scheme != Uri.UriSchemeHttp && parsedUri.Scheme != Uri.UriSchemeHttps))
            {
                SettingsFeedbackText.Text = "Saisis une URL valide en http ou https.";
                return;
            }

            if (string.IsNullOrWhiteSpace(pin) || pin.Length < 4)
            {
                SettingsFeedbackText.Text = "Le PIN maitre doit contenir au moins 4 caracteres.";
                return;
            }

            if (pin != confirmPin)
            {
                SettingsFeedbackText.Text = "Les deux PIN ne correspondent pas.";
                return;
            }

            var settings = new AppSettings
            {
                WebUrl = url,
                MasterPinHash = pin
            };

            await _settingsService.SaveAsync(settings);

            WebUrlTextBox.Text = string.Empty;
            MasterPinBox.Password = string.Empty;
            ConfirmMasterPinBox.Password = string.Empty;

            ShowKioskPreview(url);
        }
    }
}
