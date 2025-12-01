using NUnit.Framework;
using AppointmentScheduler.Controller;
using AppointmentScheduler.Database;
using AppointmentScheduler.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace AppointmentScheduler.Tests.Acceptance
{
    [TestFixture]
    public class AcceptanceScenarioTests
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

        [Test, Category("Acceptance")]
        public void Scenario_UserCanViewAvailableAppointmentSlots()
        {
            var futureDate = DateTime.Now.AddDays(1);
            var slots = _appointmentController.GetAvailableSlots(futureDate);

            Assert.That(slots, Is.Not.Null, "Available slots should not be null");
            Assert.That(slots.Count, Is.GreaterThan(0), "Should have at least one available slot");

            foreach (var slot in slots)
            {
                Assert.That(slot, Does.Match(@"^\d{2}:\d{2} - \d{2}:\d{2}$"), "Slot format should be HH:mm - HH:mm");
            }
        }

        [Test, Category("Acceptance")]
        public void Scenario_UserCanRetrieveCustomerList()
        {
            var customers = _customerController.GetCustomerNames();

            Assert.That(customers, Is.Not.Null, "Customer list should not be null");
            Assert.That(customers, Is.TypeOf<Dictionary<int, string>>());

            foreach (var customer in customers)
            {
                Assert.That(customer.Key, Is.GreaterThan(0), "Customer ID should be positive");
                Assert.That(customer.Value, Is.Not.Empty, "Customer name should not be empty");
            }
        }
    }
}