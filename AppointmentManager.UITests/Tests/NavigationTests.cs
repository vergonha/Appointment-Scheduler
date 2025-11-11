using AppointmentManager.UITests.Helpers;
using FlaUI.Core.AutomationElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace AppointmentManager.UITests.Tests
{
    [TestClass]
    public class NavigationTests : TestBase
    {
        [TestMethod]
        public void Test_01_CanLogin_OpensCustomersTab()
        {
            var username = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("txtUserLogin"))
                .AsTextBox();

            var password = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("txtUserPassword"))
                .AsTextBox();

            var loginButton = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("btnLogin"))
                .AsButton();

            username.Text = "test";
            password.Text = "test";
            loginButton.Click();
            App.CloseMessageBox();

            System.Threading.Thread.Sleep(200);
            App.SwitchToSchedulerWindow();

            var addButton = App.MainWindow
                .FindFirstDescendant(cf => cf.ByName("Add Customer"));

            Assert.IsNotNull(addButton, "Opens on Customers tab");
        }

        [TestMethod]
        public void Test_02_AppointmentsMenu_Exists()
        {
            var username = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("txtUserLogin"))
                .AsTextBox();

            var password = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("txtUserPassword"))
                .AsTextBox();

            var loginButton = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("btnLogin"))
                .AsButton();

            username.Text = "test";
            password.Text = "test";
            loginButton.Click();
            App.CloseMessageBox();

            System.Threading.Thread.Sleep(200);
            App.SwitchToSchedulerWindow();

            var appointmentsMenu = App.MainWindow
                .FindFirstDescendant(cf => cf.ByName("Appointments"));

            Assert.IsNotNull(appointmentsMenu);
        }

        [TestMethod]
        public void Test_03_CanClickAppointmentsMenu()
        {
            var username = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("txtUserLogin"))
                .AsTextBox();

            var password = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("txtUserPassword"))
                .AsTextBox();

            var loginButton = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("btnLogin"))
                .AsButton();

            username.Text = "test";
            password.Text = "test";
            loginButton.Click();
            App.CloseMessageBox();

            System.Threading.Thread.Sleep(200);
            App.SwitchToSchedulerWindow();

            var appointmentsMenu = App.MainWindow
                .FindFirstDescendant(cf => cf.ByName("Appointments"));

            appointmentsMenu.Click();
            System.Threading.Thread.Sleep(500);

            var addButton = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("AddBttn"));

            Assert.IsNotNull(addButton);
        }
    }
}