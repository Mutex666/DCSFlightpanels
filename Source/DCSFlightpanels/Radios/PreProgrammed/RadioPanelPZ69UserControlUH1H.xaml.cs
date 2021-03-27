﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ClassLibraryCommon;
using DCSFlightpanels.Interfaces;
using DCSFlightpanels.PanelUserControls;
using NonVisuals;
using DCSFlightpanels.Properties;
using NonVisuals.Interfaces;
using NonVisuals.Radios;
using NonVisuals.Radios.Knobs;
using NonVisuals.Saitek;
using NonVisuals.Saitek.Panels;


namespace DCSFlightpanels.Radios.PreProgrammed
{
    /// <summary>
    /// Interaction logic for RadioPanelPZ69UserControlUH1H.xaml
    /// </summary>
    public partial class RadioPanelPZ69UserControlUH1H : UserControlBase, IGamingPanelListener, IProfileHandlerListener, IGamingPanelUserControl
    {
        private readonly RadioPanelPZ69UH1H _radioPanelPZ69;

        public RadioPanelPZ69UserControlUH1H(HIDSkeleton hidSkeleton, TabItem parentTabItem, IGlobalHandler globalHandler)
        {
            InitializeComponent();

            ParentTabItem = parentTabItem;

            hidSkeleton.HIDReadDevice.Removed += DeviceRemovedHandler;

            HideAllImages();
            _radioPanelPZ69 = new RadioPanelPZ69UH1H(hidSkeleton);
            _radioPanelPZ69.FrequencyKnobSensitivity = Settings.Default.RadioFrequencyKnobSensitivity;
            _radioPanelPZ69.Attach((IGamingPanelListener)this);
            globalHandler.Attach(_radioPanelPZ69);
            GlobalHandler = globalHandler;

            //LoadConfiguration();
        }

        public void BipPanelRegisterEvent(object sender, BipPanelRegisteredEventArgs e)
        {
        }

        public override GamingPanel GetGamingPanel()
        {
            return _radioPanelPZ69;
        }

        public override GamingPanelEnum GetPanelType()
        {
            return GamingPanelEnum.PZ69RadioPanel;
        }

        public string GetName()
        {
            return GetType().Name;
        }

        public void UpdatesHasBeenMissed(object sender, DCSBIOSUpdatesMissedEventArgs e) { }

        public void SelectedProfile(object sender, AirframeEventArgs e) { }

        public void UISwitchesChanged(object sender, SwitchesChangedEventArgs e)
        {
            try
            {
                SetGraphicsState(e.Switches);
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox( ex);
            }
        }

        public void PanelBindingReadFromFile(object sender, PanelBindingReadFromFileEventArgs e){}

        public void SettingsCleared(object sender, PanelEventArgs e) { }

        public void LedLightChanged(object sender, LedLightChangeEventArgs e) { }

        public void PanelDataAvailable(object sender, PanelDataToDCSBIOSEventEventArgs e) { }

        public void DeviceAttached(object sender, PanelEventArgs e) { }

        public void SettingsApplied(object sender, PanelEventArgs e) { }

        public void PanelSettingsChanged(object sender, PanelEventArgs e) { }

        public void DeviceDetached(object sender, PanelEventArgs e) { }

        private void SetGraphicsState(HashSet<object> knobs)
        {
            try
            {
                foreach (var radioKnobO in knobs)
                {
                    var radioKnob = (RadioPanelKnobUH1H)radioKnobO;
                    Dispatcher?.BeginInvoke((Action)delegate
                    {
                        /*if (radioKnob.IsOn)
                        {
                            SetGroupboxVisibility(radioKnob.RadioPanelPZ69Knob);
                        }*/
                    });
                    switch (radioKnob.RadioPanelPZ69Knob)
                    {
                        case RadioPanelPZ69KnobsUH1H.UPPER_VHFCOMM:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        TopLeftCom1.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.UPPER_UHF:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        TopLeftCom2.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.UPPER_VHFNAV:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        TopLeftNav1.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.UPPER_VHFFM:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        TopLeftNav2.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.UPPER_ADF:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        TopLeftADF.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.UPPER_DME:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        TopLeftDME.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.UPPER_INTERCOMM:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        TopLeftXPDR.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.LOWER_VHFCOMM:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        LowerLeftCom1.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.LOWER_UHF:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        LowerLeftCom2.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.LOWER_VHFNAV:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        LowerLeftNav1.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.LOWER_VHFFM:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        LowerLeftNav2.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.LOWER_ADF:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        LowerLeftADF.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.LOWER_DME:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        LowerLeftDME.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.LOWER_INTERCOMM:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        LowerLeftXPDR.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.UPPER_SMALL_FREQ_WHEEL_INC:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        UpperSmallerLCDKnobInc.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.UPPER_SMALL_FREQ_WHEEL_DEC:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        UpperSmallerLCDKnobDec.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.UPPER_LARGE_FREQ_WHEEL_INC:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        UpperLargerLCDKnobInc.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.UPPER_LARGE_FREQ_WHEEL_DEC:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        UpperLargerLCDKnobDec.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.LOWER_SMALL_FREQ_WHEEL_INC:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        LowerSmallerLCDKnobInc.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.LOWER_SMALL_FREQ_WHEEL_DEC:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        LowerSmallerLCDKnobDec.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.LOWER_LARGE_FREQ_WHEEL_INC:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        LowerLargerLCDKnobInc.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.LOWER_LARGE_FREQ_WHEEL_DEC:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        LowerLargerLCDKnobDec.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.UPPER_FREQ_SWITCH:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        UpperRightSwitch.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                        case RadioPanelPZ69KnobsUH1H.LOWER_FREQ_SWITCH:
                            {
                                var key = radioKnob;
                                Dispatcher?.BeginInvoke(
                                    (Action)delegate
                                    {
                                        LowerRightSwitch.Visibility = key.IsOn ? Visibility.Visible : Visibility.Collapsed;
                                    });
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox( ex);
            }
        }
        /*
        private void SetGroupboxVisibility(RadioPanelPZ69KnobsUH1H panelPZ69KnobsUH1H)
        {
            try
            {
                GroupLowerSelectorKnobUHF.Visibility = panelPZ69KnobsUH1H == RadioPanelPZ69KnobsUH1H.UPPER_UHF ? Visibility.Visible : Visibility.Hidden;
                GroupLowerSelectorKnobVHFFM.Visibility = panelPZ69KnobsUH1H == RadioPanelPZ69KnobsUH1H.UPPER_VHFFM ? Visibility.Visible : Visibility.Hidden;
                GroupLowerSelectorKnobVHFAM.Visibility = panelPZ69KnobsUH1H == RadioPanelPZ69KnobsUH1H.UPPER_VHFAM ? Visibility.Visible : Visibility.Hidden;
                GroupLowerSelectorKnobTACAN.Visibility = panelPZ69KnobsUH1H == RadioPanelPZ69KnobsUH1H.UPPER_TACAN ? Visibility.Visible : Visibility.Hidden;
                GroupLowerSelectorKnobILS.Visibility = panelPZ69KnobsUH1H == RadioPanelPZ69KnobsUH1H.UPPER_ILS ? Visibility.Visible : Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(129814, ex);
            }
        }
        */
        private void HideAllImages()
        {
            TopLeftCom1.Visibility = Visibility.Collapsed;
            TopLeftCom2.Visibility = Visibility.Collapsed;
            TopLeftNav1.Visibility = Visibility.Collapsed;
            TopLeftNav2.Visibility = Visibility.Collapsed;
            TopLeftADF.Visibility = Visibility.Collapsed;
            TopLeftDME.Visibility = Visibility.Collapsed;
            TopLeftXPDR.Visibility = Visibility.Collapsed;
            LowerLeftCom1.Visibility = Visibility.Collapsed;
            LowerLeftCom2.Visibility = Visibility.Collapsed;
            LowerLeftNav1.Visibility = Visibility.Collapsed;
            LowerLeftNav2.Visibility = Visibility.Collapsed;
            LowerLeftADF.Visibility = Visibility.Collapsed;
            LowerLeftDME.Visibility = Visibility.Collapsed;
            LowerLeftXPDR.Visibility = Visibility.Collapsed;
            LowerLargerLCDKnobDec.Visibility = Visibility.Collapsed;
            UpperLargerLCDKnobInc.Visibility = Visibility.Collapsed;
            UpperRightSwitch.Visibility = Visibility.Collapsed;
            UpperSmallerLCDKnobDec.Visibility = Visibility.Collapsed;
            UpperSmallerLCDKnobInc.Visibility = Visibility.Collapsed;
            UpperLargerLCDKnobDec.Visibility = Visibility.Collapsed;
            LowerLargerLCDKnobInc.Visibility = Visibility.Collapsed;
            LowerRightSwitch.Visibility = Visibility.Collapsed;
            LowerSmallerLCDKnobDec.Visibility = Visibility.Collapsed;
            LowerSmallerLCDKnobInc.Visibility = Visibility.Collapsed;
        }

        private void ButtonGetId_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_radioPanelPZ69 != null)
                {
                    TextBoxLogPZ69.Text = "";
                    TextBoxLogPZ69.Text = _radioPanelPZ69.HIDInstanceId;
                    Clipboard.SetText(_radioPanelPZ69.HIDInstanceId);
                    MessageBox.Show("The Instance Id for the panel has been copied to the Clipboard.");
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox( ex);
            }
        }

        private void ComboBoxFreqKnobSensitivity_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (UserControlLoaded)
                {
                    Settings.Default.RadioFrequencyKnobSensitivity = int.Parse(ComboBoxFreqKnobSensitivity.SelectedValue.ToString());
                    _radioPanelPZ69.FrequencyKnobSensitivity = int.Parse(ComboBoxFreqKnobSensitivity.SelectedValue.ToString());
                    Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox( ex);
            }
        }

        private void RadioPanelPZ69UserControlUH1H_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboBoxFreqKnobSensitivity.SelectedValue = Settings.Default.RadioFrequencyKnobSensitivity;
                ComboBoxSyncOKDelayTimeout.SelectedValue = Settings.Default.SyncOKDelayTimeout;
                UserControlLoaded = true;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox( ex);
            }
        }

        private void ComboBoxSyncOKDelayTimeout_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (UserControlLoaded)
                {
                    Settings.Default.SyncOKDelayTimeout = int.Parse(ComboBoxSyncOKDelayTimeout.SelectedValue.ToString());
                    _radioPanelPZ69.SyncOKDelayTimeout = int.Parse(ComboBoxSyncOKDelayTimeout.SelectedValue.ToString());
                    Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox( ex);
            }
        }

        private void ButtonGetIdentify_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _radioPanelPZ69.Identify();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }
    }
}