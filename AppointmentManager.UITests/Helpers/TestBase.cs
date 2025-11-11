using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppointmentManager.UITests.Helpers
{
    public class TestBase
    {
        protected AppHelper App { get; private set; }

        [TestInitialize]
        public void Setup()
        {
            App = new AppHelper();
            App.Start();
        }

        [TestCleanup]
        public void Teardown()
        {
            App?.Dispose();
        }
    }
}