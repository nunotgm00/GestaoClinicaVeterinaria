using ClinicaVeterinariaWPF.Models;
using ClinicaVeterinariaWPF.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace ClinicaVeterinariaWPF
{
    /// <summary>
    /// Interaction logic for DoctorWindow.xaml
    /// </summary>
    public partial class DoctorWindow : Window
    {
        private readonly ApiService apiService = new ApiService();
        private const string urlBase = "http://gestaoclinicaveterinariaapi.somee.com/api";
        private int? selectedDoctorId = -1;
        private List<Doctor> doctors = new List<Doctor>();
        private List<Doctor> searchDoctors = new List<Doctor>();
        private List<Appointment> appointments = new List<Appointment>();
        private List<DoctorSchedule> doctorSchedules = new List<DoctorSchedule>();
        private List<DaySchedule> daySchedules = new List<DaySchedule>();

        public DoctorWindow()
        {
            InitializeComponent();
            this.Loaded += DoctorWindow_Loaded;
        }

        private async void DoctorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDoctors();
            await LoadDoctorSchedules();
            await LoadAppointments();
            CreateSchedule();
        }

        private async Task LoadDoctors()
        {
            var response = await apiService.GetAllAsync<Doctor>(urlBase, "Doctor");

            if (response.IsSuccess)
            {
                doctors = (List<Doctor>)response.Result;
                DataGridSearch.ItemsSource = doctors;
            }
            else
            {
                MessageBox.Show("Erro ao carregar doutor: " + response.Message);
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
                MessageBox.Show("Erro ao carregar horários: " + response.Message);
            }
        }
        private async Task LoadAppointments()
        {
            var response = await apiService.GetAllAsync<Appointment>(urlBase, "Appointment");

            if (response.IsSuccess)
            {
                appointments = (List<Appointment>)response.Result;
            }
            else
            {
                MessageBox.Show("Erro ao carregar consultas: " + response.Message);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            searchDoctors.Clear();

            if (ComboBoxSearch.SelectedIndex == 0)
            {
                foreach (var doctor in doctors)
                {
                    if (doctor.Name.IndexOf(TextBoxSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        searchDoctors.Add(doctor);
                    }
                }
                DataGridSearch.Items.Refresh();
            }
            else if (ComboBoxSearch.SelectedIndex == 1)
            {
                foreach (var doctor in doctors)
                {
                    if (doctor.Speciality.IndexOf(TextBoxSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        searchDoctors.Add(doctor);
                    }
                }
                DataGridSearch.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Nenhum item de procura selecionado");
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Doctor selected = DataGridSearch.SelectedItem as Doctor;

            if (selected == null)
            {
                MessageBox.Show("Nenhum doutor selecionado");
            }
            else
            {
                selectedDoctorId = selected.Id;
                TextBoxName.Text = selected.Name;
                TextBoxPhoneNumber.Text = selected.PhoneNumber;
                TextBoxSpeciality.Text = selected.Speciality;

                CreateSchedule();
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Doctor selected = DataGridSearch.SelectedItem as Doctor;

            if (selected == null)
            {
                MessageBox.Show("Selecione um Doutor para eliminar");
                return;
            }
            else
            {
                foreach(var appointment in appointments)
                {
                    if(appointment.DoctorId == selected.Id)
                    {
                        MessageBox.Show("O doutor não pode ser eliminado pois tem consultas marcadas");
                        return;
                    }
                }
                foreach (var doctorSchedule in doctorSchedules)
                {
                    if (doctorSchedule.DoctorId == selected.Id)
                    {
                        var response = await apiService.DeleteAsync(urlBase, "doctorSchedule", doctorSchedule.Id);

                        if (!response.IsSuccess)
                        {
                            MessageBox.Show("Erro: " + response.Message);
                            return;
                        }
                    }
                }

                var response2 = await apiService.DeleteAsync(urlBase, "doctor", selected.Id);

                if (response2.IsSuccess)
                {
                    MessageBox.Show("Doutor eliminado");
                    ClearForm();
                    await LoadDoctors();
                    await LoadDoctorSchedules();
                    await LoadAppointments();
                }
                else
                {
                    MessageBox.Show("Erro: " + response2.Message);
                }
            }
        }

        private void btnAddHour_Click(object sender, RoutedEventArgs e)
        {
            DaySchedule selected = DataGridSchedule.SelectedItem as DaySchedule;
            if (selected == null) return;

            selected.Start = ToTimeSpan(TimePickerStart.Value);
            selected.End = ToTimeSpan(TimePickerEnd.Value);

            DataGridSchedule.Items.Refresh();
        }

        private void btnDayOff_Click(object sender, RoutedEventArgs e)
        {
            DaySchedule selected = DataGridSchedule.SelectedItem as DaySchedule;
            if (selected == null) return;

            selected.Start = TimeSpan.Zero;
            selected.End = TimeSpan.Zero;

            DataGridSchedule.Items.Refresh();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Doctor doctor = new Doctor()
            {
                Name = TextBoxName.Text,
                PhoneNumber = TextBoxPhoneNumber.Text,
                Speciality = TextBoxSpeciality.Text,
                Active = (bool)CheckBoxActive.IsChecked
            };

            Response response;

            if (selectedDoctorId == -1)
            {
                response = await apiService.PostAsync(urlBase, "doctor", doctor);
            }
            else
            {
                doctor.Id = selectedDoctorId.Value;
                response = await apiService.PutAsync(urlBase, "doctor", doctor, selectedDoctorId.Value);
            }

            if (response.IsSuccess)
            {
                if (selectedDoctorId == -1)
                {
                    Doctor createdDoctor = JsonConvert.DeserializeObject<Doctor>((string)response.Result);
                    doctor.Id = createdDoctor.Id;

                    for (int i = 0; i < 7; i++)
                    {
                        foreach (var daySchedule in daySchedules)
                        {
                            if (daySchedule.Order == i)
                            {
                                DoctorSchedule doctorSchedule = new DoctorSchedule()
                                {
                                    DoctorId = doctor.Id,
                                    DayOfWeek = (byte)i,
                                    StartTime = daySchedule.Start,
                                    EndTime = daySchedule.End,
                                };

                                response = await apiService.PostAsync(urlBase, "doctorSchedule", doctorSchedule);

                                if (!response.IsSuccess)
                                {
                                    MessageBox.Show("Erro: " + response.Message);
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 7; i++)
                    {
                        foreach (var doctorSchedule in doctorSchedules)
                        {
                            if (doctorSchedule.DoctorId == doctor.Id)
                            {
                                if(doctorSchedule.DayOfWeek == (byte)i)
                                {
                                    foreach (var daySchedule in daySchedules)
                                    {
                                        if (daySchedule.Order == i)
                                        {
                                            DoctorSchedule editedDoctorSchedule = new DoctorSchedule()
                                            {
                                                Id = doctorSchedule.Id,
                                                DoctorId = doctor.Id,
                                                DayOfWeek = (byte)i,
                                                StartTime = daySchedule.Start,
                                                EndTime = daySchedule.End,
                                            };

                                            response = await apiService.PutAsync(urlBase, "doctorSchedule", editedDoctorSchedule, editedDoctorSchedule.Id);

                                            if (!response.IsSuccess)
                                            {
                                                MessageBox.Show("Erro: " + response.Message);
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                MessageBox.Show("Doutor guardado com sucesso");
                ClearForm();
                await LoadDoctors();
                await LoadDoctorSchedules();
            }
            else
            {
                MessageBox.Show("Erro: " + response.Message);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CreateSchedule()
        {
            string[] days = { "Domingo", "Segunda", "Terça-feira", "Quarta-feira", "Quinta-Feira", "Sexta-feira", "Sábado"};

            daySchedules.Clear();

            for (int i = 0; i < 7; i++)
            {
                DaySchedule daySchedule = new DaySchedule()
                {
                    Order = i,
                    DayOfWeek = days[i],
                    Start = TimeSpan.Zero,
                    End = TimeSpan.Zero
                };

                daySchedules.Add(daySchedule);
            }

            if (selectedDoctorId != -1)
            {
                foreach (var doctorSchedule in doctorSchedules)
                {
                    if (selectedDoctorId == doctorSchedule.DoctorId)
                    {
                        foreach (var daySchedule in daySchedules)
                        {
                            if (daySchedule.Order == doctorSchedule.DayOfWeek)
                            {
                                daySchedule.Start = doctorSchedule.StartTime;
                                daySchedule.End = doctorSchedule.EndTime;
                            }
                        }
                    }
                }
            }
            DataGridSchedule.Items.Refresh();
        }

        private void ClearForm()
        {
            selectedDoctorId = -1;
            TextBoxName.Text = "";
            TextBoxPhoneNumber.Text = "";
            TextBoxSpeciality.Text = "";
            CreateSchedule();
        }

        private TimeSpan ToTimeSpan(DateTime? dateTime)
        {
            return (TimeSpan)dateTime.Value.TimeOfDay;
        }
    }
}
