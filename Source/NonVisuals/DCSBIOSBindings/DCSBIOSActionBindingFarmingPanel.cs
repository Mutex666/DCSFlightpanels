﻿namespace NonVisuals.DCSBIOSBindings
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using DCS_BIOS;

    using MEF;

    using NonVisuals.Saitek;

    [Serializable]
    public class DCSBIOSActionBindingFarmingPanel : DCSBIOSActionBindingBase
    {
        /*
         This class binds a physical switch on the Farming Simulator Side Panel with a DCSBIOSInput
         Pressing the button will send a DCSBIOS command.
         */
        private FarmingPanelMKKeys _farmingPanelKeys;

        ~DCSBIOSActionBindingFarmingPanel()
        {
            CancelSendDCSBIOSCommands = true;
            DCSBIOSCommandsThread?.Abort();
        }

        internal override void ImportSettings(string settings)
        {
            if (string.IsNullOrEmpty(settings))
            {
                throw new ArgumentException("Import string empty. (DCSBIOSActionBindingFarmingPanel)");
            }

            if (settings.StartsWith("FarmingPanelDCSBIOSControl{"))
            {
                // FarmingPanelDCSBIOSControl{1KNOB_ENGINE_OFF}\o/DCSBIOSInput{AAP_STEER|SET_STATE|2}\o/DCSBIOSInput{BAT_PWR|INC|2}\o/\\?\hid#vid_06a3&pid_0d67#9&231fd360&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}
                var parameters = settings.Split(new[] { SaitekConstants.SEPARATOR_SYMBOL }, StringSplitOptions.RemoveEmptyEntries);

                // FarmingPanelDCSBIOSControl{1KNOB_ENGINE_LEFT}
                var param0 = parameters[0].Substring(parameters[0].IndexOf("{", StringComparison.InvariantCulture) + 1);

                // 1KNOB_ENGINE_LEFT}
                param0 = param0.Remove(param0.Length - 1, 1);

                // 1KNOB_ENGINE_LEFT
                WhenOnTurnedOn = (param0.Substring(0, 1) == "1");
                if (param0.Contains("|"))
                {
                    // 1KNOB_ALT|Landing gear up and blablabla description
                    param0 = param0.Substring(1);

                    // KNOB_ALT|Landing gear up and blablabla description
                    var stringArray = param0.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    _farmingPanelKeys = (FarmingPanelMKKeys)Enum.Parse(typeof(FarmingPanelMKKeys), stringArray[0]);
                    Description = stringArray[1];
                }
                else
                {
                    param0 = param0.Substring(1);
                    _farmingPanelKeys = (FarmingPanelMKKeys)Enum.Parse(typeof(FarmingPanelMKKeys), param0);
                }

                // The rest of the array besides last entry are DCSBIOSInput
                // DCSBIOSInput{AAP_EGIPWR|FIXED_STEP|INC}
                DCSBIOSInputs = new List<DCSBIOSInput>();
                for (var i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].StartsWith("DCSBIOSInput{"))
                    {
                        var dcsbiosInput = new DCSBIOSInput();
                        dcsbiosInput.ImportString(parameters[i]);
                        DCSBIOSInputs.Add(dcsbiosInput);
                    }
                }
            }
        }

        public override string ExportSettings()
        {
            if (DCSBIOSInputs.Count == 0)
            {
                return null;
            }

            var onStr = WhenOnTurnedOn ? "1" : "0";

            // \o/DCSBIOSInput{AAP_STEER|SET_STATE|2}\o/DCSBIOSInput{BAT_PWR|INC|2}
            var stringBuilder = new StringBuilder();
            foreach (var dcsbiosInput in DCSBIOSInputs)
            {
                stringBuilder.Append(SaitekConstants.SEPARATOR_SYMBOL + dcsbiosInput);
            }

            if (!string.IsNullOrWhiteSpace(Description))
            {
                return "FarmingPanelDCSBIOSControl{" + onStr + Enum.GetName(typeof(FarmingPanelMKKeys), FarmingPanelKey) + "|" + Description + "}" + SaitekConstants.SEPARATOR_SYMBOL + stringBuilder;
            }

            return "FarmingPanelDCSBIOSControl{" + onStr + Enum.GetName(typeof(FarmingPanelMKKeys), FarmingPanelKey) + "}" + SaitekConstants.SEPARATOR_SYMBOL + stringBuilder;
        }

        public FarmingPanelMKKeys FarmingPanelKey
        {
            get => _farmingPanelKeys;
            set => _farmingPanelKeys = value;
        }
    }
}
