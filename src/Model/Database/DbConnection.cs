using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace AppointmentScheduler.Database
{
    public class DbConnection
    {
        public static MySqlConnection conn { get; set; }

        public static void StartConnection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["localdb"].ConnectionString;
            try
            {
                conn = new MySqlConnection(conStr);
                conn.Open();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void CloseConnection()
        {
            try
            {
                if (conn != null)
                {
                    conn.Close();
                }
                conn = null;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static bool InitializeDatabase()
        {
            bool isNewDatabase = false;
            try
            {
                StartConnection();

                MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM user", conn);
                int userCount = 0;

                try
                {
                    userCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                }
                catch
                {
                    userCount = 0;
                }

                if (userCount == 0)
                {
                    MySqlScript script = new MySqlScript(conn, Queries.InitializeDatabaseQuery);
                    script.Execute();
                    isNewDatabase = true;
                }

                return isNewDatabase;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database initialization error: " + ex.Message);
                return isNewDatabase;
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}