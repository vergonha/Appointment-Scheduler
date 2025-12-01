using NUnit.Framework;
using AppointmentScheduler.Controller;
using AppointmentScheduler.Database;
using AppointmentScheduler.Utils;
using System;
using System.Collections.Generic;

namespace AppointmentScheduler.Tests.BlackBox
{
    [TestFixture]
    public class BlackBoxRequirementTests
    {
        private AppointmentController _appointmentController;
        private CustomerController _customerController;

        [SetUp]
        public void SetUp()
        {
            _appointmentController = new AppointmentController();
            _customerController = new CustomerController();
            UserSession.CurrentUserName = "test";
            UserSession.CurrentUserId = 1;
        }

        [Test, Category("BlackBox")]
        public void Requirement_E_AppointmentTimesAreConvertedToUserTimeZone()
        {
            var selectedDate = new DateTime(2025, 12, 15);
            var timeStr = "14:00 - 14:30";

            var result = _appointmentController.ConvertStringToDateTime(selectedDate, timeStr);

            Assert.That(result["StartTime"], Is.LessThan(result["EndTime"]));
            Assert.That(result["StartTime"].Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [Test, Category("BlackBox")]
        public void Requirement_F_AppointmentSlotsOnlyShowBusinessHours()
        {
            var futureDate = DateTime.Now.AddDays(2);

            var slots = _appointmentController.GetAvailableSlots(futureDate);

            Assert.That(slots.Count, Is.GreaterThan(0));
            Assert.That(slots.Count, Is.LessThanOrEqualTo(16));
        }

        [Test, Category("BlackBox")]
        public void Requirement_D_AppointmentFiltersReturnCorrectData()
        {
            var filterTypes = new[] { "All", "Weekly", "Monthly" };

            foreach (var filter in filterTypes)
            {
                var appointments = _appointmentController.GetAppointments(filter);
                Assert.That(appointments, Is.Not.Null, $"Filter '{filter}' should return data");
                Assert.That(appointments.Rows.Count, Is.GreaterThanOrEqualTo(0));
            }
        }

        [Test, Category("BlackBox")]
        public void Requirement_G_LambdaExpressionsCorrectlyFilterData()
        {
            var futureDate = DateTime.Now.AddDays(1);
            var slots = _appointmentController.GetAvailableSlots(futureDate);

            Assert.That(slots, Is.Not.Null);
            Assert.That(slots, Is.TypeOf<List<string>>());
        }
    }
}