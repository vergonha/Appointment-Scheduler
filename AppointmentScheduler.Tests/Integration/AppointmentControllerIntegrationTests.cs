using NUnit.Framework;
using AppointmentScheduler.Controller;
using AppointmentScheduler.Utils;
using System;
using System.Collections.Generic;

namespace AppointmentScheduler.Tests.Integration
{
    [TestFixture]
    public class AppointmentControllerIntegrationTests
    {
        private AppointmentController _appointmentController;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            UserSession.CurrentUserName = "test";
            UserSession.CurrentUserId = 1;
        }

        [SetUp]
        public void SetUp()
        {
            _appointmentController = new AppointmentController();
        }

        [Test, Category("Integration")]
        public void ConvertStringToDateTime_SingleSlot_ReturnsValidDateTimeDictionary()
        {
            var selectedDate = new DateTime(2025, 12, 15);
            var timeString = "09:00 - 09:30";

            var result = _appointmentController.ConvertStringToDateTime(selectedDate, timeString);

            Assert.That(result, Is.Not.Null, "Result should not be null");
            Assert.That(result.ContainsKey("StartTime"), Is.True, "Should have StartTime key");
            Assert.That(result.ContainsKey("EndTime"), Is.True, "Should have EndTime key");
            Assert.That(result["StartTime"], Is.LessThan(result["EndTime"]), "Start should be before End");
            Assert.That(result["StartTime"].Kind, Is.EqualTo(DateTimeKind.Utc), "Should be in UTC");
            Assert.That(result["EndTime"].Kind, Is.EqualTo(DateTimeKind.Utc), "Should be in UTC");
        }

        [Test, Category("Integration")]
        public void ConvertStringToDateTime_MultipleSlots_AllProduceDifferentTimes()
        {
            var selectedDate = new DateTime(2025, 12, 15);
            var timeStrings = new[] { "09:00 - 09:30", "10:00 - 10:30", "14:00 - 14:30" };
            var results = new List<Dictionary<string, DateTime>>();

            foreach (var timeStr in timeStrings)
            {
                results.Add(_appointmentController.ConvertStringToDateTime(selectedDate, timeStr));
            }

            for (int i = 0; i < results.Count - 1; i++)
            {
                Assert.That(results[i]["StartTime"], Is.Not.EqualTo(results[i + 1]["StartTime"]),
                    "Different time slots should produce different start times");
            }
        }

        [Test, Category("Integration")]
        public void ConvertStringToDateTime_BusinessHoursRange_ProducesValidUTCConversion()
        {
            var selectedDate = new DateTime(2025, 12, 15);
            var businessHourSlots = new[] { "09:00 - 09:30", "17:00 - 17:30" };

            foreach (var slot in businessHourSlots)
            {
                var result = _appointmentController.ConvertStringToDateTime(selectedDate, slot);
                Assert.That(result["StartTime"].Kind, Is.EqualTo(DateTimeKind.Utc));
                Assert.That(result["EndTime"].Kind, Is.EqualTo(DateTimeKind.Utc));
                Assert.That(result["StartTime"], Is.Not.EqualTo(new DateTime(2025, 12, 15, 9, 0, 0)));
            }
        }

        [Test, Category("Integration")]
        public void ConvertStringToDateTime_ConsecutiveSlots_MaintainCorrectDuration()
        {
            var selectedDate = new DateTime(2025, 12, 15);
            var timeString = "09:00 - 09:30";

            var result = _appointmentController.ConvertStringToDateTime(selectedDate, timeString);

            var duration = result["EndTime"] - result["StartTime"];
            Assert.That(duration.TotalMinutes, Is.EqualTo(30), "Duration should be 30 minutes");
        }

        [Test, Category("Integration")]
        public void ConvertStringToDateTime_InvalidTimeFormat_ThrowsFormatException()
        {
            var selectedDate = new DateTime(2025, 12, 15);
            var invalidTimeStr = "not_a_time - also_not_time";

            Assert.Throws<FormatException>(() =>
                _appointmentController.ConvertStringToDateTime(selectedDate, invalidTimeStr),
                "Should throw FormatException for invalid time format");
        }

        [Test, Category("Integration")]
        public void ConvertStringToDateTime_OutOfBusinessHours_StillConvertsCorrectly()
        {
            var selectedDate = new DateTime(2025, 12, 15);
            var earlyMorningSlot = "06:00 - 06:30";

            var result = _appointmentController.ConvertStringToDateTime(selectedDate, earlyMorningSlot);

            Assert.That(result, Is.Not.Null, "Should convert any valid time format");
            Assert.That(result["StartTime"].Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [Test, Category("Integration")]
        public void ConvertStringToDateTime_DifferentDates_ProducesDifferentResults()
        {
            var date1 = new DateTime(2025, 12, 15);
            var date2 = new DateTime(2025, 12, 16);
            var timeString = "09:00 - 09:30";

            var result1 = _appointmentController.ConvertStringToDateTime(date1, timeString);
            var result2 = _appointmentController.ConvertStringToDateTime(date2, timeString);

            Assert.That(result1["StartTime"].Day, Is.EqualTo(15));
            Assert.That(result2["StartTime"].Day, Is.EqualTo(16));
            Assert.That(result1["StartTime"], Is.Not.EqualTo(result2["StartTime"]));
        }

        [Test, Category("Integration")]
        public void ConvertStringToDateTime_TimeZoneAdjustment_IsConsistent()
        {
            var selectedDate = new DateTime(2025, 12, 15);
            var timeString = "09:00 - 09:30";

            var result1 = _appointmentController.ConvertStringToDateTime(selectedDate, timeString);
            var result2 = _appointmentController.ConvertStringToDateTime(selectedDate, timeString);

            Assert.That(result1["StartTime"], Is.EqualTo(result2["StartTime"]),
                "Same input should produce identical output (deterministic)");
        }

        [Test, Category("Integration")]
        public void GetAvailableSlots_ReturnsListOfStrings()
        {
            var futureDate = DateTime.Now.AddDays(10);

            var slots = _appointmentController.GetAvailableSlots(futureDate);

            Assert.That(slots, Is.Not.Null, "Available slots should not be null");
            Assert.That(slots, Is.TypeOf<List<string>>(), "Should return List of strings");
            Assert.That(slots.Count, Is.GreaterThan(0), "Should have at least 1 slot");
        }

        [Test, Category("Integration")]
        public void GetAvailableSlots_AllSlotsMatchTimeFormat()
        {
            var futureDate = DateTime.Now.AddDays(10);

            var slots = _appointmentController.GetAvailableSlots(futureDate);

            foreach (var slot in slots)
            {
                Assert.That(slot, Does.Match(@"^\d{2}:\d{2} - \d{2}:\d{2}$"),
                    $"Slot '{slot}' should match format HH:mm - HH:mm");
            }
        }

        [Test, Category("Integration")]
        public void GetAvailableSlots_SlotsAre30MinutesEach()
        {
            var futureDate = DateTime.Now.AddDays(10);

            var slots = _appointmentController.GetAvailableSlots(futureDate);

            foreach (var slot in slots)
            {
                var parts = slot.Split(new[] { " - " }, StringSplitOptions.None);
                var startTime = DateTime.ParseExact(parts[0], "HH:mm", null);
                var endTime = DateTime.ParseExact(parts[1], "HH:mm", null);

                var duration = endTime - startTime;
                Assert.That(duration.TotalMinutes, Is.EqualTo(30),
                    $"Slot '{slot}' should be 30 minutes long");
            }
        }

        [Test, Category("Integration")]
        public void GetAvailableSlots_RespectsBusinessHours()
        {
            var futureDate = DateTime.Now.AddDays(10);

            var slots = _appointmentController.GetAvailableSlots(futureDate);

            Assert.That(slots.Count, Is.LessThanOrEqualTo(16),
                "Should have at most 16 slots (9AM-5PM with 30-min intervals)");
        }

        [Test, Category("Integration")]
        public void GetAvailableSlots_SlotsAreInChronologicalOrder()
        {
            var futureDate = DateTime.Now.AddDays(10);

            var slots = _appointmentController.GetAvailableSlots(futureDate);

            for (int i = 0; i < slots.Count - 1; i++)
            {
                var currentSlot = slots[i];
                var nextSlot = slots[i + 1];

                var currentEnd = DateTime.ParseExact(
                    currentSlot.Split(new[] { " - " }, StringSplitOptions.None)[1], "HH:mm", null);
                var nextStart = DateTime.ParseExact(
                    nextSlot.Split(new[] { " - " }, StringSplitOptions.None)[0], "HH:mm", null);

                Assert.That(currentEnd, Is.LessThanOrEqualTo(nextStart),
                    "Slots should be in chronological order");
            }
        }

        [Test, Category("Integration")]
        public void GetBookedSlots_ReturnsList()
        {
            var testDate = DateTime.Now.AddDays(15);

            var bookedSlots = _appointmentController.GetBookedSlots(testDate);

            Assert.That(bookedSlots, Is.Not.Null, "Booked slots should not be null");
            Assert.That(bookedSlots, Is.TypeOf<List<Tuple<DateTime, DateTime>>>(),
                "Should return a List of Tuples");
        }

        [Test, Category("Integration")]
        public void GetBookedSlots_AllTuplesValid_StartBeforeEnd()
        {
            var testDate = DateTime.Now.AddDays(15);

            var bookedSlots = _appointmentController.GetBookedSlots(testDate);

            foreach (var slot in bookedSlots)
            {
                Assert.That(slot.Item1, Is.LessThan(slot.Item2),
                    "Start time should be before end time in booked slot");
            }
        }
    }
}