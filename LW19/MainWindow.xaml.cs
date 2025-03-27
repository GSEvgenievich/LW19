using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Windows;

namespace LW19
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Entry>? Entries { get; set; } = new ObservableCollection<Entry>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            EntriesListView.DataContext = this;
        }

        private void CreateArchiveFromFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "all files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                SaveFileDialog saveFileDialog = new();
                saveFileDialog.Filter = "zip files (*.zip)|*.zip";
                if (saveFileDialog.ShowDialog() == true)
                {
                    using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
                        arch.CreateEntryFromFile(openFileDialog.FileName, Path.GetFileName(openFileDialog.FileName));
                    GetEntriesFromZip(saveFileDialog.FileName);
                    MessageBox.Show("Файл успешно архивирован");
                }
            }
        }

        private void CreateArchiveFromDirectory_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new();
            if (openFolderDialog.ShowDialog() == true)
            {
                SaveFileDialog saveFileDialog = new();
                saveFileDialog.Filter = "zip files (*.zip)|*.zip";
                if (saveFileDialog.ShowDialog() == true)
                {
                    ZipFile.CreateFromDirectory(openFolderDialog.FolderName, saveFileDialog.FileName);
                    GetEntriesFromZip(saveFileDialog.FileName);
                    MessageBox.Show("Папка успешно архивирована");
                }
            }
        }

        private void GetEntriesFromZip(string archPath)
        {
            Entries.Clear();

            using ZipArchive zip = ZipFile.Open(archPath, ZipArchiveMode.Update);
            foreach (var item in zip.Entries)
            {
                var size = item.CompressedLength;
                var entry = new Entry()
                {
                    Name = item.Name,
                    ZipSize = size < (1 << 10) ? size + " B" : size < (1 << 20) ? (size >> 10) + " KB" : size < (1 << 30) ? (size >> 20) + " MB" : (size >> 30) + " GB"
                };
                if (!String.IsNullOrEmpty(entry.Name))
                    Entries.Add(entry);
            }
        }

        private void OpenArch_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "zip files (*.zip)|*.zip";
            if (openFileDialog.ShowDialog() == true)
            {
                GetEntriesFromZip(openFileDialog.FileName);
            }
        }

        private void AddInArch_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "all files (*.*)|*.*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                SaveFileDialog saveFileDialog = new();
                saveFileDialog.Filter = "zip files (*.zip)|*.zip";
                if (saveFileDialog.ShowDialog() == true)
                {

                    MessageBox.Show("d");
                    using (ZipArchive zip = ZipFile.Open(openFileDialog.FileName, ZipArchiveMode.Update))
                    {
                        foreach(var item in saveFileDialog.FileNames)
                            zip.CreateEntryFromFile(item, Path.GetFileName(item));
                    }
                    GetEntriesFromZip(openFileDialog.FileName);
                    MessageBox.Show("Файл(-ы) успешно архивирован");
                }
            }
        }
    }
}