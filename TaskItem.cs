using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TimeTrackerApp
{
    public class TaskItem : INotifyPropertyChanged
    {
        private string name = "";
        private string category = "";
        private string description = "";
        private TimeSpan elapsedTime;
        private string status = "";
        private DateTime? startTime;

        public string Name
        {
            get => name;
            set { name = value; OnPropertyChanged(); }
        }

        public string Category
        {
            get => category;
            set { category = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => description;
            set { description = value; OnPropertyChanged(); }
        }

        public TimeSpan ElapsedTime
        {
            get => elapsedTime;
            set { elapsedTime = value; OnPropertyChanged(); }
        }

        public string Status
        {
            get => status;
            set { status = value; OnPropertyChanged(); }
        }

        public void Start()
        {
            if (Status != "Running")
            {
                startTime = DateTime.Now;
                Status = "Running";
            }
        }

        public void Pause()
        {
            if (Status == "Running" && startTime.HasValue)
            {
                ElapsedTime += DateTime.Now - startTime.Value;
                startTime = null;
                Status = "Paused";
            }
        }

        public void Stop()
        {
            if (Status == "Running" && startTime.HasValue)
            {
                ElapsedTime += DateTime.Now - startTime.Value;
            }
            startTime = null;
            Status = "Stopped";
        }

        public void UpdateElapsedTime()
        {
            if (Status == "Running" && startTime.HasValue)
            {
                ElapsedTime = ElapsedTime + (DateTime.Now - startTime.Value);
                startTime = DateTime.Now;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}