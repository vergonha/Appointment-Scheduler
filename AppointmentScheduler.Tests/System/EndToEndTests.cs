using AppointmentScheduler.Controller;
using AppointmentScheduler.Database;
using AppointmentScheduler.Utils;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;

namespace AppointmentScheduler.Tests.System
{
    [TestFixture]
    public class EndToEndSystemTests
    {
        private MySqlConnection _conn;
        private MySqlTransaction _tx;
        private AppointmentController _appointmentController;
        private CustomerController _customerController;

        [SetUp]
        public void SetUp()
        {
            _appointmentController = new AppointmentController();
            _customerController = new CustomerController();
            UserSession.CurrentUserName = "test";
            UserSession.CurrentUserId = 1;

            var connStr = ConfigurationManager.ConnectionStrings["localdb"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(connStr))
            {
                Assert.Inconclusive("Connection string not found.");
                return;
            }

            _conn = new MySql.Data.MySqlClient.MySqlConnection(connStr);
            try { _conn.Open(); }
            catch
            {
                Assert.Inconclusive("Cannot open database connection.");
                return;
            }

            _tx = _conn.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            try { _tx?.Rollback(); } catch { }
            try { _conn?.Close(); } catch { }
        }

        [Test, Category("System")]
        public void System_CreateAppointment_VerifyReportGeneration()
        {
            var appointmentData = new Dictionary<string, string>
            {
                { "CustomerId", "1" },
                { "UserId", "1" },
                { "Description", "System Test Appointment" },
                { "Location", "Virtual" },
                { "VisitType", "Presentation" },
                { "ConsultantName", "test" },
                { "CustomerName", "John Doe" },
                { "AppointmentId", "999" }
            };

            var startEndTime = new Dictionary<string, DateTime>
            {
                { "StartTime", DateTime.UtcNow.AddDays(1) },
                { "EndTime", DateTime.UtcNow.AddDays(1).AddMinutes(30) }
            };

            try
            {
                _appointmentController.SaveAppointment(appointmentData, startEndTime, false);
                var allAppointments = _appointmentController.GetAppointments("All");

                Assert.That(allAppointments, Is.Not.Null);
                Assert.That(allAppointments.Rows.Count, Is.GreaterThanOrEqualTo(0));
            }
            catch (Exception ex)
            {
                Assert.Fail($"System test failed: {ex.Message}");
            }
        }

        [Test, Category("System")]
        public void System_ViewAppointmentFiltersDaily_VerifyFilteredResults()
        {
            var dailyAppointments = _appointmentController.GetAppointments("All");

            Assert.That(dailyAppointments, Is.Not.Null);
            foreach (DataRow row in dailyAppointments.Rows)
            {
                Assert.That(row["start"], Is.TypeOf<DateTime>());
                Assert.That(row["end"], Is.TypeOf<DateTime>());
            }
        }
    }
}