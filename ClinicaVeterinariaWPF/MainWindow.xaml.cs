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
            SelectButton(btnHome);
            ContentArea.Content = new MainUC();
        }

        private void btnClients_Click(object sender, RoutedEventArgs e)
        {
            ClientUC clientUC = new ClientUC();
            clientUC.CloseRequested += UC_CloseRequested;
            ContentArea.Content = clientUC;
            SelectButton(btnClients);
        }

        private void btnAnimals_Click(object sender, RoutedEventArgs e)
        {
            AnimalUC animalUC = new AnimalUC();
            animalUC.CloseRequested += UC_CloseRequested;
            ContentArea.Content = animalUC;
            SelectButton(btnAnimals);
        }

        private void btnDoctors_Click(object sender, RoutedEventArgs e)
        {
            DoctorUC doctorUC = new DoctorUC();
            doctorUC.CloseRequested += UC_CloseRequested;
            ContentArea.Content = doctorUC;
            SelectButton(btnDoctors);
        }

        private void btnRooms_Click(object sender, RoutedEventArgs e)
        {
            RoomUC roomUC = new RoomUC();
            roomUC.CloseRequested += UC_CloseRequested;
            ContentArea.Content = roomUC;
            SelectButton(btnRooms);
        }

        private void btnAppointments_Click(object sender, RoutedEventArgs e)
        {
            AppointmentUC appointmentUC = new AppointmentUC();
            appointmentUC.CloseRequested += UC_CloseRequested;
            ContentArea.Content = appointmentUC;
            SelectButton(btnAppointments);
        }

        private void btnCredits_Click(object sender, RoutedEventArgs e)
        {
            popupCredits.IsOpen = !popupCredits.IsOpen;
        }

        private void UC_CloseRequested(object sender, EventArgs e)
        {
            ContentArea.Content = new MainUC();
            SelectButton(btnHome);
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            MainUC mainUC = new MainUC();
            ContentArea.Content = mainUC;
            SelectButton(btnHome);
        }

        private void ResetButtons()
        {
            btnHome.ClearValue(Button.BackgroundProperty);
            btnClients.ClearValue(Button.BackgroundProperty);
            btnAnimals.ClearValue(Button.BackgroundProperty);
            btnDoctors.ClearValue(Button.BackgroundProperty);
            btnRooms.ClearValue(Button.BackgroundProperty);
            btnAppointments.ClearValue(Button.BackgroundProperty);
            btnCredits.ClearValue(Button.BackgroundProperty);
        }

        private void SelectButton(Button button)
        {
            ResetButtons();

            button.Background = new SolidColorBrush(Color.FromRgb(34, 79, 79));
        }
    }
}
