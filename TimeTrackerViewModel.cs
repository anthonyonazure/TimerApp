using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using OfficeOpenXml; 
using System.Linq; 

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

        public void ExportToExcel(string filePath)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Tasks");

                // Add headers
                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Category";
                worksheet.Cells[1, 3].Value = "Description";
                worksheet.Cells[1, 4].Value = "Elapsed Time";
                worksheet.Cells[1, 5].Value = "Status";

                // Add data
                for (int i = 0; i < Tasks.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = Tasks[i].Name;
                    worksheet.Cells[i + 2, 2].Value = Tasks[i].Category;
                    worksheet.Cells[i + 2, 3].Value = Tasks[i].Description;
                    worksheet.Cells[i + 2, 4].Value = Tasks[i].ElapsedTime.ToString();
                    worksheet.Cells[i + 2, 5].Value = Tasks[i].Status;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Save the file
                File.WriteAllBytes(filePath, package.GetAsByteArray());
            }
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