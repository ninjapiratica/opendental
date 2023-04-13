using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;

namespace UnitTestsCore
{
    public class MiscTestUtils
    {
        ///<summary>Collapses multiple consecutive whitespace characters to a single character.  Helpful for unit tests comparing equality on two strings 
        ///when whitespace is inconsequential.</summary>
        public static string CollapseWhitespace(string value)
        {
            value = Regex.Replace(value, @"\s+", " ").ToLower().Trim();
            return Regex.IsMatch(value, @"\s\s+") ? CollapseWhitespace(value) : value;
        }

        ///<summary>Compares two DateTimes to the nearest second.</summary>
        public static void AssertDateTime(DateTime expected, DateTime actual)
        {
            string format = "yyyy MM dd HH mm ss";
            Assert.AreEqual(expected.ToString(format), actual.ToString(format));
        }
    }
}
