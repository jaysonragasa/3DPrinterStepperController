using System.Text.RegularExpressions;

namespace GCode.Helpers
{
    public class MarlinSettings
    {
        #region steps
        public double Step_X { get; set; } = 0.0d;
        public double Step_Y { get; set; } = 0.0d;
        public double Step_Z { get; set; } = 0.0d;
        public double Step_E { get; set; } = 0.0d;
        #endregion

        #region maximum feed rates
        public double FeedRates_X { get; set; } = 0.0d;
        public double FeedRates_Y { get; set; } = 0.0d;
        public double FeedRates_Z { get; set; } = 0.0d;
        public double FeedRates_E { get; set; } = 0.0d;
        #endregion

        #region maximum acceleration settings (units/s2)
        public double StepperAcc_X { get; set; } = 0.0d;
        public double StepperAcc_Y { get; set; } = 0.0d;
        public double StepperAcc_Z { get; set; } = 0.0d;
        public double StepperAcc_E { get; set; } = 0.0d;
        #endregion

        #region extruder acceleration
        public double Extruder_Print_Acceleration { get; set; } = 0.0d;
        public double Extruder_Retract_Acceleration { get; set; } = 0.0d;
        public double Extruder_Travel_Acceleration { get; set; } = 0.0d;
        #endregion

        public MarlinSettings()
        {

        }

        public void ReadSettings(string log)
        {
            string regex = string.Empty;

            RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
            Regex reg = null;

            /*
             * some reg ex 
             *  echo:\sLast\sUpdated:\s(?<last_updated>.*)\s\|\sAuthor:\s\(\s\)\n
                echo:Compiled:\s(?<compiled>.*)\n
                echo:\sFree\sMemory:\s(?<free_mem>\d+)\s\sPlannerBufferBytes:\s(?<buff_bytes>\d+)

               STEPPER
                echo:\s\sM92\sX(?<steps_x>.+)\sY(?<steps_y>.+)\sZ(?<steps_z>.+ )E(?<steps_e>.+)\n
               
               FEED RATES
                echo:\s\sM203\sX(?<frate_x>.+)\sY(?<frate_y>.+)\sZ(?<frate_z>.+ )E(?<frate_e>.+)\n

               MAX ACC SETTING
                echo:\s\sM203\sX(?<step_acc_x>.+)\sY(?<step_acc__y>.+)\sZ(?<step_acc__z>.+ )E(?<step_acc__e>.+)\n

               EXTRUDER ACC
                echo:\s\sM204\sP(?<print_acc>.+)\sR(?<retract_acc>.+)\sT(?<travel_acc>.+ )\n
             */

            // get step per unit settings
            regex = "echo:\\s\\sM92\\sX(?<steps_x>.+)\\sY(?<steps_y>.+)\\sZ(?<steps_z>.+ )E(?<steps_e>.+)\\n";
            reg = new Regex(regex, options);
            if(reg.IsMatch(log))
            {
                var groups = reg.Match(log).Groups;
                this.Step_X = double.Parse(groups["steps_x"].Value);
                this.Step_Y = double.Parse(groups["steps_y"].Value);
                this.Step_Z = double.Parse(groups["steps_z"].Value);
                this.Step_E = double.Parse(groups["steps_e"].Value);
            }

            // feed rates
            regex = "echo:\\s\\sM203\\sX(?<frate_x>.+)\\sY(?<frate_y>.+)\\sZ(?<frate_z>.+ )E(?<frate_e>.+)\\n";
            reg = new Regex(regex, options);
            if (reg.IsMatch(log))
            {
                var groups = reg.Match(log).Groups;
                this.FeedRates_X = double.Parse(groups["frate_x"].Value);
                this.FeedRates_Y = double.Parse(groups["frate_y"].Value);
                this.FeedRates_Z = double.Parse(groups["frate_z"].Value);
                this.FeedRates_E = double.Parse(groups["frate_e"].Value);
            }

            // max acceleration settings
            regex = "echo:\\s\\sM201\\sX(?<step_acc_x>.+)\\sY(?<step_acc_y>.+)\\sZ(?<step_acc_z>.+ )E(?<step_acc_e>.+)\\n";
            reg = new Regex(regex, options);
            if (reg.IsMatch(log))
            {
                var groups = reg.Match(log).Groups;
                this.StepperAcc_X = double.Parse(groups["step_acc_x"].Value);
                this.StepperAcc_Y = double.Parse(groups["step_acc_y"].Value);
                this.StepperAcc_Z = double.Parse(groups["step_acc_z"].Value);
                this.StepperAcc_E = double.Parse(groups["step_acc_e"].Value);
            }

            // extruder acc
            regex = "echo:\\s\\sM204\\sP(?<print_acc>.+)\\sR(?<retract_acc>.+)\\sT(?<travel_acc>.+ )\\n";
            reg = new Regex(regex, options);
            if (reg.IsMatch(log))
            {
                var groups = reg.Match(log).Groups;
                this.Extruder_Print_Acceleration = double.Parse(groups["print_acc"].Value);
                this.Extruder_Retract_Acceleration = double.Parse(groups["retract_acc"].Value);
                this.Extruder_Travel_Acceleration = double.Parse(groups["travel_acc"].Value);
            }
        }
    }


}
