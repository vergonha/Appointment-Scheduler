using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace AppointmentScheduler.Tests
{
    [TestFixture]
    public class ArchitectureTests
    {
        private Assembly DomainAssembly;
        private Assembly DatabaseAssembly;
        private Assembly ControllerAssembly;

        [OneTimeSetUp]
        public void LoadAssemblies()
        {
            // Load your project assemblies
            DomainAssembly = typeof(AppointmentScheduler.Model.Appointment).Assembly;
            DatabaseAssembly = typeof(AppointmentScheduler.Database.DbConnection).Assembly;
            ControllerAssembly = typeof(AppointmentScheduler.Controller.AppointmentController).Assembly;
        }

        [Test]
        public void Model_Should_Not_Depend_On_Database()
        {
            var types = DomainAssembly.GetTypes();
            foreach (var type in types)
            {
                var refs = type.Assembly.GetReferencedAssemblies();
                Assert.That(refs.All(r => !r.Name.Contains("Database")),
                    $"Model layer should not reference Database layer: found in {type.FullName}");
            }
        }

        [Test]
        public void Model_Should_Not_Reference_WindowsForms()
        {
            var formsReference = DomainAssembly.GetReferencedAssemblies()
                .Any(r => r.Name == "System.Windows.Forms");

            Assert.That(formsReference, Is.False, "Model layer must not reference System.Windows.Forms");
        }

        [Test]
        public void Controller_Should_Not_Reference_WindowsForms()
        {
            var refs = ControllerAssembly.GetReferencedAssemblies();
            Assert.That(!refs.Any(r => r.Name == "System.Windows.Forms"),
                "Controller should not reference Windows.Forms directly (UI code must stay in View)");
        }

        [Test]
        public void Views_Should_Not_Access_Database_Directly()
        {
            var uiAssembly = typeof(AppointmentScheduler.Components.AppointmentForm).Assembly;
            var refs = uiAssembly.GetReferencedAssemblies();
            Assert.That(!refs.Any(r => r.Name.Contains("Database")),
                "UI layer should not directly reference the Database layer — it must go through Controller.");
        }

        [Test]
        public void No_Static_Classes_Outside_Utility_Layer()
        {
            var allAssemblies = new[] { DatabaseAssembly, DomainAssembly, ControllerAssembly };
            var invalidStatics = allAssemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsAbstract && t.IsSealed && !t.Name.Contains("Utils"))
                .ToList();

            Assert.That(invalidStatics.Count, Is.EqualTo(0),
                $"Found invalid static classes outside Utils: {string.Join(", ", invalidStatics.Select(t => t.FullName))}");
        }
    }
}