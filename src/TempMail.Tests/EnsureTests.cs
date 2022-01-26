using System;
using NUnit.Framework;
using SmorcIRL.TempMail.Helpers;

namespace SmorcIRL.TempMail.Tests
{
    public class EnsureTests
    {
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("123", true)]
        public void IsPresentTest(string value, bool result)
        {
            if (result)
            {
                Ensure.IsPresent(value, "value");
            }
            else
            {
                Assert.Throws<ArgumentException>(() => Ensure.IsPresent(value, "value"));
            }
        }
    
        [TestCase(-10, false)]
        [TestCase(0, false)]
        [TestCase(10, true)]
        public void IsPositiveTest(int value, bool result)
        {
            if (result)
            {
                Ensure.IsPositive(value, "value");
            }
            else
            {
                Assert.Throws<ArgumentException>(() => Ensure.IsPositive(value, "value"));
            }
        }
    }
}