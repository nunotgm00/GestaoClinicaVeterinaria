using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaVeterinariaWPF.Models
{
    public class Room
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public bool UnderMaintenance { get; set; }

        public string RoomName 
        { 
            get
            {
                return "Sala " + Id.ToString();
            }

        }
    }
}
