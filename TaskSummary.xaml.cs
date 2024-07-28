using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace TimeTrackerApp
{
    public partial class TaskSummary : UserControl
    {
        public TaskSummary()
        {
            InitializeComponent();
        }

        public void UpdateSummary(IEnumerable<TaskItem> tasks)
        {
            int totalTasks = tasks.Count();
            TimeSpan totalTime = TimeSpan.FromSeconds(tasks.Sum(t => t.ElapsedTime.TotalSeconds));
            TimeSpan averageTime = totalTasks > 0 ? TimeSpan.FromSeconds(totalTime.TotalSeconds / totalTasks) : TimeSpan.Zero;

            TotalTasksTextBlock.Text = $"Total Tasks: {totalTasks}";
            TotalTimeTextBlock.Text = $"Total Time: {totalTime:hh\\:mm\\:ss}";
            AverageTimeTextBlock.Text = $"Average Time per Task: {averageTime:hh\\:mm\\:ss}";

            var topTasks = tasks.OrderByDescending(t => t.ElapsedTime)
                                .Take(5)
                                .Select(t => $"{t.Name}: {t.ElapsedTime:hh\\:mm\\:ss}")
                                .ToList();

            TopTasksItemsControl.ItemsSource = topTasks;
        }
    }
}