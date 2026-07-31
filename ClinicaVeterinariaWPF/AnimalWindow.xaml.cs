using ClinicaVeterinariaWPF.Models;
using ClinicaVeterinariaWPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ClinicaVeterinariaWPF
{
    /// <summary>
    /// Interaction logic for AnimalWindow.xaml
    /// </summary>
    public partial class AnimalWindow : Window
    {
        private readonly ApiService apiService = new ApiService();
        private const string urlBase = "http://gestaoclinicaveterinariaapi.somee.com/api";
        private int? selectedAnimalId = -1;
        private List<Animal> animals = new List<Animal>();
        private List<Animal> searchAnimals = new List<Animal>();
        private List<Client> clients = new List<Client>();
        private List<Appointment> appointments = new List<Appointment>();
        private List<Appointment> appointmentsAnimal = new List<Appointment>();
        private List<AppointmentHelper> appointmentHelpers = new List<AppointmentHelper>();

        public AnimalWindow()
        {
            InitializeComponent();
            this.Loaded += AnimalWindow_Loaded;
        }

        private async void AnimalWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadClients();
            await LoadAnimals();
            await LoadAppointments();
        }

        private async Task LoadAnimals()
        {
            var response = await apiService.GetAllAsync<Animal>(urlBase, "Animal");

            if (response.IsSuccess)
            {
                animals = (List<Animal>)response.Result;

                foreach (var animal in animals)
                {
                    if (animal.ClientId == null)
                    {
                        animal.ClientName = "Sem dono";
                    }
                    else
                    {
                        foreach (var client in clients)
                        {
                            if (animal.ClientId == client.Id)
                            {
                                animal.ClientName = client.Name;
                            }
                        }
                    }
                }
                DataGridSearch.ItemsSource = animals;
            }
            else
            {
                MessageBox.Show("Erro ao carregar animal: " + response.Message);
            }
        }

        private async Task LoadClients()
        {
            var response = await apiService.GetAllAsync<Client>(urlBase, "Client");

            if (response.IsSuccess)
            {
                clients = (List<Client>)response.Result;
            }
            else
            {
                MessageBox.Show("Erro ao carregar cliente: " + response.Message);
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
            searchAnimals.Clear();

            if (ComboBoxSearch.SelectedIndex == 0)
            {
                foreach (var animal in animals)
                {
                    if (animal.Name.IndexOf(TextBoxSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        searchAnimals.Add(animal);
                    }
                }
                DataGridSearch.Items.Refresh();
            }
            else if (ComboBoxSearch.SelectedIndex == 1)
            {
                foreach (var animal in animals)
                {
                    if (animal.Species.IndexOf(TextBoxSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        searchAnimals.Add(animal);
                    }
                }
                DataGridSearch.Items.Refresh();
            }
            else if (ComboBoxSearch.SelectedIndex == 2)
            {
                foreach (var client in clients)
                {
                    if (client.Name.IndexOf(TextBoxSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        foreach (var animal in animals)
                        {
                            if (animal.ClientId == client.Id)
                            {
                                searchAnimals.Add(animal);
                            }
                        }
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
            Animal selected = DataGridSearch.SelectedItem as Animal;

            if (selected == null)
            {
                MessageBox.Show("Nenhum animal selecionado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                selectedAnimalId = selected.Id;
                TextBoxName.Text = selected.Name;
                TextBoxSpecies.Text = selected.Species;
                TextBoxBreed.Text = selected.Breed;
                TextBoxAge.Text = selected.Age.ToString();
                TextBoxWeight.Text = selected.Weight.ToString();
                TextBoxColor.Text = selected.Color;
                if (selected.Sex == "Masculino")
                {
                    ComboBoxSex.SelectedIndex = 0;
                }
                else if (selected.Sex == "Feminino")
                {
                    ComboBoxSex.SelectedIndex = 1;
                }
                else if (selected.Sex == "Outro")
                {
                    ComboBoxSex.SelectedIndex = 2;
                }

                appointmentsAnimal.Clear();
                appointmentHelpers.Clear();

                foreach (var appointment in appointments)
                {
                    if (appointment.AnimalId == selectedAnimalId)
                    {
                        appointmentsAnimal.Add(appointment);

                        foreach(var appointment2 in appointmentsAnimal)
                        {
                            AppointmentHelper appointmentHelper = new AppointmentHelper()
                            {
                                Date = appointment2.Date,
                                StartTime = appointment2.StartTime,
                                EndTime = appointment2.EndTime,
                                Motive = appointment2.Motive,
                                RoomName = "Sala " + appointment2.RoomId,
                            };

                            appointmentHelpers.Add(appointmentHelper);
                        }
                    }
                }

                DataGridAppointments.ItemsSource = null;
                DataGridAppointments.ItemsSource = appointmentHelpers;

                DataGridSearch.Items.Refresh();

                await LoadAnimals();
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Animal selected = DataGridSearch.SelectedItem as Animal;

            if (selected == null)
            {
                MessageBox.Show("Selecione um animal para eliminar", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(selected.ClientId != null)
            {
                MessageBox.Show("Animal não pode ser eliminado pois tem um cliente associado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var response = await apiService.DeleteAsync(urlBase, "animal", selected.Id);

            if (response.IsSuccess)
            {
                MessageBox.Show("Animal eliminado");
                ClearForm();
                await LoadClients();
                await LoadAnimals();
            }
            else
            {
                MessageBox.Show("Erro: " + response.Message);
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Validation())
            {
                ComboBoxItem selected = ComboBoxSex.SelectedItem as ComboBoxItem;

                string value = selected.Content.ToString();

                Animal animal = new Animal()
                {
                    Name = TextBoxName.Text,
                    Species = TextBoxSpecies.Text,
                    Breed = TextBoxBreed.Text,
                    Age = Convert.ToInt32(TextBoxAge.Text),
                    Weight = Convert.ToInt32(TextBoxWeight.Text),
                    Color = TextBoxColor.Text,
                    Sex = value
                };

                Response response;

                if (selectedAnimalId == -1)
                {
                    response = await apiService.PostAsync(urlBase, "animal", animal);
                }
                else
                {
                    animal.Id = selectedAnimalId.Value;

                    foreach (var animalEdited in animals)
                    {
                        if (animal.Id == animalEdited.Id)
                        {
                            animal.ClientId = animalEdited.ClientId;
                        }
                    }

                    response = await apiService.PutAsync(urlBase, "animal", animal, selectedAnimalId.Value);
                }

                if (response.IsSuccess)
                {
                    MessageBox.Show("Animal guardado com sucesso");
                    ClearForm();
                    await LoadAnimals();
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

        private void ClearForm()
        {
            selectedAnimalId = -1;
            TextBoxName.Text = "";
            TextBoxSpecies.Text = "";
            TextBoxBreed.Text = "";
            TextBoxAge.Text = "";
            TextBoxWeight.Text = "";
            TextBoxColor.Text = "";
            ComboBoxSex.SelectedIndex = 2;

            DataGridAppointments.ItemsSource = null;
        }

        private bool Validation()
        {
            if (string.IsNullOrEmpty(TextBoxName.Text))
            {
                MessageBox.Show("Insira nome do animal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(TextBoxSpecies.Text))
            {
                MessageBox.Show("Insira a espécie do animal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(TextBoxBreed.Text))
            {
                MessageBox.Show("Insira a raça do animal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!(TextBoxAge.Text.All(char.IsDigit)) || string.IsNullOrEmpty(TextBoxAge.Text))
            {
                MessageBox.Show("Insira idade válida do animal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!(TextBoxWeight.Text.All(char.IsDigit)) || string.IsNullOrEmpty(TextBoxWeight.Text))
            {
                MessageBox.Show("Insira o peso válido do animal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(TextBoxColor.Text) || (TextBoxColor.Text.Any(char.IsDigit)))
            {
                MessageBox.Show("Insira a cor do animal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (ComboBoxSex.SelectedValue == null)
            {
                MessageBox.Show("Insira o sexo do animal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}
