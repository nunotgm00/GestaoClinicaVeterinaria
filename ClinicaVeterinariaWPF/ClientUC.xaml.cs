using ClinicaVeterinariaWPF.Models;
using ClinicaVeterinariaWPF.Services;
using Newtonsoft.Json;
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
    public partial class ClientUC : UserControl
    {
        private readonly ApiService apiService = new ApiService();
        private const string urlBase = "http://gestaoclinicaveterinariaapi.somee.com/api";
        private int selectedClientId = -1;
        private List<Client> clients = new List<Client>();
        private List<Client> searchClients = new List<Client>();
        private List<Animal> animals = new List<Animal>();
        private List<Animal> animalsClient = new List<Animal>();
        private List<Animal> animalsWithoutClient = new List<Animal>();

        public ClientUC()
        {
            InitializeComponent();
            this.Loaded += ClientWindow_Loaded;
        }

        public event EventHandler CloseRequested;

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
                animals = (List<Animal>)response.Result;

                animalsWithoutClient.Clear();
                animalsClient.Clear();

                foreach (var animal in animals)
                {
                    if (animal.ClientId == null)
                    {
                        animalsWithoutClient.Add(animal);
                    }

                    if (animal.ClientId == selectedClientId)
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
            ClearTools();
            await LoadAnimals();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            searchClients.Clear();

            if (ComboBoxSearch.SelectedValue == null)
            {
                MessageBox.Show("Nenhum item de procura selecionado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ComboBoxSearch.SelectedIndex == 0)
            {
                foreach (var client in clients)
                {
                    if (client.Name.IndexOf(TextBoxSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        searchClients.Add(client);
                    }
                }
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
            }

            DataGridSearch.ItemsSource = null;
            DataGridSearch.ItemsSource = searchClients;
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Client selected = DataGridSearch.SelectedItem as Client;

            ClearTools();

            if (selected == null)
            {
                MessageBox.Show("Nenhum cliente selecionado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Nenhum animal selecionado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Nenhum animal selecionado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                animalsWithoutClient.Add(selectedAnimal);
                animalsClient.Remove(selectedAnimal);
                UpdateListBox();
            }
        }

        private void UpdateListBox()
        {
            ListBoxAvailableAnimals.ItemsSource = null;
            ListBoxAvailableAnimals.ItemsSource = animalsWithoutClient;
            ListBoxClientAnimals.ItemsSource = null;
            ListBoxClientAnimals.ItemsSource = animalsClient;
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Client selected = DataGridSearch.SelectedItem as Client;

            if (selected == null)
            {
                MessageBox.Show("Selecione um cliente para eliminar", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            foreach (var animal in animals)
            {
                if (animal.ClientId == selected.Id)
                {
                    MessageBox.Show("Este cliente não pode ser eliminado porque tem animais associados", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            DeactivateButtons();

            var response = await apiService.DeleteAsync(urlBase, "client", selected.Id);

            if (response.IsSuccess)
            {
                MessageBox.Show("Cliente eliminado");
                ClearTools();
                await LoadClients();
                await LoadAnimals();
            }
            else
            {
                MessageBox.Show("Erro: " + response.Message);
            }

            ActivateButtons();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Validation())
            {
                DeactivateButtons();

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
                    client.Id = selectedClientId;
                    response = await apiService.PutAsync(urlBase, "client", client, selectedClientId);
                }

                if (response.IsSuccess)
                {
                    if (selectedClientId == -1)
                    {
                        Client createdClient = JsonConvert.DeserializeObject<Client>((string)response.Result);
                        client.Id = createdClient.Id;
                    }

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

                    if (response.IsSuccess)
                    {
                        MessageBox.Show("Cliente guardado com sucesso");
                        ClearTools();
                        await LoadClients();
                        await LoadAnimals();
                    }
                    else
                    {
                        MessageBox.Show("Erro: " + response.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Erro: " + response.Message);
                }

                ActivateButtons();
            }
        }

        private void ClearTools()
        {
            selectedClientId = -1;
            TextBoxName.Text = "";
            TextBoxAddress.Text = "";
            TextBoxNif.Text = "";
            TextBoxPhoneNumber.Text = "";
            TextBoxEmail.Text = "";
            TextBlockAnimalName.Text = "---";
            TextBlockAnimalSex.Text = "---";
            TextBlockAnimalSpecies.Text = "---";

        }

        private bool Validation()
        {
            if (string.IsNullOrEmpty(TextBoxName.Text))
            {
                MessageBox.Show("Insira nome do cliente", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrEmpty(TextBoxAddress.Text))
            {
                MessageBox.Show("Insira a morada do cliente", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (TextBoxNif.Text.Length != 9 || !(TextBoxNif.Text.All(char.IsDigit)))
            {
                MessageBox.Show("Insira um NIF válido", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrEmpty(TextBoxPhoneNumber.Text) || !(TextBoxPhoneNumber.Text.All(char.IsDigit)))
            {
                MessageBox.Show("Insira um número de telefone válido", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrEmpty(TextBoxEmail.Text) || !(TextBoxEmail.Text.Contains("@")))
            {
                MessageBox.Show("Insira um E-Mail válido", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void DeactivateButtons()
        {
            btnSearch.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnNew.IsEnabled = false;
            btnSave.IsEnabled = false;
            btnClose.IsEnabled = false;
            btnAddAnimal.IsEnabled = false;
            btnRemoveAnimal.IsEnabled = false;
        }

        private void ActivateButtons()
        {
            btnSearch.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnNew.IsEnabled = true;
            btnSave.IsEnabled = true;
            btnClose.IsEnabled = true;
            btnAddAnimal.IsEnabled = true;
            btnRemoveAnimal.IsEnabled = true;
        }

        private async void btnAllList_Click(object sender, RoutedEventArgs e)
        {
            await LoadClients();
        }

        private void ListBoxClientAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Animal selectedAnimal = (Animal)ListBoxClientAnimals.SelectedItem;

            if(selectedAnimal != null)
            {
                TextBlockAnimalName.Text = selectedAnimal.Name;
                TextBlockAnimalSex.Text = selectedAnimal.Sex;
                TextBlockAnimalSpecies.Text = selectedAnimal.Species;
            }
        }

        private void ListBoxAvailableAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Animal selectedAnimal = (Animal)ListBoxAvailableAnimals.SelectedItem;

            if(selectedAnimal != null)
            {
                TextBlockAnimalName.Text = selectedAnimal.Name;
                TextBlockAnimalSex.Text = selectedAnimal.Sex;
                TextBlockAnimalSpecies.Text = selectedAnimal.Species;
            }
        }
    }
}

