﻿using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using DCS_BIOS;
using NonVisuals.Saitek;
using NonVisuals.StreamDeck;

namespace DCSFlightpanels.Bills
{
    public class BillStreamDeckAction : BillBaseInput
    {
        private StreamDeckButtonOnOff _button;
        private ActionTypeDCSBIOS _dcsbiosBindingStreamDeck;
        private BIPLinkStreamDeck _bipLinkStreamDeck;
        private ActionTypeLayer _actionTypeLayer;

        public BillStreamDeckAction(TextBox textBox, StreamDeckButtonOnOff button)
        {
            TextBox = textBox;
            _button = button;
        }

        public override bool ContainsDCSBIOS()
        {
            return _dcsbiosBindingStreamDeck != null;// && _dcsbiosInputs.Count > 0;
        }

        public bool ContainsStreamDeckLayer()
        {
            return _actionTypeLayer != null;
        }

        public override bool ContainsBIPLink()
        {
            return _bipLinkStreamDeck != null && _bipLinkStreamDeck.BIPLights.Count > 0;
        }

        public override bool IsEmpty()
        {
            return (_bipLinkStreamDeck == null || _bipLinkStreamDeck.BIPLights.Count == 0) && 
                   (_dcsbiosBindingStreamDeck?.DCSBIOSInputs == null || _dcsbiosBindingStreamDeck.DCSBIOSInputs.Count == 0) && 
                   (KeyPress == null || KeyPress.KeySequence.Count == 0) &&
                   _actionTypeLayer == null;
        }

        public override void Consume(List<DCSBIOSInput> dcsBiosInputs)
        {
            if (_dcsbiosBindingStreamDeck == null)
            {
                _dcsbiosBindingStreamDeck = new ActionTypeDCSBIOS();
            }

            _dcsbiosBindingStreamDeck.DCSBIOSInputs = dcsBiosInputs;
        }

        public ActionTypeDCSBIOS DCSBIOSBinding
        {
            get => _dcsbiosBindingStreamDeck;
            set
            {
                if (ContainsKeyPress())
                {
                    throw new Exception("Cannot insert DCSBIOSInputs, Bill already contains KeyPress");
                }
                _dcsbiosBindingStreamDeck = value;
                if (_dcsbiosBindingStreamDeck != null)
                {
                    if (string.IsNullOrEmpty(_dcsbiosBindingStreamDeck.Description))
                    {
                        TextBox.Text = "DCS-BIOS";
                    }
                    else
                    {
                        TextBox.Text = _dcsbiosBindingStreamDeck.Description;
                    }
                }
                else
                {
                    TextBox.Text = "";
                }
            }
        }

        public BIPLinkStreamDeck BIPLink
        {
            get => _bipLinkStreamDeck;
            set
            {
                _bipLinkStreamDeck = value;
                if (_bipLinkStreamDeck != null)
                {
                    TextBox.Background = Brushes.Bisque;
                }
                else
                {
                    TextBox.Background = Brushes.LightSteelBlue;
                }
            }
        }
        
        public StreamDeckButtonOnOff Key
        {
            get => _button;
            set => _button = value;
        }

        public ActionTypeLayer StreamDeckLayerTarget
        {
            get => _actionTypeLayer;
            set => _actionTypeLayer = value;
        }

        public override void ClearAll()
        {
            _dcsbiosBindingStreamDeck = null;
            _bipLinkStreamDeck = null;
            KeyPress = null;
            TextBox.Background = Brushes.LightSteelBlue;
            TextBox.Text = "";
        }
    }
}
