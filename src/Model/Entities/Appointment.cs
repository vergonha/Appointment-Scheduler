using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentScheduler.Model
{
    public class Appointment : BaseClass
    {
        public int AppointmentID { get; set; }

        public Customer Customer { get; set; }

        public int UserID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public string Contact { get; set; }

        public string URL { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }
}
