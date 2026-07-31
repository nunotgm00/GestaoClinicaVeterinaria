using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaVeterinariaWPF.Models
{
    public class AppointmentHelper
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string AnimalName { get; set; }

        public string DoctorName { get; set; }

        public string RoomName { get; set; }

        public string Motive { get; set; }

        public string Canceled { get; set; }

        public string DateTimeDisplay
        { 
            get 
            {
                return (Date.ToString("dd/MM/yyyy") + " " + StartTime.ToString(@"hh\:mm") + " - " + EndTime.ToString(@"hh\:mm"));
            } 
        }
    }
}
