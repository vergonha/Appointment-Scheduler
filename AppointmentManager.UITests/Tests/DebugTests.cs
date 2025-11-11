using AppointmentManager.UITests.Helpers;
using FlaUI.Core.AutomationElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace AppointmentManager.UITests.Tests
{

    [TestClass]
    public class DebugTests : TestBase
    {
        [TestMethod]
        public void Test_Debug_ListAllAppointmentsFields()
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

            System.Threading.Thread.Sleep(2000);
            App.SwitchToSchedulerWindow();

            var appointmentsMenu = App.MainWindow
                .FindFirstDescendant(cf => cf.ByName("Appointments"));
            appointmentsMenu?.Click();

            System.Threading.Thread.Sleep(500);

            var form = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("AddBttn"));
            form?.AsButton().Click();

            System.Threading.Thread.Sleep(1000);

            var elements = App.MainWindow.FindAllDescendants();
            System.Diagnostics.Debug.WriteLine(
                "===== ELEMENTOS NO FORMULÁRIO DE APPOINTMENT ====="
            );
            foreach (var element in elements)
            {
                try
                {
                    var name = element.Name ?? "(sem nome)";
                    var automationId = "(sem id)";
                    try
                    {
                        automationId = element.AutomationId ?? "(sem id)";
                    }
                    catch { }
                    System.Diagnostics.Debug.WriteLine(
                        $"Name: '{name}' | " +
                        $"AutomationId: '{automationId}' | " +
                        $"Type: {element.ControlType}"
                    );
                }
                catch { }
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_Debug_ListButtonsInAppointments()
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

            System.Threading.Thread.Sleep(2000);
            App.SwitchToSchedulerWindow();

            var appointmentsMenu = App.MainWindow
                .FindFirstDescendant(cf => cf.ByName("Appointments"));
            appointmentsMenu.Click();

            System.Threading.Thread.Sleep(1000);

            // Lista TODOS os botões
            var buttons = App.MainWindow
                .FindAllDescendants(cf => cf.ByControlType(
                    FlaUI.Core.Definitions.ControlType.Button
                ));

            System.Diagnostics.Debug.WriteLine("===== BOTÕES NA ABA APPOINTMENTS =====");
            foreach (var button in buttons)
            {
                try
                {
                    var name = button.Name ?? "(sem nome)";
                    var automationId = "(sem id)";

                    try { automationId = button.AutomationId ?? "(sem id)"; } catch { }

                    System.Diagnostics.Debug.WriteLine(
                        $"Name: '{name}' | AutomationId: '{automationId}'"
                    );
                }
                catch { }
            }

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_Debug_ListElementsInScheduler()
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

            System.Threading.Thread.Sleep(2000);

            // Encontra janela Scheduler
            var desktop = App.MainWindow.Automation.GetDesktop();
            var windows = desktop.FindAllChildren();
            var schedulerWindow = windows
                .FirstOrDefault(w => w.Name.Contains("Scheduler"));

            Assert.IsNotNull(schedulerWindow);

            // Lista elementos na janela Scheduler
            var allElements = schedulerWindow.FindAllDescendants();

            System.Diagnostics.Debug.WriteLine(
                "===== ELEMENTOS NA JANELA SCHEDULER ====="
            );
            foreach (var element in allElements.Take(50)) // Primeiros 50
            {
                try
                {
                    var name = element.Name ?? "(sem nome)";
                    var automationId = "(sem id)";

                    try
                    {
                        automationId = element.AutomationId ?? "(sem id)";
                    }
                    catch { }

                    System.Diagnostics.Debug.WriteLine(
                        $"Name: '{name}' | " +
                        $"AutomationId: '{automationId}' | " +
                        $"Type: {element.ControlType}"
                    );
                }
                catch { }
            }

            Assert.IsTrue(true);
        }
    }
}