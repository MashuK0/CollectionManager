using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Maui.Controls;

namespace CollectionManager
{
    public partial class MainPage : ContentPage
    {
        private List<string> collections;

        public MainPage()
        {
            InitializeComponent();
            collections = LoadCollections();
            RefreshCollections();
        }

        private List<string> LoadCollections()
        {
            var collectionsDirectory = Path.Combine(AppContext.BaseDirectory, "Kolekcje");
            if (!Directory.Exists(collectionsDirectory))
            {
                Directory.CreateDirectory(collectionsDirectory);
            }
            var collectionFiles = Directory.GetFiles(collectionsDirectory, "*.txt");
            var collectionNames = collectionFiles.Select(Path.GetFileNameWithoutExtension).ToList();
            return collectionNames;
        }

        private void RefreshCollections()
        {
            CollectionsListView.ItemsSource = null;
            CollectionsListView.ItemsSource = collections;
        }

        private async void AddCollectionButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewCollectionPage());
        }

        private async void CollectionsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var collectionName = e.Item.ToString();

            var action = await DisplayActionSheet($"Opcje dla kolekcji '{collectionName}'", "Anuluj", null, "Wyświetl kolekcję", "Edytuj kolekcję");
            switch (action)
            {
                case "Wyświetl kolekcję":
                    await Navigation.PushAsync(new CollectionPage(collectionName));
                    break;
                case "Edytuj kolekcję":
                    await Navigation.PushAsync(new EditPage(collectionName));
                    break;
            }

            collections = LoadCollections();
            RefreshCollections();
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var collectionName = button.CommandParameter.ToString();

            var result = await DisplayAlert("Potwierdzenie", $"Czy na pewno chcesz usunąć kolekcję '{collectionName}'?", "Tak", "Anuluj");
            if (result)
            {
                var collectionsDirectory = Path.Combine(AppContext.BaseDirectory, "Kolekcje");
                var collectionFilePath = Path.Combine(collectionsDirectory, $"{collectionName}.txt");
                if (File.Exists(collectionFilePath))
                {
                    File.Delete(collectionFilePath);
                }
                collections.Remove(collectionName);
                RefreshCollections();
            }
        }

        private async void GenerateSummaryButton_Clicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var collectionName = button.CommandParameter.ToString();
            var summary = GenerateSummary(collectionName);

            await DisplayAlert("Podsumowanie", summary, "OK");
        }

        private string GenerateSummary(string collectionName)
        {
            var folderPath = Path.Combine(AppContext.BaseDirectory, "Kolekcje");
            var filePath = Path.Combine(folderPath, $"{collectionName}.txt");

            if (!File.Exists(filePath))
            {
                return "Nie można znaleźć pliku kolekcji.";
            }

            var lines = File.ReadAllLines(filePath);

            int totalItems = lines.Length;

            int soldItems = 0;
            int itemsToSell = 0;

            foreach (var line in lines)
            {
                var elements = line.Split(';');
                if (elements.Length >= 4)
                {
                    var status = elements[2].Trim();
                    if (status.Equals("Sprzedany", StringComparison.OrdinalIgnoreCase))
                    {
                        soldItems++;
                    }
                    else if (status.Equals("Na Sprzedaż", StringComparison.OrdinalIgnoreCase))
                    {
                        itemsToSell++;
                    }
                }
            }

            var summary = $"Podsumowanie kolekcji '{collectionName}':\n" +
                          $"- Ilość posiadanych przedmiotów: {totalItems}\n" +
                          $"- Ilość przedmiotów sprzedanych: {soldItems}\n" +
                          $"- Ilość przedmiotów do sprzedania: {itemsToSell}";

            return summary;
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            collections = LoadCollections();
            RefreshCollections();

            Debug.WriteLine("Ścieżka do danych aplikacji: " + FileSystem.AppDataDirectory);
        }
    }
}
