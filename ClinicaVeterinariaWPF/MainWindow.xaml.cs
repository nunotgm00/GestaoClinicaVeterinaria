using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClinicaVeterinariaWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnClients_Click(object sender, RoutedEventArgs e)
        {
            ClientUC clientUC = new ClientUC();
            clientUC.CloseRequested += UC_CloseRequested;
            ContentArea.Content = clientUC;
        }

        private void btnAnimals_Click(object sender, RoutedEventArgs e)
        {
            AnimalUC animalUC = new AnimalUC();
            animalUC.CloseRequested += UC_CloseRequested;
            ContentArea.Content = animalUC;
        }

        private void btnDoctors_Click(object sender, RoutedEventArgs e)
        {
            DoctorUC doctorUC = new DoctorUC();
            doctorUC.CloseRequested += UC_CloseRequested;
            ContentArea.Content = doctorUC;
        }

        private void btnRooms_Click(object sender, RoutedEventArgs e)
        {
            RoomUC roomUC = new RoomUC();
            roomUC.CloseRequested += UC_CloseRequested;
            ContentArea.Content = roomUC;
        }

        private void btnAppointments_Click(object sender, RoutedEventArgs e)
        {
            AppointmentUC appointmentUC = new AppointmentUC();
            appointmentUC.CloseRequested += UC_CloseRequested;
            ContentArea.Content = appointmentUC;
        }

        private void btnCredits_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UC_CloseRequested(object sender, EventArgs e)
        {
            ContentArea.Content = null;
        }
    }
}
