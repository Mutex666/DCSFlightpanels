﻿namespace DCSFlightpanels.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using ClassLibraryCommon;

    using DCS_BIOS;

    using EnumEx = ClassLibraryCommon.EnumEx;

    /// <summary>
    /// Interaction logic for DCSBiosOutputTriggerWindow.xaml
    /// </summary>
    public partial class DCSBiosOutputTriggerWindow
    {
        private readonly bool _showCriteria;
        private readonly IEnumerable<DCSBIOSControl> _dcsbiosControls;
        private readonly string _description;
        private DCSBIOSOutput _dcsBiosOutput;
        private bool _formLoaded;
        private DCSBIOSControl _dcsbiosControl;
        private DCSFPProfile _dcsfpProfile;
        private Popup _popupSearch;
        private DataGrid _dataGridValues;

        public DCSBiosOutputTriggerWindow(DCSFPProfile dcsfpProfile, string description, bool showCriteria = true)
        {
            InitializeComponent();
            _showCriteria = showCriteria;
            _dcsfpProfile = dcsfpProfile;
            _description = description;
            _dcsBiosOutput = new DCSBIOSOutput();
            DCSBIOSControlLocator.LoadControls();
            _dcsbiosControls = DCSBIOSControlLocator.GetIntegerOutputControls();
        }

        public DCSBiosOutputTriggerWindow(DCSFPProfile dcsfpProfile, string description, DCSBIOSOutput dcsBiosOutput, bool showCriteria = true)
        {
            InitializeComponent();
            _showCriteria = showCriteria;
            _dcsfpProfile = dcsfpProfile;
            _description = description;
            _dcsBiosOutput = dcsBiosOutput;
            DCSBIOSControlLocator.LoadControls();
            _dcsbiosControl = DCSBIOSControlLocator.GetControl(_dcsBiosOutput.ControlId);
            _dcsbiosControls = DCSBIOSControlLocator.GetIntegerOutputControls();

        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _popupSearch = (Popup)FindResource("PopUpSearchResults");
                _popupSearch.Height = 400;
                _dataGridValues = ((DataGrid)LogicalTreeHelper.FindLogicalNode(_popupSearch, "DataGridValues"));
                LabelDescription.Content = _description;
                ShowValues1();
                ShowValues2();
                _formLoaded = true;
                SetFormState();
                TextBoxSearchWord.Focus();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox( ex);
            }
        }




        private void ShowValues1()
        {
            ComboBoxComparisonCriteria.SelectedValue = _dcsBiosOutput.DCSBiosOutputComparison.GetDescriptionField();
            if (_dcsBiosOutput.DCSBiosOutputType == DCSBiosOutputType.INTEGER_TYPE)
            {
                TextBoxTriggerValue.Text = _dcsBiosOutput.SpecifiedValueInt.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                TextBoxTriggerValue.Text = _dcsBiosOutput.SpecifiedValueString;
            }
        }

        private void ShowValues2()
        {
            if (_dcsbiosControl != null)
            {
                TextBoxControlId.Text = _dcsbiosControl.identifier;
                TextBoxControlDescription.Text = _dcsbiosControl.description;
                TextBoxMaxValue.Text = _dcsbiosControl.outputs[0].max_value.ToString();
                TextBoxOutputType.Text = _dcsbiosControl.outputs[0].type;
            }
        }

        private void SetFormState()
        {
            if (!_formLoaded)
            {
                return;
            }


            ComboBoxComparisonCriteria.IsEnabled = _dcsbiosControl != null && _dcsbiosControl.outputs.Count == 1 && _dcsbiosControl.outputs[0].OutputDataType == DCSBiosOutputType.INTEGER_TYPE;

            LabelCriteria.Visibility = _showCriteria ? Visibility.Visible : Visibility.Collapsed;
            ComboBoxComparisonCriteria.Visibility = _showCriteria ? Visibility.Visible : Visibility.Collapsed;
            LabelTriggerValue.Visibility = _showCriteria ? Visibility.Visible : Visibility.Collapsed;
            TextBoxTriggerValue.Visibility = _showCriteria ? Visibility.Visible : Visibility.Collapsed;


            if (ComboBoxComparisonCriteria.IsEnabled && _dcsBiosOutput != null)
            {
                ComboBoxComparisonCriteria.SelectedValue = _dcsBiosOutput.DCSBiosOutputComparison.GetDescription();
            }
            if (TextBoxTriggerValue != null)
            {
                //todo
                //TextBoxTriggerValue.IsEnabled = _dcsBiosOutput != null;
            }
        }

        private void CopyValues()
        {
            try
            {
                //This is were DCSBiosOutput (subset of DCSBIOSControl) get populated from DCSBIOSControl
                if (string.IsNullOrEmpty(TextBoxTriggerValue.Text))
                {
                    throw new Exception("Value cannot be empty");
                }
                if (ComboBoxComparisonCriteria.SelectedValue == null)
                {
                    throw new Exception("Comparison criteria cannot be empty");
                }
                if (_dcsbiosControl == null)
                {
                    _dcsbiosControl = DCSBIOSControlLocator.GetControl(TextBoxControlId.Text);
                }
                _dcsBiosOutput.Consume(_dcsbiosControl);
                if (!_showCriteria)
                {
                    //Value isn't used anyways
                    _dcsBiosOutput.DCSBiosOutputComparison = DCSBiosOutputComparison.Equals;
                }
                else
                {
                    _dcsBiosOutput.DCSBiosOutputComparison = EnumEx.GetValueFromDescription<DCSBiosOutputComparison>(ComboBoxComparisonCriteria.SelectedValue.ToString());
                }
                try
                {
                    if (_showCriteria)
                    {
                        //Assume only Integer trigger values can be used. That is how it should be!!
                        try
                        {
                            var f = int.Parse(TextBoxTriggerValue.Text);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Error while checking Trigger value. Only integers are allowed : " + e.Message);
                        }
                        if (_dcsBiosOutput.DCSBiosOutputType == DCSBiosOutputType.INTEGER_TYPE)
                        {
                            _dcsBiosOutput.SpecifiedValueInt = (uint)Convert.ToInt32(TextBoxTriggerValue.Text);
                        }
                        else
                        {
                            throw new Exception("Error, DCSBIOSOutput can only have a Integer type output. This has String : " + _dcsBiosOutput.ControlId);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Error while checking Value format : " + e.Message);
                }
            }
            catch (Exception e)
            {
                Common.ShowErrorMessageBox( e, "Error in CopyValues() : ");
            }
        }

        private void ClearAll()
        {
            _dcsbiosControl = null;
            _dcsBiosOutput = null;
        }

        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            try
            {
                CopyValues();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox( ex);
            }
        }

        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearAll();
                DialogResult = false;
                Close();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox( ex);
            }
        }

        public DCSBIOSOutput DCSBiosOutput
        {
            get { return _dcsBiosOutput; }
            set { _dcsBiosOutput = value; }
        }

        private void AdjustShownPopupData()
        {
            _popupSearch.PlacementTarget = TextBoxSearchWord;
            _popupSearch.Placement = PlacementMode.Bottom;
            if (!_popupSearch.IsOpen)
            {
                _popupSearch.IsOpen = true;
            }
            if (_dataGridValues != null)
            {
                if (string.IsNullOrEmpty(TextBoxSearchWord.Text))
                {
                    _dataGridValues.DataContext = _dcsbiosControls;
                    _dataGridValues.ItemsSource = _dcsbiosControls;
                    _dataGridValues.Items.Refresh();
                    return;
                }
                var subList = _dcsbiosControls.Where(controlObject => (!string.IsNullOrWhiteSpace(controlObject.identifier) && controlObject.identifier.ToUpper().Contains(TextBoxSearchWord.Text.ToUpper()))
                    || (!string.IsNullOrWhiteSpace(controlObject.description) && controlObject.description.ToUpper().Contains(TextBoxSearchWord.Text.ToUpper())));
                _dataGridValues.DataContext = subList;
                _dataGridValues.ItemsSource = subList;
                _dataGridValues.Items.Refresh();
            }
        }

        private void TextBoxSearchWord_OnKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                AdjustShownPopupData();
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox( ex);
            }
        }

        private void TextBoxSearchWord_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (TextBoxSearchWord.Text == string.Empty)
                {
                    // Create an ImageBrush.
                    var textImageBrush = new ImageBrush();
                    textImageBrush.ImageSource =
                        new BitmapImage(new Uri("pack://application:,,,/dcsfp;component/Images/cue_banner_search_dcsbios.png", UriKind.RelativeOrAbsolute));
                    textImageBrush.AlignmentX = AlignmentX.Left;
                    textImageBrush.Stretch = Stretch.Uniform;

                    // Use the brush to paint the button's background.
                    TextBoxSearchWord.Background = textImageBrush;
                }
                else
                {
                    TextBoxSearchWord.Background = null;
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (_dataGridValues.SelectedItems.Count == 1)
                {
                    _dcsbiosControl = (DCSBIOSControl)_dataGridValues.SelectedItem;
                    _dcsBiosOutput = new DCSBIOSOutput();
                    _dcsBiosOutput.Consume(_dcsbiosControl);
                    ShowValues2();
                    if (_dcsBiosOutput.DCSBiosOutputType == DCSBiosOutputType.INTEGER_TYPE)
                    {
                        TextBoxTriggerValue.Text = "0";
                    }
                    else if (_dcsBiosOutput.DCSBiosOutputType == DCSBiosOutputType.STRING_TYPE)
                    {
                        TextBoxTriggerValue.Text = "<string>";
                    }
                }
                _popupSearch.IsOpen = false;
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox( ex);
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (_dataGridValues.SelectedItems.Count == 1)
                {
                    _dcsbiosControl = (DCSBIOSControl)_dataGridValues.SelectedItem;
                    _dcsBiosOutput = new DCSBIOSOutput();
                    _dcsBiosOutput.Consume(_dcsbiosControl);
                    ShowValues2();
                }
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox( ex);
            }
        }


        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (_dataGridValues.SelectedItems.Count == 1)
                {
                    _dcsbiosControl = (DCSBIOSControl)_dataGridValues.SelectedItem;
                    _dcsBiosOutput = new DCSBIOSOutput();
                    _dcsBiosOutput.Consume(_dcsbiosControl);
                    ShowValues2();
                    if (_dcsBiosOutput.DCSBiosOutputType == DCSBiosOutputType.INTEGER_TYPE)
                    {
                        TextBoxTriggerValue.Text = "0";
                    }
                    else if (_dcsBiosOutput.DCSBiosOutputType == DCSBiosOutputType.STRING_TYPE)
                    {
                        TextBoxTriggerValue.Text = "<string>";
                    }
                }
                _popupSearch.IsOpen = false;
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox( ex);
            }
        }

        private void DCSBiosOutputTriggerWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!ButtonOk.IsEnabled && e.Key == Key.Escape)
            {
                DialogResult = false;
                e.Handled = true;
                Close();
            }
        }

    }
}
