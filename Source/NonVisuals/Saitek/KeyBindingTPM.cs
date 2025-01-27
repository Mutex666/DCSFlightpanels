﻿namespace NonVisuals.Saitek
{
    using System;
    using System.Collections.Generic;

    using MEF;

    [Serializable]
    public class KeyBindingTPM : KeyBinding
    {
        /*
         This class binds a physical switch on the TPM with a user made virtual keypress in Windows.
         */
        private TPMPanelSwitches _tpmPanelSwitch;

        internal override void ImportSettings(string settings)
        {
            if (string.IsNullOrEmpty(settings))
            {
                throw new ArgumentException("Import string empty. (KeyBinding)");
            }

            if (settings.StartsWith("TPMPanelSwitch{"))
            {
                // TPMPanelSwitch{1KNOB_ENGINE_LEFT}\o/OSKeyPress{[FiftyMilliSec,RCONTROL + RSHIFT + VK_R][FiftyMilliSec,RCONTROL + RSHIFT + VK_W]}\o/\\?\hid#vid_06a3&pid_0d67#9&231fd360&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}
                var parameters = settings.Split(new[] { SaitekConstants.SEPARATOR_SYMBOL }, StringSplitOptions.RemoveEmptyEntries);

                // TPMPanelSwitch{1KNOB_ENGINE_LEFT}
                var param0 = parameters[0].Trim().Substring(15);

                // 1KNOB_ENGINE_LEFT}
                param0 = param0.Remove(param0.Length - 1, 1);

                // 1KNOB_ENGINE_LEFT
                WhenTurnedOn = (param0.Substring(0, 1) == "1");
                param0 = param0.Substring(1);
                _tpmPanelSwitch = (TPMPanelSwitches)Enum.Parse(typeof(TPMPanelSwitches), param0);

                // OSKeyPress{[FiftyMilliSec,RCONTROL + RSHIFT + VK_R][FiftyMilliSec,RCONTROL + RSHIFT + VK_W]}
                OSKeyPress = new KeyPress();
                OSKeyPress.ImportString(parameters[1]);
            }
        }

        public TPMPanelSwitches TPMSwitch
        {
            get => _tpmPanelSwitch;
            set => _tpmPanelSwitch = value;
        }

        public override string ExportSettings()
        {
            if (OSKeyPress == null || OSKeyPress.IsEmpty())
            {
                return null;
            }

            var onStr = WhenTurnedOn ? "1" : "0";
            return "TPMPanelSwitch{" + onStr + Enum.GetName(typeof(TPMPanelSwitches), TPMSwitch) + "}" + SaitekConstants.SEPARATOR_SYMBOL + OSKeyPress.ExportString();
        }

        public static HashSet<KeyBindingTPM> SetNegators(HashSet<KeyBindingTPM> knobBindings)
        {
            if (knobBindings == null)
            {
                return null;
            }

            foreach (var keyBindingTPM in knobBindings)
            {
                /*
                 * Some deleted keystrokes may be included here, they have a wrapper although the actual OSKeyPress is null
                 * and will be removed until next "Save". So disregard those.
                 */
                if (keyBindingTPM.OSKeyPress == null)
                {
                    continue;
                }

                // Clear all negators
                keyBindingTPM.OSKeyPress.NegatorOSKeyPresses.Clear();

                foreach (var keyBinding in knobBindings)
                {
                    if (keyBinding != keyBindingTPM && keyBinding.TPMSwitch == keyBindingTPM.TPMSwitch && keyBinding.WhenTurnedOn != keyBindingTPM.WhenTurnedOn)
                    {
                        keyBindingTPM.OSKeyPress.NegatorOSKeyPresses.Add(keyBinding.OSKeyPress);
                    }
                }
            }

            return knobBindings;
        }
    }
}
