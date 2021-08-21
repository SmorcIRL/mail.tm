using System;

namespace SmorcIRL.TempMail.Helpers
{
    internal static class Ensure
    {
        public static void IsPresent(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Missing string param", paramName);
            }
        }

        public static void IsPositive(int value, string paramName)
        {
            if (value <= 0)
            {
                throw new ArgumentException("Positive value expected", paramName);
            }
        }
    }
}