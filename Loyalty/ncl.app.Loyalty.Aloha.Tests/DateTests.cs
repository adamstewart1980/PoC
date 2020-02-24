using NUnit.Framework;
using System;
using ncl.app.Loyalty.Aloha.Relay.Helpers;

namespace ncl.app.Loyalty.Aloha.Tests
{
    [TestFixture]
    public class DateTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [TestCase(2020, 2, 24, "24-02-2020")] //monday should return monday
        [TestCase(2020, 2, 25, "24-02-2020")] //tuesday should return monday
        [TestCase(2020, 2, 23, "17-02-2020")] //sunday should return monday
        [TestCase(2020, 3, 1, "24-02-2020")] //should deal with feb
        [TestCase(2020, 1, 2, "30-12-2019")] //should deal with year change
        public void EnsureGetStartOfTheWeek(int year, int month, int day, string expectedDate)
        {
            var currentDate = new DateTime(year, month, day);

            var startOfWeek = currentDate.StartOfWeek(DayOfWeek.Monday);

            Assert.IsTrue(startOfWeek.ToString("dd-MM-yyyy") == expectedDate);
        }
    }
}
