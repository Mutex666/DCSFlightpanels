﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using ClassLibraryCommon;
using DCS_BIOS;
using DCSFlightpanels.Properties;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace DCSFlightpanels.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {

        public string IpAddressFromDCSBIOS { get; private set; }
        public string PortFromDCSBIOS { get; private set; }
        public string IpAddressToDCSBIOS { get; private set; }
        public string PortToDCSBIOS { get; private set; }
        public string DcsBiosJSONLocation { get; private set; }
        public string IpAddressFromSRS { get; private set; }
        public string PortFromSRS { get; private set; }
        public string IpAddressToSRS { get; private set; }
        public string PortToSRS { get; private set; }
        public bool GeneralChanged { get; private set; } = false;
        public bool DCSBIOSChanged { get; private set; } = false;
        public bool SRSChanged { get; private set; } = false;
        public bool StreamDeckChanged { get; private set; } = false;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SettingsWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ButtonOk.IsEnabled = false;
                LoadSettings();
                if (!Common.IsEmulationModesFlagSet(EmulationMode.SRSEnabled))
                {
                    LabelSRS.Visibility = Visibility.Collapsed;
                }
                else if (Common.NoDCSBIOSEnabled())
                {
                    LabelDCSBIOS.Visibility = Visibility.Collapsed;
                }
                StackPanelGeneralSettings.Visibility = Visibility.Visible;
                StackPanelDCSBIOSSettings.Visibility = Visibility.Collapsed;
                StackPanelSRSSettings.Visibility = Visibility.Collapsed;
                SetEventsHandlers();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + Environment.NewLine + exception.StackTrace);
            }
        }

        private void SetEventsHandlers()
        {
            RadioButtonBelowNormal.Checked += GeneralDirty;
            RadioButtonNormal.Checked += GeneralDirty;
            RadioButtonAboveNormal.Checked += GeneralDirty;
            RadioButtonHigh.Checked += GeneralDirty;
            RadioButtonRealtime.Checked += GeneralDirty;
            RadioButtonKeyBd.Checked += GeneralDirty;
            RadioButtonSendInput.Checked += GeneralDirty;
            CheckBoxMinimizeToTray.Checked += GeneralDirty;
            CheckBoxMinimizeToTray.Unchecked += GeneralDirty;
            CheckBoxEnablePluginSupport.Checked += GeneralDirty;
            CheckBoxEnablePluginSupport.Unchecked += GeneralDirty;
            CheckBoxDisableKeyboardAPI.Checked += GeneralDirty;
            CheckBoxDisableKeyboardAPI.Unchecked += GeneralDirty;

            TextBoxDcsBiosJSONLocation.TextChanged += DcsBiosDirty;
            TextBoxDCSBIOSFromIP.TextChanged += DcsBiosDirty;
            TextBoxDCSBIOSToIP.TextChanged += DcsBiosDirty;
            TextBoxDCSBIOSFromPort.TextChanged += DcsBiosDirty;
            TextBoxDCSBIOSToPort.TextChanged += DcsBiosDirty;
            
            TextBoxSRSFromIP.TextChanged += SrsDirty;
            TextBoxSRSToIP.TextChanged += SrsDirty;
            TextBoxSRSFromPort.TextChanged += SrsDirty;
            TextBoxSRSToPort.TextChanged += SrsDirty;
        }

        private void LoadSettings()
        {
            switch (Settings.Default.ProcessPriority)
            {
                case ProcessPriorityClass.BelowNormal:
                    {
                        RadioButtonBelowNormal.IsChecked = true;
                        break;
                    }
                case ProcessPriorityClass.Normal:
                    {
                        RadioButtonNormal.IsChecked = true;
                        break;
                    }
                case ProcessPriorityClass.AboveNormal:
                    {
                        RadioButtonAboveNormal.IsChecked = true;
                        break;
                    }
                case ProcessPriorityClass.High:
                    {
                        RadioButtonHigh.IsChecked = true;
                        break;
                    }
                case ProcessPriorityClass.RealTime:
                    {
                        RadioButtonRealtime.IsChecked = true;
                        break;
                    }
            }
            if (Settings.Default.APIMode == 0)
            {
                RadioButtonKeyBd.IsChecked = true;
            }
            else
            {
                RadioButtonSendInput.IsChecked = true;
            }

            CheckBoxMinimizeToTray.IsChecked = Settings.Default.RunMinimized;
            CheckBoxEnablePluginSupport.IsChecked = Settings.Default.EnablePlugin;
            CheckBoxDisableKeyboardAPI.IsChecked = Settings.Default.DisableKeyboardAPI;

            if (Common.PartialDCSBIOSEnabled())
            {
                TextBoxDcsBiosJSONLocation.Text = Settings.Default.DCSBiosJSONLocation;
                TextBoxDCSBIOSFromIP.Text = Settings.Default.DCSBiosIPFrom;
                TextBoxDCSBIOSToIP.Text = Settings.Default.DCSBiosIPTo;
                TextBoxDCSBIOSFromPort.Text = Settings.Default.DCSBiosPortFrom;
                TextBoxDCSBIOSToPort.Text = Settings.Default.DCSBiosPortTo;
            }
            if (Common.IsEmulationModesFlagSet(EmulationMode.SRSEnabled))
            {
                TextBoxSRSFromIP.Text = Settings.Default.SRSIpFrom;
                TextBoxSRSToIP.Text = Settings.Default.SRSIpTo;
                TextBoxSRSFromPort.Text = Settings.Default.SRSPortFrom.ToString();
                TextBoxSRSToPort.Text = Settings.Default.SRSPortTo.ToString();
            }
        }

        private void GeneralSettings_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            CollapseAllSettingsPanelExcept(StackPanelGeneralSettings);
        }

        private void DCSBIOS_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            CollapseAllSettingsPanelExcept(StackPanelDCSBIOSSettings);
        }

        private void SRS_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            CollapseAllSettingsPanelExcept(StackPanelSRSSettings);
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckValuesDCSBIOS();
                CheckValuesSRS();

                if (GeneralChanged)
                {
                    if (RadioButtonKeyBd.IsChecked == true)
                    {
                        Settings.Default.APIMode = 0;
                        Common.APIMode = APIModeEnum.keybd_event;
                        Settings.Default.Save();
                    }
                    if (RadioButtonSendInput.IsChecked == true)
                    {
                        Settings.Default.APIMode = 1;
                        Common.APIMode = APIModeEnum.SendInput;
                        Settings.Default.Save();
                    }
                    if (RadioButtonBelowNormal.IsChecked == true)
                    {
                        Settings.Default.ProcessPriority = ProcessPriorityClass.BelowNormal;
                        Settings.Default.Save();
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
                    }
                    if (RadioButtonNormal.IsChecked == true)
                    {
                        Settings.Default.ProcessPriority = ProcessPriorityClass.Normal;
                        Settings.Default.Save();
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
                    }
                    if (RadioButtonAboveNormal.IsChecked == true)
                    {
                        Settings.Default.ProcessPriority = ProcessPriorityClass.AboveNormal;
                        Settings.Default.Save();
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
                    }
                    if (RadioButtonHigh.IsChecked == true)
                    {
                        Settings.Default.ProcessPriority = ProcessPriorityClass.High;
                        Settings.Default.Save();
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
                    }
                    if (RadioButtonRealtime.IsChecked == true)
                    {
                        Settings.Default.ProcessPriority = ProcessPriorityClass.RealTime;
                        Settings.Default.Save();
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
                    }

                    Settings.Default.Save();

                    Settings.Default.RunMinimized = CheckBoxMinimizeToTray.IsChecked == true;
                    Settings.Default.EnablePlugin = CheckBoxEnablePluginSupport.IsChecked == true;
                    Settings.Default.DisableKeyboardAPI = this.CheckBoxDisableKeyboardAPI.IsChecked == true;
                    Settings.Default.Save();
                }

                if (DCSBIOSChanged)
                {
                    Settings.Default.DCSBiosJSONLocation = TextBoxDcsBiosJSONLocation.Text;
                    Settings.Default.DCSBiosIPFrom = IpAddressFromDCSBIOS;
                    Settings.Default.DCSBiosPortFrom = PortFromDCSBIOS;
                    Settings.Default.DCSBiosIPTo = IpAddressToDCSBIOS;
                    Settings.Default.DCSBiosPortTo = PortToDCSBIOS;
                    Settings.Default.Save();
                }

                if (SRSChanged)
                {
                    Settings.Default.SRSIpFrom = IpAddressFromSRS;
                    Settings.Default.SRSPortFrom = int.Parse(PortFromSRS);
                    Settings.Default.SRSIpTo = IpAddressToSRS;
                    Settings.Default.SRSPortTo = int.Parse(PortToSRS);
                    Settings.Default.Save();
                }

                if (StreamDeckChanged)
                {
                    Settings.Default.Save();
                }
                DialogResult = true;
                Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception.Message}{Environment.NewLine}{exception.StackTrace}");
            }
        }

        private void ButtonBrowse_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.ShowNewFolderButton = false;
                if (!string.IsNullOrEmpty(DCS_BIOS.DBCommon.GetDCSBIOSJSONDirectory(Settings.Default.DCSBiosJSONLocation)))
                {
                    folderBrowserDialog.SelectedPath = DBCommon.GetDCSBIOSJSONDirectory(Settings.Default.DCSBiosJSONLocation);
                }

                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (CheckJSONDirectory(folderBrowserDialog.SelectedPath))
                    {
                        TextBoxDcsBiosJSONLocation.Text = folderBrowserDialog.SelectedPath;
                    }
                    else
                    {
                        MessageBox.Show("Cannot use selected directory as it did not contain json files.", "Invalid directory", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private bool CheckJSONDirectory(string jsonDirectory)
        {
            jsonDirectory = DBCommon.GetDCSBIOSJSONDirectory(jsonDirectory);

            if (string.IsNullOrEmpty(jsonDirectory) || !Directory.Exists(jsonDirectory))
            {
                return false;
            }

            var files = Directory.EnumerateFiles(jsonDirectory);

            foreach (var filename in files)
            {
                if (filename.ToLower().EndsWith(".json"))
                {
                    return true;
                }
            }

            return false;
        }

        private void CheckValuesDCSBIOS()
        {
            try
            {
                IPAddress ipAddress;
                if (string.IsNullOrEmpty(TextBoxDCSBIOSFromIP.Text))
                {
                    throw new Exception("DCS-BIOS IP address from cannot be empty");
                }
                if (string.IsNullOrEmpty(TextBoxDCSBIOSToIP.Text))
                {
                    throw new Exception("DCS-BIOS IP address to cannot be empty");
                }
                if (string.IsNullOrEmpty(TextBoxDCSBIOSFromPort.Text))
                {
                    throw new Exception("DCS-BIOS Port from cannot be empty");
                }
                if (string.IsNullOrEmpty(TextBoxDCSBIOSToPort.Text))
                {
                    throw new Exception("DCS-BIOS Port to cannot be empty");
                }
                if (string.IsNullOrEmpty(TextBoxDcsBiosJSONLocation.Text))
                {
                    throw new Exception("DCS-BIOS JSON directory cannot be empty");
                }
                try
                {
                    if (!IPAddress.TryParse(TextBoxDCSBIOSFromIP.Text, out ipAddress))
                    {
                        throw new Exception();
                    }
                    IpAddressFromDCSBIOS = TextBoxDCSBIOSFromIP.Text;
                }
                catch (Exception e)
                {
                    throw new Exception("DCS-BIOS Error while checking IP from : " + e.Message);
                }
                try
                {
                    if (!IPAddress.TryParse(TextBoxDCSBIOSToIP.Text, out ipAddress))
                    {
                        throw new Exception();
                    }
                    IpAddressToDCSBIOS = TextBoxDCSBIOSToIP.Text;
                }
                catch (Exception e)
                {
                    throw new Exception("DCS-BIOS Error while checking IP to : " + e.Message);
                }
                try
                {
                    var test = Convert.ToInt32(TextBoxDCSBIOSFromPort.Text);
                    PortFromDCSBIOS = TextBoxDCSBIOSFromPort.Text;
                }
                catch (Exception e)
                {
                    throw new Exception("DCS-BIOS Error while Port from : " + e.Message);
                }
                try
                {
                    var test = Convert.ToInt32(TextBoxDCSBIOSFromPort.Text);
                    PortToDCSBIOS = TextBoxDCSBIOSToPort.Text;
                }
                catch (Exception e)
                {
                    throw new Exception("DCS-BIOS Error while Port to : " + e.Message);
                }
                try
                {
                    var directoryInfo = new DirectoryInfo(TextBoxDcsBiosJSONLocation.Text);
                    DcsBiosJSONLocation = TextBoxDcsBiosJSONLocation.Text;
                }
                catch (Exception e)
                {
                    throw new Exception("DCS-BIOS Error while checking DCS-BIOS location : " + e.Message);
                }
            }
            catch (Exception e)
            {
                throw new Exception("DCS-BIOS Error checking values : " + Environment.NewLine + e.Message);
            }
        }

        private void CheckValuesSRS()
        {
            try
            {
                IPAddress ipAddress;
                if (string.IsNullOrEmpty(TextBoxSRSFromIP.Text))
                {
                    throw new Exception("SRS IP address from cannot be empty");
                }
                if (string.IsNullOrEmpty(TextBoxSRSToIP.Text))
                {
                    throw new Exception("SRS IP address to cannot be empty");
                }
                if (string.IsNullOrEmpty(TextBoxSRSFromPort.Text))
                {
                    throw new Exception("SRS Port from cannot be empty");
                }
                if (string.IsNullOrEmpty(TextBoxSRSToPort.Text))
                {
                    throw new Exception("SRS Port to cannot be empty");
                }
                try
                {
                    if (!IPAddress.TryParse(TextBoxSRSFromIP.Text, out ipAddress))
                    {
                        throw new Exception();
                    }
                    IpAddressFromSRS = TextBoxSRSFromIP.Text;
                }
                catch (Exception e)
                {
                    throw new Exception("SRS Error while checking IP from : " + e.Message);
                }
                try
                {
                    if (!IPAddress.TryParse(TextBoxSRSToIP.Text, out ipAddress))
                    {
                        throw new Exception();
                    }
                    IpAddressToSRS = TextBoxSRSToIP.Text;
                }
                catch (Exception e)
                {
                    throw new Exception("SRS Error while checking IP to : " + e.Message);
                }
                try
                {
                    var test = Convert.ToInt32(TextBoxSRSFromPort.Text);
                    PortFromSRS = TextBoxSRSFromPort.Text;
                }
                catch (Exception e)
                {
                    throw new Exception("SRS Error while Port from : " + e.Message);
                }
                try
                {
                    var test = Convert.ToInt32(TextBoxSRSFromPort.Text);
                    PortToSRS = TextBoxSRSToPort.Text;
                }
                catch (Exception e)
                {
                    throw new Exception("SRS Error while Port to : " + e.Message);
                }
            }
            catch (Exception e)
            {
                throw new Exception("SRS Error checking values : " + Environment.NewLine + e.Message);
            }
        }

        private void DcsBiosDirty(object sender, TextChangedEventArgs e)
        {
            DCSBIOSChanged = true;
            ButtonOk.IsEnabled = true;
        }

        private void SrsDirty(object sender, TextChangedEventArgs e)
        {
            SRSChanged = true;
            ButtonOk.IsEnabled = true;
        }

        private void SettingsWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!ButtonOk.IsEnabled && e.Key == Key.Escape)
            {
                DialogResult = false;
                e.Handled = true;
                Close();
            }
        }

        private void GeneralDirty(object sender, RoutedEventArgs e)
        {
            GeneralChanged = true;
            ButtonOk.IsEnabled = true;
        }

        private void CollapseAllSettingsPanelExcept(StackPanel visibleStackPanel)
        {
            StackPanelGeneralSettings.Visibility = Visibility.Collapsed;
            StackPanelDCSBIOSSettings.Visibility = Visibility.Collapsed;
            StackPanelSRSSettings.Visibility = Visibility.Collapsed;
        
            visibleStackPanel.Visibility = Visibility.Visible;
        }
    }
}
