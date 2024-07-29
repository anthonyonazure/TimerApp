using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections.Generic;
using Microsoft.Win32;

namespace TimeTrackerApp
{
    public partial class MainWindow : Window
    {
        private TimeTrackerViewModel viewModel;
        private DispatcherTimer timer;
        private DateTime lastChartUpdateTime = DateTime.MinValue;
        private const int ChartUpdateIntervalSeconds = 5;

        public MainWindow()
        {
            InitializeComponent();
            
            viewModel = new TimeTrackerViewModel();
            DataContext = viewModel;
            TaskListView.ItemsSource = viewModel.Tasks;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            viewModel.LoadData();
            RefreshChart();

            var categories = new List<string> { "Work", "Personal", "Study", "Other" };
            CategoryComboBox.ItemsSource = categories;
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                DefaultExt = "xlsx",
                FileName = "TaskExport.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    viewModel.ExportToExcel(saveFileDialog.FileName);
                    MessageBox.Show("Data exported successfully!", "Export Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting data: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            viewModel.UpdateElapsedTimes();
            
            if ((DateTime.Now - lastChartUpdateTime).TotalSeconds >= ChartUpdateIntervalSeconds)
            {
                RefreshChart();
                lastChartUpdateTime = DateTime.Now;
            }
        }

        private void RefreshChart()
        {
            TaskChart.UpdateChart(viewModel.Tasks);
            TaskSummary.UpdateSummary(viewModel.Tasks);
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TaskNameTextBox.Text))
            {
                viewModel.AddTask(TaskNameTextBox.Text, CategoryComboBox.SelectedItem as string ?? "Uncategorized", "");
                TaskNameTextBox.Clear();
                CategoryComboBox.SelectedIndex = -1;
                RefreshChart();
                lastChartUpdateTime = DateTime.Now;
            }
        }

        private void SaveData_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SaveData();
            MessageBox.Show("Data saved successfully!", "Save Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void StartTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is TaskItem task)
            {
                viewModel.StartTask(task);
            }
        }

        private void PauseTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is TaskItem task)
            {
                viewModel.PauseTask(task);
            }
        }

        private void StopTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is TaskItem task)
            {
                viewModel.StopTask(task);
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is TaskItem task)
            {
                viewModel.DeleteTask(task);
                RefreshChart();
                lastChartUpdateTime = DateTime.Now;
            }
        }
    }
}