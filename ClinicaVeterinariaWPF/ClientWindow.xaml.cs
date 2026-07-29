using ClinicaVeterinariaWPF.Models;
using ClinicaVeterinariaWPF.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace ClinicaVeterinariaWPF
{
    public partial class ClientWindow : Window
    {
        private ApiService apiService = new ApiService();
        private const string urlBase = "http://gestaoclinicaveterinariaapi.somee.com/api";
        private int? selectedClientId = -1;
        private List<Client> clients = new List<Client>();
        private List<Client> searchClients = new List<Client>();
        private List<Animal> animalsClient = new List<Animal>();
        private List<Animal> animalsWithoutClient = new List<Animal>();


        public ClientWindow()
        {
            InitializeComponent();
            this.Loaded += ClientWindow_Loaded;
        }

        private async void ClientWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadClients();
            await LoadAnimals();
        }

        private async Task LoadClients()
        {
            var response = await apiService.GetAllAsync<Client>(urlBase, "Client");

            if (response.IsSuccess)
            {
                clients = (List<Client>)response.Result;
                DataGridSearch.ItemsSource = clients;
            }
            else
            {
                MessageBox.Show("Erro ao carregar cliente: " + response.Message);
            }
        }

        private async Task LoadAnimals()
        {
            var response = await apiService.GetAllAsync<Animal>(urlBase, "Animal");

            if (response.IsSuccess)
            {
                List<Animal> animals = (List<Animal>)response.Result;

                animalsWithoutClient.Clear();
                animalsClient.Clear();

                foreach (var animal in animals)
                {
                    if (animal.ClientId == null)
                    {
                        animalsWithoutClient.Add(animal);
                    }
                }

                foreach (var animal in animals)
                {
                    if(animal.ClientId == selectedClientId)
                    {
                        animalsClient.Add(animal);
                    }
                }

                UpdateListBox();
            }
            else
            {
                MessageBox.Show("Erro ao carregar animal: " + response.Message);
            }
        }

        private async void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            await LoadAnimals();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //
            //
            // TO DO
            //
            //
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            searchClients.Clear();

            if (ComboBoxSearch.SelectedIndex == 0)
            {
                foreach (var client in clients)
                {
                    if (client.Name.IndexOf(TextBoxSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        searchClients.Add(client);
                    }
                }
                DataGridSearch.ItemsSource = null;
                DataGridSearch.ItemsSource = searchClients;
            }
            else if (ComboBoxSearch.SelectedIndex == 1)
            {
                foreach (var client in clients)
                {
                    if (client.Nif.IndexOf(TextBoxSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        searchClients.Add(client);
                    }
                }
                DataGridSearch.ItemsSource = null;
                DataGridSearch.ItemsSource = searchClients;
            }
            else if (ComboBoxSearch.SelectedIndex == 2)
            {
                foreach (var client in clients)
                {
                    if (client.PhoneNumber.IndexOf(TextBoxSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        searchClients.Add(client);
                    }
                }
                DataGridSearch.ItemsSource = null;
                DataGridSearch.ItemsSource = searchClients;
            }
            else
            {
                MessageBox.Show("Nenhum item de procura selecionado");
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Client selected = DataGridSearch.SelectedItem as Client;

            if (selected == null)
            {
                MessageBox.Show("Nenhum cliente selecionado");
            }
            else
            {
                selectedClientId = selected.Id;
                TextBoxName.Text = selected.Name;
                TextBoxAddress.Text = selected.Address;
                TextBoxNif.Text = selected.Nif;
                TextBoxPhoneNumber.Text = selected.PhoneNumber;
                TextBoxEmail.Text = selected.Email;

                await LoadAnimals();
            }
        }

        private void btnAddAnimal_Click(object sender, RoutedEventArgs e)
        {
            Animal selectedAnimal = (Animal)ListBoxAvailableAnimals.SelectedItem;

            if (selectedAnimal == null)
            {
                MessageBox.Show("Nenhum animal selecionado");
            }
            else
            {
                animalsClient.Add(selectedAnimal);
                animalsWithoutClient.Remove(selectedAnimal);
                UpdateListBox();
            }
        }

        private void btnRemoveAnimal_Click(object sender, RoutedEventArgs e)
        {
            Animal selectedAnimal = (Animal)ListBoxClientAnimals.SelectedItem;

            if (selectedAnimal == null)
            {
                MessageBox.Show("Nenhum animal selecionado");
            }
            else
            {
                animalsWithoutClient.Add(selectedAnimal);
                animalsClient.Remove(selectedAnimal);
                UpdateListBox();
            }
        }

        private void ClearForm()
        {
            selectedClientId = -1;
            TextBoxName.Text = "";
            TextBoxAddress.Text = "";
            TextBoxNif.Text = "";
            TextBoxPhoneNumber.Text = "";
            TextBoxEmail.Text = "";
        }

        private void UpdateListBox()
        {
            ListBoxAvailableAnimals.ItemsSource = null;
            ListBoxClientAnimals.ItemsSource = null;

            ListBoxAvailableAnimals.ItemsSource = animalsWithoutClient;
            ListBoxClientAnimals.ItemsSource = animalsClient;
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Client selected = DataGridSearch.SelectedItem as Client;


            if (selected.Id == null)
            {
                MessageBox.Show("Selecione um cliente para eliminar");
                return;
            }

            var response = await apiService.DeleteAsync(urlBase, "client", selected.Id);

            if (response.IsSuccess)
            {
                MessageBox.Show("Cliente eliminado");
                ClearForm();
                await LoadClients();
                await LoadAnimals();
            }
            else
            {
                MessageBox.Show("Erro: " + response.Message);
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Client client = new Client()
            {
                Name = TextBoxName.Text,
                Address = TextBoxAddress.Text,
                Nif = TextBoxNif.Text,
                PhoneNumber = TextBoxPhoneNumber.Text,
                Email = TextBoxEmail.Text,
            };

            Response response;

            if (selectedClientId == -1)
            {
                response = await apiService.PostAsync(urlBase, "client", client);
            }
            else
            {
                client.Id = selectedClientId.Value;
                response = await apiService.PutAsync(urlBase, "client", client, selectedClientId.Value);
            }

            if (response.IsSuccess)
            {
                foreach (var animal in animalsWithoutClient)
                {
                    if (animal.ClientId == client.Id)
                    {
                        animal.ClientId = null;
                        response = await apiService.PutAsync(urlBase, "animal", animal, animal.Id);
                    }
                }

                foreach (var animal in animalsClient)
                {
                    animal.ClientId = client.Id;
                    response = await apiService.PutAsync(urlBase, "animal", animal, animal.Id);
                }

                MessageBox.Show("Cliente guardado com sucesso");
                ClearForm();
                await LoadClients();
                await LoadAnimals();
            }
            else
            {
                MessageBox.Show("Erro: " + response.Message);
            }
        }
    }
}