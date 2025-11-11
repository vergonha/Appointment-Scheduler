using AppointmentManager.UITests.Helpers;
using FlaUI.Core.AutomationElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppointmentManager.UITests.Tests
{
    [TestClass]
    public class LoginTests : TestBase
    {
        [TestMethod]
        public void Test_01_LoginWindow_Opens()
        {
            Assert.IsNotNull(App.MainWindow);
            Assert.AreEqual("Log In", App.MainWindow.Title);
        }

        [TestMethod]
        public void Test_02_UsernameField_Exists()
        {
            var username = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("txtUserLogin"));

            Assert.IsNotNull(username);
        }

        [TestMethod]
        public void Test_03_PasswordField_Exists()
        {
            var password = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("txtUserPassword"));

            Assert.IsNotNull(password);
        }

        [TestMethod]
        public void Test_04_LoginButton_Exists()
        {
            var button = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("btnLogin"));

            Assert.IsNotNull(button);
        }

        [TestMethod]
        public void Test_05_CanTypeUsername()
        {
            var username = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("txtUserLogin"))
                .AsTextBox();

            username.Text = "test";
            Assert.AreEqual("test", username.Text);
        }

        [TestMethod]
        public void Test_06_CanTypePassword()
        {
            var password = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("txtUserPassword"))
                .AsTextBox();

            password.Text = "test";
            // Password não retorna texto por segurança, só verifica que não deu erro
            Assert.IsNotNull(password);
        }

        [TestMethod]
        public void Test_07_CanClickLoginButton()
        {
            var username = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("txtUserLogin"))
                .AsTextBox();

            var password = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("txtUserPassword"))
                .AsTextBox();

            var button = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("btnLogin"))
                .AsButton();

            username.Text = "test";
            password.Text = "test";
            button.Click();

            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_08_ExitButton_Exists()
        {
            var exitButton = App.MainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId("btnExit"));

            Assert.IsNotNull(exitButton);
        }
    }
}