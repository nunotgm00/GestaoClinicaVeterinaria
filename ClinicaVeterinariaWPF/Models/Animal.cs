using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaVeterinariaWPF.Models
{
    public class Animal
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Species { get; set; }

        public string Breed { get; set; }

        public int Age { get; set;}

        public decimal Weight { get; set;}

        public string Color { get; set; }

        public string Sex { get; set; }

        public int? ClientId { get; set; }
    }
}
