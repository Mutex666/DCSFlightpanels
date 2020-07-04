﻿using System;
using System.Windows.Controls;
using ClassLibraryCommon;
using DCSFlightpanels.CustomControls;
using DCSFlightpanels.Windows;
using NonVisuals;
using NonVisuals.Interfaces;

namespace DCSFlightpanels.PanelUserControls
{
    public class UserControlBase : UserControl, IDisposable
    {
        private IGlobalHandler _globalHandler;
        private TabItem _parentTabItem;
        private string _parentTabItemHeader;
        private bool _userControlLoaded;
        

        public UserControlBase()
        {
            
        }
        
        public void DeviceRemovedHandler()
        {
            GlobalHandler.Detach(GetGamingPanel());
        }

        public virtual GamingPanel GetGamingPanel()
        {
            return null;
        }

        public IGlobalHandler GlobalHandler
        {
            set => _globalHandler = value;
            get => _globalHandler;
        }

        protected virtual void Dispose(bool disposing)
        { }

        public void Dispose()
        {
            Dispose(true);
        }

        public TabItem ParentTabItem
        {
            get => _parentTabItem;
            set
            {
                _parentTabItem = value;
                if (_parentTabItem != null && _parentTabItem.Header != null)
                {
                    ParentTabItemHeader = ParentTabItem.Header.ToString();
                }
            }
        }

        public string ParentTabItemHeader
        {
            get => _parentTabItemHeader;
            set => _parentTabItemHeader = value;
        }

        public bool UserControlLoaded
        {
            get => _userControlLoaded;
            set => _userControlLoaded = value;
        }

        internal void CheckContextMenuItems(KeyPressLength keyPressLength, ContextMenu contextMenu)
        {
            try
            {
                foreach (MenuItem item in contextMenu.Items)
                {
                    item.IsChecked = false;
                }

                foreach (MenuItem item in contextMenu.Items)
                {
                    if (item.Name == "contextMenuItemKeepPressed" && keyPressLength == KeyPressLength.Indefinite)
                    {
                        item.IsChecked = true;
                    }
                    else if (item.Name == "contextMenuItemThirtyTwoMilliSec" && keyPressLength == KeyPressLength.ThirtyTwoMilliSec)
                    {
                        item.IsChecked = true;
                    }
                    else if (item.Name == "contextMenuItemFiftyMilliSec" && keyPressLength == KeyPressLength.FiftyMilliSec)
                    {
                        item.IsChecked = true;
                    }
                    else if (item.Name == "contextMenuItemHalfSecond" && keyPressLength == KeyPressLength.HalfSecond)
                    {
                        item.IsChecked = true;
                    }
                    else if (item.Name == "contextMenuItemSecond" && keyPressLength == KeyPressLength.Second)
                    {
                        item.IsChecked = true;
                    }
                    else if (item.Name == "contextMenuItemSecondAndHalf" && keyPressLength == KeyPressLength.SecondAndHalf)
                    {
                        item.IsChecked = true;
                    }
                    else if (item.Name == "contextMenuItemTwoSeconds" && keyPressLength == KeyPressLength.TwoSeconds)
                    {
                        item.IsChecked = true;
                    }
                    else if (item.Name == "contextMenuItemThreeSeconds" && keyPressLength == KeyPressLength.ThreeSeconds)
                    {
                        item.IsChecked = true;
                    }
                    else if (item.Name == "contextMenuItemFourSeconds" && keyPressLength == KeyPressLength.FourSeconds)
                    {
                        item.IsChecked = true;
                    }
                    else if (item.Name == "contextMenuItemFiveSecs" && keyPressLength == KeyPressLength.FiveSecs)
                    {
                        item.IsChecked = true;
                    }
                    else if (item.Name == "contextMenuItemFifteenSecs" && keyPressLength == KeyPressLength.FifteenSecs)
                    {
                        item.IsChecked = true;
                    }
                    else if (item.Name == "contextMenuItemTenSecs" && keyPressLength == KeyPressLength.TenSecs)
                    {
                        item.IsChecked = true;
                    }
                    else if (item.Name == "contextMenuItemTwentySecs" && keyPressLength == KeyPressLength.TwentySecs)
                    {
                        item.IsChecked = true;
                    }
                    else if (item.Name == "contextMenuItemThirtySecs" && keyPressLength == KeyPressLength.ThirtySecs)
                    {
                        item.IsChecked = true;
                    }
                    else if (item.Name == "contextMenuItemFortySecs" && keyPressLength == KeyPressLength.FortySecs)
                    {
                        item.IsChecked = true;
                    }
                    else if (item.Name == "contextMenuItemSixtySecs" && keyPressLength == KeyPressLength.SixtySecs)
                    {
                        item.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        internal void SetKeyPressLength(TextBoxBaseInput textBox, MenuItem contextMenuItem)
        {
            try
            {
                if (contextMenuItem.Name == "contextMenuItemKeepPressed")
                {
                    var message = "Remember to set a command for the opposing action!\n\n" +
                                  "For example if you set Keep Pressed for the \"On\" position for a button you need to set a command for \"Off\" position.\n" +
                                  "This way the continuous Keep Pressed will be canceled.\n" +
                                  "If you do not want a key press to cancel the continuous key press you can add a \"VK_NULL\" key.\n" +
                                  "\"VK_NULL\'s\" sole purpose is to cancel a continuous key press.";
                    var infoDialog = new InformationTextBlockWindow(message);
                    infoDialog.Height = 250;
                    infoDialog.ShowDialog();
                    textBox.Bill.KeyPress.SetLengthOfKeyPress(KeyPressLength.Indefinite);
                }
                else if (contextMenuItem.Name == "contextMenuItemThirtyTwoMilliSec")
                {
                    textBox.Bill.KeyPress.SetLengthOfKeyPress(KeyPressLength.ThirtyTwoMilliSec);
                }
                else if (contextMenuItem.Name == "contextMenuItemFiftyMilliSec")
                {
                    textBox.Bill.KeyPress.SetLengthOfKeyPress(KeyPressLength.FiftyMilliSec);
                }
                else if (contextMenuItem.Name == "contextMenuItemHalfSecond")
                {
                    textBox.Bill.KeyPress.SetLengthOfKeyPress(KeyPressLength.HalfSecond);
                }
                else if (contextMenuItem.Name == "contextMenuItemSecond")
                {
                    textBox.Bill.KeyPress.SetLengthOfKeyPress(KeyPressLength.Second);
                }
                else if (contextMenuItem.Name == "contextMenuItemSecondAndHalf")
                {
                    textBox.Bill.KeyPress.SetLengthOfKeyPress(KeyPressLength.SecondAndHalf);
                }
                else if (contextMenuItem.Name == "contextMenuItemTwoSeconds")
                {
                    textBox.Bill.KeyPress.SetLengthOfKeyPress(KeyPressLength.TwoSeconds);
                }
                else if (contextMenuItem.Name == "contextMenuItemThreeSeconds")
                {
                    textBox.Bill.KeyPress.SetLengthOfKeyPress(KeyPressLength.ThreeSeconds);
                }
                else if (contextMenuItem.Name == "contextMenuItemFourSeconds")
                {
                    textBox.Bill.KeyPress.SetLengthOfKeyPress(KeyPressLength.FourSeconds);
                }
                else if (contextMenuItem.Name == "contextMenuItemFiveSecs")
                {
                    textBox.Bill.KeyPress.SetLengthOfKeyPress(KeyPressLength.FiveSecs);
                }
                else if (contextMenuItem.Name == "contextMenuItemTenSecs")
                {
                    textBox.Bill.KeyPress.SetLengthOfKeyPress(KeyPressLength.TenSecs);
                }
                else if (contextMenuItem.Name == "contextMenuItemFifteenSecs")
                {
                    textBox.Bill.KeyPress.SetLengthOfKeyPress(KeyPressLength.FifteenSecs);
                }
                else if (contextMenuItem.Name == "contextMenuItemTwentySecs")
                {
                    textBox.Bill.KeyPress.SetLengthOfKeyPress(KeyPressLength.TwentySecs);
                }
                else if (contextMenuItem.Name == "contextMenuItemThirtySecs")
                {
                    textBox.Bill.KeyPress.SetLengthOfKeyPress(KeyPressLength.ThirtySecs);
                }
                else if (contextMenuItem.Name == "contextMenuItemFortySecs")
                {
                    textBox.Bill.KeyPress.SetLengthOfKeyPress(KeyPressLength.FortySecs);
                }
                else if (contextMenuItem.Name == "contextMenuItemSixtySecs")
                {
                    textBox.Bill.KeyPress.SetLengthOfKeyPress(KeyPressLength.SixtySecs);
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }
    }
}
