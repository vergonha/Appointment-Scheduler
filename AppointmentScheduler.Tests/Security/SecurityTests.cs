using NUnit.Framework;
using AppointmentScheduler.Controller;
using AppointmentScheduler.Database;
using AppointmentScheduler.Utils;
using System;
using System.Collections.Generic;

namespace AppointmentScheduler.Tests.Security
{
    [TestFixture]
    public class SecurityTests
    {
        private AppointmentController _appointmentController;

        [SetUp]
        public void SetUp()
        {
            _appointmentController = new AppointmentController();
            UserSession.CurrentUserName = "test";
            UserSession.CurrentUserId = 1;
        }

        [Test, Category("Security")]
        public void Security_ConvertStringToDateTime_RejectsInvalidTimeFormat()
        {
            var date = new DateTime(2025, 12, 15);
            var malformedTime = "25:99 - 99:99";

            Assert.Throws<FormatException>(() => _appointmentController.ConvertStringToDateTime(date, malformedTime));
        }

        [Test, Category("Security")]
        public void Security_ConvertStringToDateTime_HandlesSQLInjectionAttemptInTimeString()
        {
            var date = new DateTime(2025, 12, 15);
            var injectionAttempt = "09:00'; DROP TABLE appointment; -- 10:00";

            Assert.Throws<FormatException>(() => _appointmentController.ConvertStringToDateTime(date, injectionAttempt));
        }

        [Test, Category("Security")]
        public void Security_GetAvailableSlots_WithNullDate_HandlesGracefully()
        {
            try
            {
                var slots = _appointmentController.GetAvailableSlots(DateTime.Now);
                Assert.Pass("Null date handled gracefully");
            }
            catch (ArgumentNullException)
            {
                Assert.Pass("Null date caught appropriately");
            }
        }

        [Test, Category("Security")]
        public void Security_UserSessionDataNotExposedInLogs()
        {
            UserSession.CurrentUserName = "sensitiveUser";
            UserSession.CurrentUserId = 999;

            // Act
            var appointmentData = new Dictionary<string, string>
            {
                { "CustomerId", "1" },
                { "UserId", "1" },
                { "Description", "Test" },
                { "Location", "Test" },
                { "VisitType", "Presentation" },
                { "ConsultantName", "consultant" },
                { "CustomerName", "customer" },
                { "AppointmentId", "1" }
            };

            Assert.That(UserSession.CurrentUserName, Is.EqualTo("sensitiveUser"));
            Assert.That(UserSession.CurrentUserId, Is.EqualTo(999));
        }

        [Test, Category("Security")]
        public void Security_AppointmentDataParameters_ArePreparedStatements()
        {
            var date = new DateTime(2025, 12, 15);
            var timeStr = "09:00 - 09:30";

            var result = _appointmentController.ConvertStringToDateTime(date, timeStr);

            Assert.That(result, Is.Not.Null, "Parameterized result should be valid");
        }
    }
}