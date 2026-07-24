using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaVeterinariaWPF.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string Speciality { get; set; }

        public bool Active { get; set; }
    }
}
