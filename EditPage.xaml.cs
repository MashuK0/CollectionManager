using System.Collections.ObjectModel;
using System.Text.Json;

namespace CollectionManager;

public partial class EditPage : ContentPage
{
    private ObservableCollection<CollectionElement> elements;
    private string collectionName;
    private string folderPath;

    public EditPage(string collectionName)
    {
        InitializeComponent();
        this.collectionName = collectionName;
        elements = new ObservableCollection<CollectionElement>();
        ElementCollectionView.ItemsSource = elements;
        folderPath = Path.Combine(AppContext.BaseDirectory, "Kolekcje");
        LoadElements();
        TitleLabel.Text = collectionName;
    }


    private void AddElement_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(NewElementEntry.Text))
        {
            var existingElement = elements.FirstOrDefault(el => el.Name.Equals(NewElementEntry.Text, StringComparison.OrdinalIgnoreCase));
            if (existingElement != null)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = await DisplayAlert("Powtarzaj�cy si� element", "Wygl�da na to, �e ten element ju� istnieje w kolekcji. Czy na pewno chcesz go doda�?", "Tak", "Nie");
                    if (result)
                    {
                        elements.Add(new CollectionElement
                        {
                            Name = NewElementEntry.Text,
                            Price = PriceEntry.Text,
                            Status = StatusPicker.SelectedItem?.ToString(),
                            Rate = (int)RatingSlider.Value,
                            Comment = CommentEditor.Text
                        });
                        SaveElements();
                        ClearFields();
                    }
                });
            }
            else
            {
                elements.Add(new CollectionElement
                {
                    Name = NewElementEntry.Text,
                    Price = PriceEntry.Text,
                    Status = StatusPicker.SelectedItem?.ToString(),
                    Rate = (int)RatingSlider.Value,
                    Comment = CommentEditor.Text
                });
                SaveElements();
                ClearFields();
            }
        }
    }

    private async void EditElement_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var element = (CollectionElement)button.CommandParameter;

        var action = await DisplayActionSheet($"Edycja elementu '{element.Name}'", "Anuluj", null, "Edytuj nazw�", "Edytuj cen�", "Edytuj status", "Edytuj ocen�", "Edytuj komentarz");
        switch (action)
        {
            case "Edytuj nazw�":
                var newName = await DisplayPromptAsync("Edytuj nazw�", "Wprowad� now� nazw�:", initialValue: element.Name);
                if (!string.IsNullOrEmpty(newName))
                {
                    element.Name = newName;
                    SaveElements();
                    RefreshPage();
                }
                break;
            case "Edytuj cen�":
                var newPrice = await DisplayPromptAsync("Edytuj cen�", "Wprowad� now� cen�:", initialValue: element.Price.ToString());
                if (!string.IsNullOrEmpty(newPrice))
                {
                    element.Price = newPrice;
                    SaveElements();
                    RefreshPage();
                }
                break;
            case "Edytuj status":
                var newStatus = await DisplayActionSheet("Edytuj status", "Anuluj", null, "Nowy", "U�ywany", "Na sprzeda�", "Sprzedany", "Chc� kupi�");
                if (!string.IsNullOrEmpty(newStatus))
                {
                    element.Status = newStatus;
                    SaveElements();
                    RefreshPage();
                }
                break;
            case "Edytuj ocen�":
                var newRating = await DisplayPromptAsync("Edytuj ocen�", "Wprowad� now� ocen� (skala 1-10):", initialValue: element.Rate.ToString());
                if (int.TryParse(newRating, out int rating) && rating >= 1 && rating <= 10)
                {
                    element.Rate = rating;
                    SaveElements();
                    RefreshPage();
                }
                else
                {
                    await DisplayAlert("B��d", "Prosz� poda� poprawn� ocen� (skala 1-10).", "OK");
                }
                break;
            case "Edytuj komentarz":
                var newComment = await DisplayPromptAsync("Edytuj komentarz", "Wprowad� nowy komentarz:", initialValue: element.Comment);
                if (newComment != null)
                {
                    element.Comment = newComment;
                    SaveElements();
                    RefreshPage();
                }
                break;
        }
    }

    private async void RemoveElement_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var element = (CollectionElement)button.CommandParameter;
        var result = await DisplayAlert("Potwierdzenie", $"Czy na pewno chcesz usun�� element '{element.Name}'?", "Tak", "Anuluj");
        if (result)
        {
            elements.Remove(element);
            SaveElements();
        }
    }

    private void RefreshPage()
    {
        ElementCollectionView.ItemsSource = null;
        ElementCollectionView.ItemsSource = elements;
    }

    private void LoadElements()
    {
        var filePath = Path.Combine(folderPath, $"{collectionName}.txt");
        if (File.Exists(filePath))
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 5)
                {
                    elements.Add(new CollectionElement
                    {
                        Name = parts[0],
                        Price = parts[1],
                        Status = parts[2],
                        Rate = int.Parse(parts[3]),
                        Comment = parts[4]
                    });
                }
            }
        }
    }

    private void SaveElements()
    {
        var filePath = Path.Combine(folderPath, $"{collectionName}.txt");
        using (var writer = new StreamWriter(filePath))
        {
            foreach (var element in elements)
            {
                writer.WriteLine($"{element.Name};{element.Price};{element.Status};{element.Rate};{element.Comment}");
            }
        }
    }

    private void ClearFields()
    {
        NewElementEntry.Text = string.Empty;
        PriceEntry.Text = string.Empty;
        StatusPicker.SelectedIndex = -1;
        RatingSlider.Value = 5;
        CommentEditor.Text = string.Empty;
    }

}

public class CollectionElement
{
    public string Name { get; set; }
    public string Price { get; set; }
    public string Status { get; set; }
    public int Rate { get; set; }
    public string Comment { get; set; }
}