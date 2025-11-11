using AppointmentManager.UITests.Helpers;
using FlaUI.Core.AutomationElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;

namespace AppointmentManager.UITests.Tests
{
    [TestClass]
    public class AppointmentFormTests : TestBase
    {
        public Window OpenAppointmentForm()
        {
            // Login
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

            // Clica no menu Appointments
            var appointmentsMenu = App.MainWindow
                .FindFirstDescendant(cf => cf.ByName("Appointments"));
            appointmentsMenu?.Click();

            System.Threading.Thread.Sleep(200);

            // Clica no botão Add
            var addButton = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("AddBttn"));
            addButton?.AsButton().Click();

            System.Threading.Thread.Sleep(200);

            // Encontra a nova janela de appointment
            var desktop = App.MainWindow.Automation.GetDesktop();
            var appointmentWindow = desktop.FindAllChildren()
                .FirstOrDefault(w => w.Name.Contains("Add Appointment")
                    || w.Name.Contains("Appointment"));

            return appointmentWindow?.AsWindow();
        }

        [TestMethod]
        public void Test_01_CanOpenAppointmentForm()
        {
            var form = OpenAppointmentForm();

            Assert.IsNotNull(form, "Appointment form did not open");
        }

        [TestMethod]
        public void Test_02_CustomerComboBox_Exists()
        {
            var appointmentForm = OpenAppointmentForm();

            var combo = appointmentForm
                ?.FindFirstDescendant(cf => cf.ByAutomationId("comboBoxCustomers"));

            Assert.IsNotNull(combo, "ComboBox 'comboBoxCustomers' not found");
        }

        [TestMethod]
        public void Test_03_DescriptionField_Exists()
        {
            var appointmentForm = OpenAppointmentForm();

            var description = appointmentForm
                ?.FindFirstDescendant(cf => cf.ByAutomationId("txtDescription"));

            Assert.IsNotNull(description, "TextBox 'txtDescription' not found");
        }

        [TestMethod]
        public void Test_04_SaveButton_Exists()
        {
            var appointmentForm = OpenAppointmentForm();

            var button = appointmentForm
                ?.FindFirstDescendant(cf => cf.ByAutomationId("SaveBttn"));

            Assert.IsNotNull(button, "Button 'SaveBttn' not found");
        }

        [TestMethod]
        public void Test_05_CancelButton_Exists()
        {
            var appointmentForm = OpenAppointmentForm();

            var button = appointmentForm
                ?.FindFirstDescendant(cf => cf.ByAutomationId("CancelBttn"));

            Assert.IsNotNull(button, "Button 'CancelBttn' not found");
        }
        [TestMethod]
        public void Test_06_CanAddAppointmentAndSeeItInList()
        {
            var form = OpenAppointmentForm();
            Assert.IsNotNull(form, "Appointment form not found");

            const string appointmentName = "Teste Aula";

            var descriptionBox = form
                .FindFirstDescendant(cf => cf.ByAutomationId("txtDescription"))
                ?.AsTextBox();
            Assert.IsNotNull(descriptionBox, "txtDescription not found");
            descriptionBox.Text = appointmentName;

            var saveButton = form
                .FindFirstDescendant(cf => cf.ByAutomationId("SaveBttn"))
                ?.AsButton();
            Assert.IsNotNull(saveButton, "Save button not found");
            saveButton.Click();

            App.CloseMessageBox();
            Thread.Sleep(500);
            App.SwitchToSchedulerWindow();
            Thread.Sleep(500);

            // DataGrid principal
            var grid = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("mainDataGridView"));
            Assert.IsNotNull(grid, "mainDataGridView not found");

            var cells = grid.FindAllDescendants(
                cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit));

            bool found = false;

            foreach (var cell in cells)
            {
                string value = string.Empty;

                try
                {
                    if (cell.Patterns.Text.IsSupported)
                        value = cell.Patterns.Text.Pattern.DocumentRange.GetText(-1);
                    else if (cell.Patterns.Value.IsSupported)
                        value = cell.Patterns.Value.Pattern.Value;
                }
                catch { /* ignora células sem valor */ }

                if (string.IsNullOrWhiteSpace(value))
                    continue;


                if (value.Contains(appointmentName))
                {
                    found = true;
                    break;
                }
            }

            Assert.IsTrue(found, $"'{appointmentName}' not found on appointment list.");
        }
    }
}