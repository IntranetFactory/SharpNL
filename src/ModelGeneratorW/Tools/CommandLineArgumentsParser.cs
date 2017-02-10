using System.Collections.Generic;
using System.Linq;

namespace ModelGeneratorW.Tools
{
    class ApplicationSettings
    {
        public bool InlineIds { get; set; }
        public bool IgnoreHeaderLine { get; set; }
        public string ModelFolder { get; set; }

        public ApplicationSettings()
        {
            ModelFolder = "model";
        }
    }

    class CommandLineArgumentsParserResult
    {
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public ApplicationSettings ApplicationSettings { get; set; }

        public CommandLineArgumentsParserResult()
        {
            ApplicationSettings = new ApplicationSettings();
        }
    }

    class CommandLineArgumentsParser
    {
        private int currentIndex = -1;

        public CommandLineArgumentsParser()
        {
            currentIndex = -1;
        }

        public CommandLineArgumentsParserResult Parse(string[] args)
        {
            CommandLineArgumentsParserResult result = new CommandLineArgumentsParserResult();

            List<string> booleanSwitches = new List<string> { "-ii", "-hl" };
            List<string> valueSwitches = new List<string>() { "-mf" };
            List<string> validSwitchNames = new List<string>();
            validSwitchNames.AddRange(valueSwitches);
            validSwitchNames.AddRange(booleanSwitches);

            List<string> alreadyProcessed = new List<string>();

            while (!IsEnd(args) && !result.HasError)
            {
                string currentArg = Next(args);

                if (string.IsNullOrEmpty(currentArg)) { continue; }

                result.HasError = !validSwitchNames.Any(n => n == currentArg);
                if (result.HasError)
                {
                    result.ErrorMessage = string.Format("{0} is not a valid switch name", currentArg);
                }
                else
                {
                    result.HasError = alreadyProcessed.Any(n => n == currentArg);
                    if (result.HasError)
                    {
                        result.ErrorMessage = string.Format("Switch {0} specified more that once", currentArg);
                        continue;
                    } 

                    var isBooleanSwitch = booleanSwitches.Any(n => n == currentArg);
                    if (isBooleanSwitch)
                    {
                        if (currentArg == "-ii")
                        {
                            result.ApplicationSettings.InlineIds = true;
                        }
                        else if (currentArg == "-hl")
                        {
                            result.ApplicationSettings.IgnoreHeaderLine = true;
                        }
                        alreadyProcessed.Add(currentArg);
                    }
                    else
                    {
                        if (currentArg == "-mf")
                        {
                            string nextArg = Next(args);
                            result.HasError = string.IsNullOrEmpty(nextArg);
                            if (result.HasError)
                            {
                                result.ErrorMessage = string.Format("Empty string is not a valid argument for {0} switch", currentArg);
                            }
                            else
                            {
                                result.HasError = validSwitchNames.Any(s => s == nextArg);
                                if (result.HasError)
                                {
                                    result.ErrorMessage = string.Format("Missing value for {0} switch", currentArg);
                                }
                                else
                                {
                                    result.ApplicationSettings.ModelFolder = nextArg;
                                    alreadyProcessed.Add(currentArg);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private string Next(string[] args)
        {
            this.currentIndex += 1;
            if (this.currentIndex < args.Length)
            {
                return args[this.currentIndex];
            }
            else
            {
                return string.Empty;
            }
        }

        private bool IsEnd(string[] args)
        {
            return this.currentIndex == args.Length;
        }

        private bool IsSwitch(string arg)
        {
            return arg.StartsWith("-");
        }

        private bool IsValue(string arg)
        {
            return !arg.StartsWith("-");
        }
    }
}
