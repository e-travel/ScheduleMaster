using System;
using System.Text.RegularExpressions;

namespace ScheduleMaster.Component
{
    public class ActionBaseCommand
    {
        protected static bool IsValidRegex(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return false;

            try
            {
                Regex.Match("", pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }
    }
}