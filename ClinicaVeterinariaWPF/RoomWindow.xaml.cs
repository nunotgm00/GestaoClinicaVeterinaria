using ClinicaVeterinariaWPF.Models;
using ClinicaVeterinariaWPF.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ClinicaVeterinariaWPF
{
    /// <summary>
    /// Interaction logic for RoomWindow.xaml
    /// </summary>
    public partial class RoomWindow : Window
    {
        private ApiService apiService = new ApiService();
        private const string urlBase = "http://gestaoclinicaveterinariaapi.somee.com/api";
        private int? selectedRoomId = -1;
        private List<Room> rooms = new List<Room>();
        private List<Room> searchRooms = new List<Room>();
        private List<Appointment> appointments = new List<Appointment>();

        public RoomWindow()
        {
            InitializeComponent();
            this.Loaded += RoomWindow_Loaded;
        }

        private async void RoomWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadRooms();
            await LoadAppointments();
        }

        private async Task LoadRooms()
        {
            var response = await apiService.GetAllAsync<Room>(urlBase, "Room");

            if (response.IsSuccess)
            {
                rooms = (List<Room>)response.Result;
                DataGridSearch.ItemsSource = rooms;
            }
            else
            {
                MessageBox.Show("Erro ao carregar sala: " + response.Message);
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
                MessageBox.Show("Erro ao carregar consulta: " + response.Message);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            searchRooms.Clear();

            if (ComboBoxSearch.SelectedIndex == 0)
            {
                foreach (var room in rooms)
                {
                    if (room.Id == Convert.ToInt32(TextBoxSearch.Text))
                    {
                        searchRooms.Add(room);
                    }
                }
                DataGridSearch.Items.Refresh();
            }
            else if (ComboBoxSearch.SelectedIndex == 1)
            {
                foreach (var room in rooms)
                {
                    if (room.Type.IndexOf(TextBoxSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        searchRooms.Add(room);
                    }
                }
                DataGridSearch.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Nenhum item de procura selecionado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Room selected = DataGridSearch.SelectedItem as Room;

            if (selected == null)
            {
                MessageBox.Show("Nenhuma sala selecionada", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                selectedRoomId = selected.Id;
                TextBoxRoomNumber.Text = selected.Id.ToString();
                TextBoxType.Text = selected.Type;
                CheckBoxUnderMaintenance.IsChecked = selected.UnderMaintenance;

                await LoadRooms();
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Room selected = DataGridSearch.SelectedItem as Room;


            if (selected == null)
            {
                MessageBox.Show("Selecione uma sala para eliminar", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach(var appointment in appointments)
            {
                if(appointment.RoomId == selected.Id)
                {
                    MessageBox.Show("Esta sala tem consultas marcadas e não pode ser eliminada", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            var response = await apiService.DeleteAsync(urlBase, "room", selected.Id);

            if (response.IsSuccess)
            {
                MessageBox.Show("Sala eliminada");
                ClearTools();
                await LoadRooms();
            }
            else
            {
                MessageBox.Show("Erro: " + response.Message);
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ClearTools();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Validation())
            {
                Room room = new Room()
                {
                    Type = TextBoxType.Text,
                    UnderMaintenance = (bool)CheckBoxUnderMaintenance.IsChecked
                };

                bool responsePost = false;
                Response response;

                if (selectedRoomId == -1)
                {
                    response = await apiService.PostAsync(urlBase, "room", room);
                    responsePost = true;
                }
                else
                {
                    room.Id = selectedRoomId.Value;
                    response = await apiService.PutAsync(urlBase, "room", room, selectedRoomId.Value);
                    responsePost = false;
                }

                if (response.IsSuccess)
                {
                    await LoadRooms();

                    if (responsePost)
                    {
                        int maxId = 0;

                        foreach (var roomMax in rooms)
                        {
                            if (roomMax.Id > maxId)
                            {
                                maxId = roomMax.Id;
                            }
                        }

                        MessageBox.Show($"Sala guardada com sucesso\n\nID da nova sala: {maxId}");
                    }
                    else
                    {
                        MessageBox.Show("Sala guardada com sucesso");
                    }
                    ClearTools();
                }
                else
                {
                    MessageBox.Show("Erro: " + response.Message);
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClearTools()
        {
            selectedRoomId = -1;
            TextBoxRoomNumber.Text = "";
            TextBoxType.Text = "";
            CheckBoxUnderMaintenance.IsChecked = false;
        }

        private bool Validation()
        {
            if (string.IsNullOrEmpty(TextBoxType.Text))
            {
                MessageBox.Show("Insira o tipo da sala", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}
