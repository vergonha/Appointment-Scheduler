using NUnit.Framework;
using AppointmentScheduler.Database;
using System.Data;

namespace AppointmentScheduler.Tests.Database
{
    [TestFixture]
    public class DbConnectionTests
    {
        [SetUp]
        public void SetupBeforeEach()
        {
            DbConnection.CloseConnection();
        }

        [Test]
        public void Connection_Should_Open_Successfully()
        {
            DbConnection.StartConnection();

            Assert.That(DbConnection.conn, Is.Not.Null, "Connection should not be null.");
            Assert.That(DbConnection.conn.State, Is.EqualTo(ConnectionState.Open), "Connection should be open.");

            DbConnection.CloseConnection();
        }

        [Test]
        public void Connection_Should_Close_Successfully()
        {
            DbConnection.StartConnection();

            DbConnection.CloseConnection();

            bool closed = (DbConnection.conn == null ||
                           DbConnection.conn.State == ConnectionState.Closed);

            Assert.That(closed, Is.True, "Connection should be closed.");
        }
    }
}