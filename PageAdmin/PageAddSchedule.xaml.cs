using HospitalApp.ApplicationData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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

namespace HospitalApp.PageAdmin
{
    /// <summary>
    /// Логика взаимодействия для PageAddSchedule.xaml
    /// </summary>
    public partial class PageAddSchedule : Page
    {
        public PageAddSchedule()
        {
            InitializeComponent();
        }

        private void AddScheduleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TimeTextBox.Text))
            {
                MessageBox.Show("Введите время!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!TimeSpan.TryParseExact(TimeTextBox.Text, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan timeValue))
            {
                MessageBox.Show("Введите корректное время в формате HH:mm", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var existingTimes = AppConnect.model.Schedule.Select(x => x.Time).ToList();
            foreach (var existingTime in existingTimes)
            {
                if (Math.Abs((existingTime - timeValue).TotalMinutes) < 30)
                {
                    MessageBox.Show("Разница между введенным временем и существующими записями должна быть не менее 30 минут!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            try
            {
                Schedule entity = new Schedule()
                {
                    Time = timeValue 
                };

                AppConnect.model.Schedule.Add(entity);
                AppConnect.model.SaveChanges();

                MessageBox.Show("Время успешно добавлено!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                TimeTextBox.Clear();
                PlaceholderText.Visibility = Visibility.Visible; 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void TimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTimeInputAllowed(e.Text);
        }

        private bool IsTimeInputAllowed(string text)
        {
            return char.IsDigit(text[0]) || text == ":";
        }

        private void TimeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TimeTextBox.Text))
            {
                PlaceholderText.Visibility = Visibility.Visible; 
            }
            else if (!TimeSpan.TryParseExact(TimeTextBox.Text, "hh\\:mm", CultureInfo.InvariantCulture, out _))
            {
                MessageBox.Show("Введите корректное время в формате HH:mm", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                TimeTextBox.Focus();
            }
        }

        private void TimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TimeTextBox.Text))
            {
                PlaceholderText.Visibility = Visibility.Collapsed; 
            }
        }
    }
}
