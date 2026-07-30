using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaVeterinariaWPF.Models
{
    public class DaySchedule
    {
        public int Order {  get; set; }

        public string DayOfWeek { get; set; }

        public TimeSpan Start {  get; set; }
        
        public TimeSpan End { get; set; }   

        public string StartDisplay
        {
            get
            {
                if(Start == TimeSpan.Zero && End == TimeSpan.Zero)
                {
                    return "Folga";
                }
                else
                {
                    return Start.ToString(@"hh\:mm");
                }
            }
        }

        public string EndDisplay
        {
            get
            {
                if (Start == TimeSpan.Zero && End == TimeSpan.Zero)
                {
                    return "Folga";
                }
                else
                {
                    return End.ToString(@"hh\:mm");
                }
            }
        }
    }
}
