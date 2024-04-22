namespace CollectionManager;

public partial class NewCollectionPage : ContentPage
{
    public NewCollectionPage()
    {
        InitializeComponent();
    }

    private async void AddCollectionButton_Clicked(object sender, EventArgs e)
    {
        var collectionName = CollectionNameEntry.Text?.Trim();
        if (!string.IsNullOrEmpty(collectionName))
        {
            var folderPath = Path.Combine(AppContext.BaseDirectory, "Kolekcje");
            var filePath = Path.Combine(folderPath, $"{collectionName}.txt");

            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                using (File.Create(filePath)) { }
                await DisplayAlert("Sukces", $"Kolekcja '{collectionName}' zosta�a utworzona.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("B��d", "Kolekcja o podanej nazwie ju� istnieje.", "OK");
            }
        }
        else
        {
            await DisplayAlert("B��d", "Prosz� poda� nazw� kolekcji.", "OK");
        }
    }
}