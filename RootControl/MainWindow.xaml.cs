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
        private readonly PinService _pinService = new();

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
            ActionPanel.Visibility = Visibility.Collapsed;
            SettingsFeedbackText.Text = string.Empty;
            RefreshIntervalTextBox.Text = "00:15";
            IdleRefreshDelayTextBox.Text = "00:05";
            RefreshEnabledCheckBox.IsChecked = false;
            RefreshOnIdleOnlyCheckBox.IsChecked = false;
            ForceHardRefreshCheckBox.IsChecked = false;
            BlockInputsCheckBox.IsChecked = false;
        }

        public void ShowKioskPreview(string url)
        {
            StateTitleText.Text = "Mode kiosk";
            StateDescriptionText.Text = $"Configuration chargée. URL active : {url}";
            BrowserView.Source = new Uri(url);
            BrowserView.Visibility = Visibility.Visible;
            SettingsPanel.Visibility = Visibility.Collapsed;
            ActionPanel.Visibility = Visibility.Visible;
            SettingsFeedbackText.Text = string.Empty;
        }

        private async void OpenSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            AppSettings settings = await _settingsService.LoadAsync();

            WebUrlTextBox.Text = settings.WebUrl;
            RefreshEnabledCheckBox.IsChecked = settings.RefreshEnabled;
            RefreshIntervalTextBox.Text = settings.RefreshInterval;
            RefreshOnIdleOnlyCheckBox.IsChecked = settings.RefreshOnIdleOnly;
            IdleRefreshDelayTextBox.Text = settings.IdleRefreshDelay;
            ForceHardRefreshCheckBox.IsChecked = settings.ForceHardRefresh;
            BlockInputsCheckBox.IsChecked = settings.BlockInputs;

            MasterPinBox.Password = string.Empty;
            ConfirmMasterPinBox.Password = string.Empty;
            SettingsFeedbackText.Text = string.Empty;

            StateTitleText.Text = "Parametres";
            StateDescriptionText.Text = "Modifie la configuration puis enregistre.";
            BrowserView.Visibility = Visibility.Collapsed;
            ActionPanel.Visibility = Visibility.Collapsed;
            SettingsPanel.Visibility = Visibility.Visible;
        }

        private async void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            string url = WebUrlTextBox.Text.Trim();
            string pin = MasterPinBox.Password;
            string confirmPin = ConfirmMasterPinBox.Password;
            string refreshInterval = RefreshIntervalTextBox.Text.Trim();
            string idleRefreshDelay = IdleRefreshDelayTextBox.Text.Trim();

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? parsedUri) ||
                (parsedUri.Scheme != Uri.UriSchemeHttp && parsedUri.Scheme != Uri.UriSchemeHttps))
            {
                SettingsFeedbackText.Text = "Saisis une URL valide en http ou https.";
                return;
            }

            if (string.IsNullOrWhiteSpace(pin) || pin.Length < 4)
            {
                SettingsFeedbackText.Text = "Le PIN maitre doit contenir au moins 4 caractères.";
                return;
            }

            if (pin != confirmPin)
            {
                SettingsFeedbackText.Text = "Les deux PIN ne correspondent pas.";
                return;
            }

            if (!TimeSpan.TryParse(refreshInterval, out _))
            {
                SettingsFeedbackText.Text = "La duree de rafraichissement doit etre au format hh:mm.";
                return;
            }

            if (!TimeSpan.TryParse(idleRefreshDelay, out _))
            {
                SettingsFeedbackText.Text = "La duree d'inactivite doit etre au format hh:mm.";
                return;
            }

            var settings = new AppSettings
            {
                WebUrl = url,
                RefreshEnabled = RefreshEnabledCheckBox.IsChecked == true,
                RefreshInterval = refreshInterval,
                RefreshOnIdleOnly = RefreshOnIdleOnlyCheckBox.IsChecked == true,
                IdleRefreshDelay = idleRefreshDelay,
                ForceHardRefresh = ForceHardRefreshCheckBox.IsChecked == true,
                BlockInputs = BlockInputsCheckBox.IsChecked == true,
                MasterPinHash = _pinService.HashPin(pin)
            };

            await _settingsService.SaveAsync(settings);

            WebUrlTextBox.Text = string.Empty;
            MasterPinBox.Password = string.Empty;
            ConfirmMasterPinBox.Password = string.Empty;

            ShowKioskPreview(url);
        }
    }
}
