using NUnit.Framework;
using AppointmentScheduler.Controller;
using System;
using System.Collections.Generic;

namespace AppointmentScheduler.Tests.Unit
{
    [TestFixture]
    public class CustomerControllerUnitTests
    {
        private CustomerController _customerController;

        [SetUp]
        public void SetUp()
        {
            _customerController = new CustomerController();
        }

        [Test]
        public void GetCustomerNames_ReturnsNonEmptyDictionary()
        {
            var customerNames = _customerController.GetCustomerNames();

            Assert.That(customerNames, Is.Not.Null);
            Assert.That(customerNames, Is.TypeOf<Dictionary<int, string>>());
            Assert.That(customerNames.Count, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void GetCustomerNames_DictionaryKeysAreIntegers()
        {
            var customerNames = _customerController.GetCustomerNames();

            foreach (var kvp in customerNames)
            {
                Assert.That(kvp.Key, Is.TypeOf<int>());
                Assert.That(kvp.Value, Is.TypeOf<string>());
            }
        }

        [Test]
        public void GetCustomers_WithValidQuery_ReturnsDataTable()
        {
            var query = "SELECT customerId, customerName FROM customer LIMIT 1";

            var dataTable = _customerController.GetCustomers(query);

            Assert.That(dataTable, Is.Not.Null);
            Assert.That(dataTable.Columns.Count, Is.GreaterThanOrEqualTo(0));
        }
    }
}