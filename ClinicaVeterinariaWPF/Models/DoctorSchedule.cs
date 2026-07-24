using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaVeterinariaWPF.Models
{
    internal class DoctorSchedule
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }

        public byte DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}
