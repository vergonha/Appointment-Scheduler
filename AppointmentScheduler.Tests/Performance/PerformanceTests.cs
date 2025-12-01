using NUnit.Framework;
using AppointmentScheduler.Controller;
using AppointmentScheduler.Database;
using AppointmentScheduler.Utils;
using System;
using System.Diagnostics;

namespace AppointmentScheduler.Tests.Performance
{
    [TestFixture]
    public class PerformanceTests
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

        [Test, Category("Performance")]
        public void Performance_ConvertStringToDateTime_CompletesSub10Milliseconds()
        {
            var sw = Stopwatch.StartNew();
            var date = new DateTime(2025, 12, 15);
            var time = "09:00 - 09:30";

            for (int i = 0; i < 100; i++)
            {
                _appointmentController.ConvertStringToDateTime(date, time);
            }
            sw.Stop();

            var averageMs = sw.ElapsedMilliseconds / 100.0;
            Assert.That(averageMs, Is.LessThan(10), $"Average execution time {averageMs}ms exceeds 10ms threshold");
        }

        [Test, Category("Performance")]
        public void Performance_GetAvailableSlots_CompletesUnder1500Milliseconds()
        {
            var sw = Stopwatch.StartNew();
            var date = DateTime.Now.AddDays(1);

            var slots = _appointmentController.GetAvailableSlots(date);
            sw.Stop();

            Assert.That(sw.ElapsedMilliseconds, Is.LessThan(1500), "GetAvailableSlots exceeded 1500ms");
            Assert.That(slots.Count, Is.GreaterThan(0));
        }

        [Test, Category("Performance")]
        public void Performance_GetCustomerNames_CompletesUnder1Second()
        {
            var sw = Stopwatch.StartNew();

            var customers = _customerController.GetCustomerNames();
            sw.Stop();

            Assert.That(sw.ElapsedMilliseconds, Is.LessThan(1000), "GetCustomerNames exceeded 1000ms");
        }

        [Test, Category("Performance")]
        public void Performance_GetAppointments_HandlesLargeResultSet()
        {
            var sw = Stopwatch.StartNew();

            var appointments = _appointmentController.GetAppointments("All");
            sw.Stop();

            Assert.That(sw.ElapsedMilliseconds, Is.LessThan(2000), "GetAppointments exceeded 2000ms");
            Assert.That(appointments.Rows.Count, Is.GreaterThanOrEqualTo(0));
        }
    }
}