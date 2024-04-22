using System;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Maui.Controls;

namespace CollectionManager
{
    public partial class CollectionPage : ContentPage
    {
        private ObservableCollection<CollectionElement> elements;
        private string collectionName;
        private string folderPath;

        public CollectionPage(string collectionName)
        {
            InitializeComponent();
            this.collectionName = collectionName;
            elements = new ObservableCollection<CollectionElement>();
            ElementCollectionView.ItemsSource = elements;
            folderPath = Path.Combine(AppContext.BaseDirectory, "Kolekcje");
            LoadElements();
            TitleLabel.Text = collectionName;
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
    }
}
