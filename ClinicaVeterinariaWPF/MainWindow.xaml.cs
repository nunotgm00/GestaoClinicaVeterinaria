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
            ContentArea.Content = new ClientUC();
        }

        private void btnAnimals_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new AnimalUC();
        }

        private void btnDoctors_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new DoctorUC();
        }

        private void btnRooms_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new RoomUC();
        }

        private void btnAppointments_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new AppointmentUC();
        }

        private void btnCredits_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
