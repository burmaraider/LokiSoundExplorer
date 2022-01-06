using System;

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        public enum SizeUnits
        {
            Byte, KB, MB, GB, TB, PB, EB, ZB, YB
        }
        public static string ToSize(this String val, SizeUnits unit)
        {
            Int64 value = Convert.ToInt64(val);
            //return (value / (double)Math.Pow(1024, (Int64)unit)).ToString("0.00");
            return (value / (double)Math.Pow(1024, (Int64)unit)).ToString("0");
        }
    }
}