using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AppointmentManager.UITests.Helpers
{
    public class AppHelper : IDisposable
    {
        private Application _app;
        private UIA3Automation _automation;
        public Window MainWindow { get; private set; }

        public AppHelper()
        {
            _automation = new UIA3Automation();
        }

        public void Start()
        {
            var appPath = GetApplicationPath();

            if (!File.Exists(appPath))
            {
                throw new FileNotFoundException(
                    $"Application not found at: {appPath}"
                );
            }

            _app = Application.Launch(appPath);
            _app.WaitWhileBusy();
            System.Threading.Thread.Sleep(2000);

            MainWindow = _app.GetMainWindow(_automation);
        }

        public void Close()
        {
            MainWindow?.Close();
            _app?.Close();
        }

        public void CloseMessageBox()
        {
            System.Threading.Thread.Sleep(500);
            FlaUI.Core.Input.Keyboard.Press(
                FlaUI.Core.WindowsAPI.VirtualKeyShort.ENTER
            );
            System.Threading.Thread.Sleep(500);
        }

        public void SwitchToSchedulerWindow()
        {
            System.Threading.Thread.Sleep(1000);

            var desktop = _automation.GetDesktop();
            var windows = desktop.FindAllChildren();

            var schedulerWindow = windows
                .FirstOrDefault(w => w.Name.Contains("Scheduler"));

            if (schedulerWindow != null)
            {
                MainWindow = schedulerWindow.AsWindow();
            }
        }
        public void Dispose()
        {
            Close();
            _automation?.Dispose();
        }

        private string GetApplicationPath()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var projectRoot = Directory.GetParent(baseDirectory)
                .Parent.Parent.Parent.FullName;
            return Path.Combine(
                projectRoot,
                "Appointment-Scheduler",
                "src",
                "bin",
                "Debug",
                "AppointmentManager.exe"
            );
        }
    }
}