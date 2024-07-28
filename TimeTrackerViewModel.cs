using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace TimeTrackerApp
{
    public class TimeTrackerViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TaskItem> Tasks { get; set; }

        public TimeTrackerViewModel()
        {
            Tasks = new ObservableCollection<TaskItem>();
        }

        public void AddTask(string name, string category, string description)
        {
            Tasks.Add(new TaskItem 
            { 
                Name = name,
                Category = category,
                Description = description,
                Status = "Not Started"
            });
        }

        public void StartTask(TaskItem task)
        {
            if (task != null)
            {
                task.Start();
            }
        }

        public void PauseTask(TaskItem task)
        {
            if (task != null)
            {
                task.Pause();
            }
        }

        public void StopTask(TaskItem task)
        {
            if (task != null)
            {
                task.Stop();
            }
        }

        public void DeleteTask(TaskItem task)
        {
            if (task != null)
            {
                Tasks.Remove(task);
            }
        }

        public void UpdateElapsedTimes()
        {
            foreach (var task in Tasks)
            {
                task.UpdateElapsedTime();
            }
        }

        public void SaveData()
        {
            string json = JsonConvert.SerializeObject(Tasks);
            File.WriteAllText("tasks.json", json);
        }

        public void LoadData()
        {
            if (File.Exists("tasks.json"))
            {
                string json = File.ReadAllText("tasks.json");
                var loadedTasks = JsonConvert.DeserializeObject<ObservableCollection<TaskItem>>(json);
                if (loadedTasks != null)
                {
                    Tasks.Clear();
                    foreach (var task in loadedTasks)
                    {
                        Tasks.Add(task);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}