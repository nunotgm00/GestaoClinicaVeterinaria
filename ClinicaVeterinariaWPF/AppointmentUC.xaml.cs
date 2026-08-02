using ClinicaVeterinariaWPF.Models;
using ClinicaVeterinariaWPF.Services;
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
    public partial class AppointmentUC : UserControl
    {
        private readonly ApiService apiService = new ApiService();
        private const string urlBase = "http://gestaoclinicaveterinariaapi.somee.com/api";
        private List<Animal> animals = new List<Animal>();
        private List<Appointment> appointments = new List<Appointment>();
        private List<Doctor> doctors = new List<Doctor>();
        private List<Room> rooms = new List<Room>();
        private List<AppointmentHelper> appointmentHelpers = new List<AppointmentHelper>();
        private List<AppointmentHelper> appointmentHelpersSearch = new List<AppointmentHelper>();
        private List<DoctorSchedule> doctorSchedules = new List<DoctorSchedule>();
        private int selectedAppointmentId = -1;
        public AppointmentUC()
        {
            InitializeComponent();
            this.Loaded += AppointmentWindow_Loaded;
        }

        private async void AppointmentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAnimals();
            await LoadDoctors();
            await LoadRooms();
            await LoadAppointments();
            await LoadDoctorSchedules();
        }

        private async Task LoadAnimals()
        {
            var response = await apiService.GetAllAsync<Animal>(urlBase, "Animal");

            if (response.IsSuccess)
            {
                animals = (List<Animal>)response.Result;

                ComboBoxAnimal.ItemsSource = animals;
            }
            else
            {
                MessageBox.Show("Erro ao carregar animal: " + response.Message);
            }
        }

        private async Task LoadAppointments()
        {
            var response = await apiService.GetAllAsync<Appointment>(urlBase, "Appointment");

            if (response.IsSuccess)
            {
                appointments = (List<Appointment>)response.Result;
                appointmentHelpers.Clear();

                foreach (var appointment in appointments)
                {
                    AppointmentHelper helper = new AppointmentHelper()
                    {
                        Id = appointment.Id,
                        Date = appointment.Date,
                        StartTime = appointment.StartTime,
                        EndTime = appointment.EndTime,
                    };

                    if (appointment.Canceled)
                    {
                        helper.Canceled = "Cancelado";
                    }

                    foreach (var animal in animals)
                    {
                        if (animal.Id == appointment.AnimalId)
                        {
                            helper.AnimalName = animal.Name;
                        }
                    }

                    foreach (var doctor in doctors)
                    {
                        if (doctor.Id == appointment.DoctorId)
                        {
                            helper.DoctorName = doctor.Name;
                        }
                    }

                    foreach (var room in rooms)
                    {
                        if (room.Id == appointment.RoomId)
                        {
                            helper.RoomName = "Sala " + room.Id;
                        }
                    }

                    appointmentHelpers.Add(helper);
                }

                DataGridSearch.ItemsSource = null;
                DataGridSearch.ItemsSource = appointmentHelpers;
            }
            else
            {
                MessageBox.Show("Erro ao carregar consulta: " + response.Message);
            }
        }

        private async Task LoadDoctors()
        {
            var response = await apiService.GetAllAsync<Doctor>(urlBase, "Doctor");

            if (response.IsSuccess)
            {
                doctors = (List<Doctor>)response.Result;

                ComboBoxDoctor.ItemsSource = doctors;
            }
            else
            {
                MessageBox.Show("Erro ao carregar doutor: " + response.Message);
            }
        }

        private async Task LoadRooms()
        {
            var response = await apiService.GetAllAsync<Room>(urlBase, "Room");

            if (response.IsSuccess)
            {
                rooms = (List<Room>)response.Result;

                ComboBoxRoom.ItemsSource = rooms;
            }
            else
            {
                MessageBox.Show("Erro ao carregar sala: " + response.Message);
            }
        }

        private async Task LoadDoctorSchedules()
        {
            var response = await apiService.GetAllAsync<DoctorSchedule>(urlBase, "DoctorSchedule");

            if (response.IsSuccess)
            {
                doctorSchedules = (List<DoctorSchedule>)response.Result;
            }
            else
            {
                MessageBox.Show("Erro ao carregar horário: " + response.Message);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxSearch.SelectedItem == null && DatePickerSearch.SelectedDate == null)
            {
                MessageBox.Show("Nenhum item de procura selecionado");
                return;
            }

            appointmentHelpersSearch = new List<AppointmentHelper>(appointmentHelpers);

            if (DatePickerSearch.SelectedDate != null)
            {
                foreach (var appointmentHelper in appointmentHelpers)
                {
                    if (appointmentHelper.Date != DatePickerSearch.SelectedDate)
                    {
                        appointmentHelpersSearch.Remove(appointmentHelper);
                    }
                }
            }
            if (ComboBoxSearch.SelectedIndex == 0)
            {
                foreach (var appointmentHelper in appointmentHelpers)
                {
                    if (appointmentHelper.AnimalName.IndexOf(TextBoxSearch.Text, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        appointmentHelpersSearch.Remove(appointmentHelper);
                    }
                }
            }
            else if (ComboBoxSearch.SelectedIndex == 1)
            {
                foreach (var appointmentHelper in appointmentHelpers)
                {
                    if (appointmentHelper.DoctorName.IndexOf(TextBoxSearch.Text, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        appointmentHelpersSearch.Remove(appointmentHelper);
                    }
                }
            }

            DataGridSearch.ItemsSource = null;
            DataGridSearch.ItemsSource = appointmentHelpersSearch;
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            AppointmentHelper selectedHelper = DataGridSearch.SelectedItem as AppointmentHelper;

            if (selectedHelper == null)
            {
                MessageBox.Show("Nenhum animal selecionado");
                return;
            }

            Appointment selected = new Appointment();
            Doctor selectedDoctor = new Doctor();
            Animal selectedAnimal = new Animal();
            Room selectedRoom = new Room();

            foreach (var appointment in appointments)
            {
                if (appointment.Id == selectedHelper.Id)
                {
                    selected = appointment;
                }
            }

            foreach (var doctor in doctors)
            {
                if (selected.DoctorId == doctor.Id)
                {
                    selectedDoctor = doctor;
                }
            }

            foreach (var animal in animals)
            {
                if (selected.AnimalId == animal.Id)
                {
                    selectedAnimal = animal;
                }
            }

            foreach (var room in rooms)
            {
                if (selected.RoomId == room.Id)
                {
                    selectedRoom = room;
                }
            }

            if (selected.Canceled)
            {
                CheckBoxCanceled.IsChecked = true;
            }

            selectedAppointmentId = selected.Id;
            ComboBoxAnimal.SelectedItem = selectedAnimal;
            ComboBoxDoctor.SelectedItem = selectedDoctor;
            ComboBoxRoom.SelectedItem = selectedRoom;
            TextBoxMotive.Text = selected.Motive;
            TextBoxTreatment.Text = selected.Treatment;
            DatePickerDate.SelectedDate = selected.Date;
            TimePickerStart.Value = DateTime.Today.Add(selected.StartTime);
            TimePickerEnd.Value = DateTime.Today.Add(selected.EndTime);

            await LoadAppointments();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ClearTools();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Validation())
            {
                DeactivateButtons();

                Animal animal = (Animal)ComboBoxAnimal.SelectedValue;
                Doctor doctor = (Doctor)ComboBoxDoctor.SelectedValue;
                Room room = (Room)ComboBoxRoom.SelectedValue;

                Appointment appointment = new Appointment()
                {
                    AnimalId = animal.Id,
                    DoctorId = doctor.Id,
                    RoomId = room.Id,
                    Motive = TextBoxMotive.Text,
                    Treatment = TextBoxTreatment.Text,
                    Canceled = (bool)CheckBoxCanceled.IsChecked,
                    Date = (DateTime)DatePickerDate.SelectedDate,
                    StartTime = ToTimeSpan(TimePickerStart.Value),
                    EndTime = ToTimeSpan(TimePickerEnd.Value),
                };

                Response response;

                if (selectedAppointmentId == -1)
                {
                    response = await apiService.PostAsync(urlBase, "appointment", appointment);
                }
                else
                {
                    appointment.Id = selectedAppointmentId;

                    response = await apiService.PutAsync(urlBase, "appointment", appointment, selectedAppointmentId);
                }

                if (response.IsSuccess)
                {
                    MessageBox.Show("Consulta guardada com sucesso");
                    ClearTools();
                    await LoadAppointments();
                }
                else
                {
                    MessageBox.Show("Erro: " + response.Message);
                }

                ActivateButtons();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClearTools()
        {
            ComboBoxAnimal.SelectedItem = null;
            ComboBoxDoctor.SelectedItem = null;
            ComboBoxRoom.SelectedItem = null;
            TextBoxMotive.Text = string.Empty;
            TextBoxTreatment.Text = string.Empty;
            DatePickerDate.SelectedDate = null;
            TimePickerStart.Value = null;
            TimePickerEnd.Value = null;
            CheckBoxCanceled.IsChecked = false;

            selectedAppointmentId = -1;
        }

        private TimeSpan ToTimeSpan(DateTime? dateTime)
        {
            return (TimeSpan)dateTime.Value.TimeOfDay;
        }

        private void btnCleanDate_Click(object sender, RoutedEventArgs e)
        {
            DatePickerSearch.SelectedDate = null;
        }

        private bool Validation()
        {
            if (ComboBoxAnimal.SelectedValue == null)
            {
                MessageBox.Show("Insira um animal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (ComboBoxDoctor.SelectedValue == null)
            {
                MessageBox.Show("Insira um doutor", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (ComboBoxRoom.SelectedValue == null)
            {
                MessageBox.Show("Insira uma sala", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(TextBoxMotive.Text))
            {
                MessageBox.Show("Insira um motivo válido", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(TextBoxTreatment.Text))
            {
                MessageBox.Show("Insira um tratamento válido", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (DatePickerDate.SelectedDate == null)
            {
                MessageBox.Show("Insira uma data válida", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (TimePickerStart.Value == null)
            {
                MessageBox.Show("Insira uma hora de início válida", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (TimePickerEnd.Value == null)
            {
                MessageBox.Show("Insira uma hora de fim válida", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            DateTime date = (DateTime)DatePickerDate.SelectedDate;
            TimeSpan start = ToTimeSpan(TimePickerStart.Value);
            TimeSpan end = ToTimeSpan(TimePickerEnd.Value);

            Animal animal = (Animal)ComboBoxAnimal.SelectedValue;
            Doctor doctor = (Doctor)ComboBoxDoctor.SelectedValue;
            Room room = (Room)ComboBoxRoom.SelectedValue;
            List<DoctorSchedule> doctorSchedulesValidation = new List<DoctorSchedule>();

            if (start >= end)
            {
                MessageBox.Show("Horário inválido", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            foreach (var doctorSchedule in doctorSchedules)
            {
                if (doctorSchedule.DoctorId == doctor.Id)
                {
                    doctorSchedulesValidation.Add(doctorSchedule);
                }
            }

            if (CheckBoxCanceled.IsChecked == false)
            {
                foreach (Appointment appointment in appointments)
                {
                    if (appointment.Id != selectedAppointmentId)
                    {
                        if (animal.Id == appointment.AnimalId)
                        {
                            if (appointment.Date == date && appointment.Canceled == false)
                            {
                                if (start < appointment.EndTime && end > appointment.StartTime)
                                {
                                    MessageBox.Show("O animal já tem marcação neste horário", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return false;
                                }
                            }
                        }
                    }
                }

                bool doesntWork = true;

                foreach (var doctorSchedule in doctorSchedulesValidation)
                {
                    if ((doctorSchedule.StartTime != TimeSpan.Zero && doctorSchedule.EndTime != TimeSpan.Zero) && doctorSchedule.DayOfWeek == (byte)date.DayOfWeek)
                    {
                        doesntWork = false;

                        if (start >= doctorSchedule.StartTime && end <= doctorSchedule.EndTime)
                        {
                            foreach (Appointment appointment in appointments)
                            {
                                if (appointment.Id != selectedAppointmentId)
                                {
                                    if (appointment.DoctorId == doctor.Id)
                                    {
                                        if (appointment.Date == date && appointment.Canceled == false)
                                        {
                                            if (start < appointment.EndTime && end > appointment.StartTime)
                                            {
                                                MessageBox.Show("O doutor já tem marcação neste horário", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("O doutor não trabalha nesse horário", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                            return false;
                        }
                    }
                }

                if (doesntWork)
                {
                    MessageBox.Show("O doutor não trabalha nesse dia", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                foreach (Appointment appointment in appointments)
                {
                    if (appointment.Id != selectedAppointmentId)
                    {
                        if (appointment.RoomId == room.Id)
                        {
                            if (appointment.Date == date && appointment.Canceled == false)
                            {
                                if (start < appointment.EndTime && end > appointment.StartTime)
                                {
                                    MessageBox.Show("Esta sala está ocupada no horário selecionado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        private void DeactivateButtons()
        {
            btnSearch.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnNew.IsEnabled = false;
            btnSave.IsEnabled = false;
            btnClose.IsEnabled = false;
            btnCleanDate.IsEnabled = false;
        }

        private void ActivateButtons()
        {
            btnSearch.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnNew.IsEnabled = true;
            btnSave.IsEnabled = true;
            btnClose.IsEnabled = true;
            btnCleanDate.IsEnabled = true;
        }

        private async void btnAllList_Click(object sender, RoutedEventArgs e)
        {
            await LoadAppointments();
        }
    }
}

