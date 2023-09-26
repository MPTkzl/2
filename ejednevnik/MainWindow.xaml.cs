using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using System.IO;
using System.Collections.ObjectModel;

namespace ejednevnik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {
        private NoteManager noteManager = new NoteManager();

        public ObservableCollection<Note> Notes { get; set; } = new ObservableCollection<Note>();
        public DateTime SelectedDate { get; set; } = DateTime.Today;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void LoadNotes_Click(object sender, RoutedEventArgs e)
        {
            noteManager.LoadNotes();
            Notes.Clear();
            foreach (var note in noteManager.GetNotes(SelectedDate))
            {
                Notes.Add(note);
            }
        }

        private void AddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(titleTextBox.Text))
            {
                Note note = new Note
                {
                    Title = titleTextBox.Text,
                    Description = descriptionTextBox.Text,
                    Date = SelectedDate
                };
                noteManager.AddOrUpdateNote(note);
                Notes.Add(note);
                ClearTextBoxes();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (notesListView.SelectedItem is Note selectedNote)
            {
                noteManager.DeleteNote(selectedNote);
                Notes.Remove(selectedNote);
            }
        }

        private void ClearTextBoxes()
        {
            titleTextBox.Clear();
            descriptionTextBox.Clear();
        }
    }

    public class Note
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
        }

        

        public class NoteManager
        {
            private List<Note> notes = new List<Note>();
            private string dataFilePath = "notes.json";

            public List<Note> GetNotes(DateTime date)
            {
                return notes.FindAll(note => note.Date.Date == date.Date);
            }

            public void AddOrUpdateNote(Note note)
            {
                notes.RemoveAll(n => n.Title == note.Title && n.Date.Date == note.Date.Date);
                notes.Add(note);
                SaveNotes();
            }

            public void DeleteNote(Note note)
            {
                notes.Remove(note);
                SaveNotes();
            }

            public void LoadNotes()
            {
                if (File.Exists(dataFilePath))
                {
                    string json = File.ReadAllText(dataFilePath);
                    notes = JsonSerializer.Deserialize<List<Note>>(json);
                }
            }

            private void SaveNotes()
            {
                string json = JsonSerializer.Serialize(notes);
                File.WriteAllText(dataFilePath, json);
            }
        }
        


        public class Serializer
        {
            public static void Serialize<T>(T data, string filePath)
            {
                string json = JsonSerializer.Serialize(data);
                File.WriteAllText(filePath, json);
            }

            public static T Deserialize<T>(string filePath)
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonSerializer.Deserialize<T>(json);
                }
                else
                {
                    return default(T);
                }
            }
        }
        


}

