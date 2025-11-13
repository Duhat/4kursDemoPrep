using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace _4KursOsen1
{
    public partial class MainWindow : Window
    {
        private Entities context;
        private List<Event> allEvents;

        public MainWindow()
        {
            InitializeComponent();
            context = new Entities();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Загрузка всех мероприятий
                allEvents = context.Event.ToList();

                // Загрузка направлений для фильтра
                var sections = context.Section.ToList();
                sectionComboBox.ItemsSource = sections;
                sectionComboBox.SelectedIndex = -1; // Ничего не выбрано по умолчанию

                // Установка дат по умолчанию
                dateFromPicker.SelectedDate = DateTime.Today;
                dateToPicker.SelectedDate = DateTime.Today.AddMonths(3);

                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var filteredEvents = allEvents.AsQueryable();

                // Фильтр по дате начала
                if (dateFromPicker.SelectedDate.HasValue)
                {
                    filteredEvents = filteredEvents.Where(e => e.Date_of_Start >= dateFromPicker.SelectedDate.Value);
                }

                // Фильтр по дате окончания
                if (dateToPicker.SelectedDate.HasValue)
                {
                    filteredEvents = filteredEvents.Where(e => e.Date_of_Start <= dateToPicker.SelectedDate.Value);
                }

                // Фильтр по направлению
                if (sectionComboBox.SelectedItem is Section selectedSection)
                {
                    filteredEvents = filteredEvents.Where(e => e.Section.Any(s => s.Section_ID == selectedSection.Section_ID));
                }

                // Обновление отображаемых мероприятий
                eventsItemsControl.ItemsSource = filteredEvents.ToList();

                // Обновление текста о количестве найденных мероприятий
                UpdateEventsCount(filteredEvents.Count());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}");
            }
        }

        private void UpdateEventsCount(int count)
        {
            // Можно добавить TextBlock для отображения количества
            Title = $"Мероприятия ({count} найдено)";
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            dateFromPicker.SelectedDate = DateTime.Today;
            dateToPicker.SelectedDate = DateTime.Today.AddMonths(3);
            sectionComboBox.SelectedIndex = -1;
            ApplyFilters();
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            AuthWindow authWindow = new AuthWindow();
            authWindow.Show();
            this.Close();
        }
    }
}