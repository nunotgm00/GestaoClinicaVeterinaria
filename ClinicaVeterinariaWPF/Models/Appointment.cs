using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClinicaVeterinariaWPF.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public int AnimalId { get; set; }

        public int DoctorId { get; set; }

        public int RoomId { get; set; }

        public string Motive {  get; set; }

        public string Treatment { get; set; }

        public bool Canceled { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}
