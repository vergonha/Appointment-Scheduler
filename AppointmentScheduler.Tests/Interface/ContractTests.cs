using NUnit.Framework;
using AppointmentScheduler.Controller;
using AppointmentScheduler.Database;
using AppointmentScheduler.Utils;
using System;
using System.Collections.Generic;
using System.Data;

namespace AppointmentScheduler.Tests.Interface
{
    [TestFixture]
    public class InterfaceContractTests
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

        [Test, Category("Interface")]
        public void AppointmentController_ConvertStringToDateTime_ContractReturnsValidDictionary()
        {
            var date = new DateTime(2025, 12, 15);
            var time = "09:00 - 09:30";

            var result = _appointmentController.ConvertStringToDateTime(date, time);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<Dictionary<string, DateTime>>());
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.ContainsKey("StartTime"));
            Assert.That(result.ContainsKey("EndTime"));
        }

        [Test, Category("Interface")]
        public void AppointmentController_GetAvailableSlots_ContractReturnsStringList()
        {
            var date = DateTime.Now.AddDays(1);

            var slots = _appointmentController.GetAvailableSlots(date);

            Assert.That(slots, Is.TypeOf<List<string>>());
            foreach (var slot in slots)
            {
                Assert.That(slot, Is.TypeOf<string>());
            }
        }

        [Test, Category("Interface")]
        public void AppointmentController_GetAppointments_ContractReturnsDataTable()
        {
            var result = _appointmentController.GetAppointments("All");

            Assert.That(result, Is.TypeOf<DataTable>());
        }

        [Test, Category("Interface")]
        public void CustomerController_GetCustomerNames_ContractReturnsDictionary()
        {
            var result = _customerController.GetCustomerNames();

            Assert.That(result, Is.TypeOf<Dictionary<int, string>>());
            foreach (var kvp in result)
            {
                Assert.That(kvp.Key, Is.TypeOf<int>());
                Assert.That(kvp.Value, Is.TypeOf<string>());
            }
        }
    }
}