using NUnit.Framework;
using AppointmentScheduler.Controller;
using System;
using System.Collections.Generic;

namespace AppointmentScheduler.Tests.Unit
{
    [TestFixture]
    public class AppointmentControllerUnitTests
    {
        private AppointmentController _appointmentController;

        [SetUp]
        public void SetUp()
        {
            _appointmentController = new AppointmentController();
        }

        [Test]
        public void ConvertStringToDateTime_ValidInput_ReturnsDictionaryWithStartAndEndTime()
        {
            var selectedDate = new DateTime(2025, 12, 15);
            var selectedTimeStr = "09:00 - 09:30";

            var result = _appointmentController.ConvertStringToDateTime(selectedDate, selectedTimeStr);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContainsKey("StartTime"), Is.True);
            Assert.That(result.ContainsKey("EndTime"), Is.True);
            Assert.That(result["StartTime"], Is.LessThan(result["EndTime"]));
        }

        [Test]
        public void ConvertStringToDateTime_InvalidTimeFormat_ThrowsFormatException()
        {
            var selectedDate = new DateTime(2025, 12, 15);
            var invalidTimeStr = "25:00 - 26:30";

            Assert.Throws<FormatException>(() => _appointmentController.ConvertStringToDateTime(selectedDate, invalidTimeStr));
        }

        [Test]
        public void GetAvailableSlots_ReturnsListOfStrings()
        {
            var futureDate = DateTime.Now.AddDays(1);

            var slots = _appointmentController.GetAvailableSlots(futureDate);

            Assert.That(slots, Is.Not.Null);
            Assert.That(slots, Is.TypeOf<List<string>>());
            Assert.That(slots.Count, Is.GreaterThan(0));
        }

        [Test]
        public void GetAvailableSlots_SlotsFollowBusinessHoursFormat()
        {
            var futureDate = DateTime.Now.AddDays(1);

            var slots = _appointmentController.GetAvailableSlots(futureDate);

            foreach (var slot in slots)
            {
                Assert.That(slot, Does.Match(@"^\d{2}:\d{2} - \d{2}:\d{2}$"));
            }
        }

        [Test]
        public void GetBookedSlots_ReturnsList()
        {
            var testDate = DateTime.Now.AddDays(1);

            var bookedSlots = _appointmentController.GetBookedSlots(testDate);

            Assert.That(bookedSlots, Is.Not.Null);
            Assert.That(bookedSlots, Is.TypeOf<List<Tuple<DateTime, DateTime>>>());
        }
    }
}