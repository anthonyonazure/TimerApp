using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;

namespace TimeTrackerApp
{
    public partial class TaskPieChart : UserControl
    {
        private DateTime lastUpdateTime = DateTime.MinValue;
        private const int UpdateIntervalSeconds = 5; // Update every 5 seconds

        public TaskPieChart()
        {
            InitializeComponent();
        }

        public void UpdateChart(IEnumerable<TaskItem> tasks)
        {
            if ((DateTime.Now - lastUpdateTime).TotalSeconds < UpdateIntervalSeconds)
            {
                return; // Skip update if not enough time has passed
            }

            var taskGroups = tasks.GroupBy(t => t.Name)
                                  .Select(g => new { Name = g.Key, TotalTime = g.Sum(t => t.ElapsedTime.TotalSeconds) })
                                  .OrderByDescending(g => g.TotalTime)
                                  .Take(5); // Show top 5 tasks

            if (Chart.Series.Count == 0)
            {
                // Initial chart setup
                var series = new SeriesCollection();
                foreach (var task in taskGroups)
                {
                    series.Add(new PieSeries
                    {
                        Title = task.Name,
                        Values = new ChartValues<double> { task.TotalTime },
                        DataLabels = true,
                        LabelPoint = point => $"{point.Y:F1}s ({point.Participation:P1})"
                    });
                }
                Chart.Series = series;
            }
            else
            {
                // Update existing series
                var existingSeries = Chart.Series.Cast<PieSeries>().ToList();
                foreach (var task in taskGroups)
                {
                    var series = existingSeries.FirstOrDefault(s => s.Title == task.Name);
                    if (series != null)
                    {
                        series.Values[0] = task.TotalTime;
                    }
                    else
                    {
                        Chart.Series.Add(new PieSeries
                        {
                            Title = task.Name,
                            Values = new ChartValues<double> { task.TotalTime },
                            DataLabels = true,
                            LabelPoint = point => $"{point.Y:F1}s ({point.Participation:P1})"
                        });
                    }
                }

                // Remove series that are no longer in the top 5
                for (int i = Chart.Series.Count - 1; i >= 0; i--)
                {
                    var series = Chart.Series[i] as PieSeries;
                    if (series != null && !taskGroups.Any(t => t.Name == series.Title))
                    {
                        Chart.Series.RemoveAt(i);
                    }
                }
            }

            lastUpdateTime = DateTime.Now;
        }
    }
}