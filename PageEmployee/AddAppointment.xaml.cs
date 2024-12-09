using HospitalApp.ApplicationData;
using System;
using System.Collections.Generic;
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

namespace HospitalApp.PageEmployee
{
    /// <summary>
    /// Логика взаимодействия для AddAppointment.xaml
    /// </summary>
    public partial class AddAppointment : Page
    {

        private Doctor selectedDoctor;   // Выбранный врач
        private PatientCard patient;     // Пациент
        private Talon createdTalon;      // Созданный талон

        public AddAppointment()
        {
            InitializeComponent();
            // Заполнение ComboBox для специализаций врачей
            SpecialtyComboBox.ItemsSource = AppConnect.model.Specialy.Select(s => s.SpecialyName).ToList();

            // Подписываемся на события изменения
            SpecialtyComboBox.SelectionChanged += SpecialtyComboBox_SelectionChanged;
            AppointmentDatePicker.SelectedDateChanged += AppointmentDatePicker_SelectedDateChanged;
        }


        private void PrintTalonBtn_Click(object sender, RoutedEventArgs e)
        {
            if (createdTalon == null)
            {
                MessageBox.Show("Талон еще не создан!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Открываем окно печати талона
            PrintTalonWindow printWindow = new PrintTalonWindow(createdTalon);
            printWindow.ShowDialog();
        }

        private void AppointmentDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAvailableTimes();
        }

        private void SpecialtyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAvailableTimes();
        }

        // Метод для обновления доступных временных слотов
        private void UpdateAvailableTimes()
        {
            // Проверяем, что выбраны и специализация, и дата
            if (SpecialtyComboBox.SelectedItem == null || AppointmentDatePicker.SelectedDate == null)
            {
                TimeComboBox.ItemsSource = null;
                return;
            }

            string selectedSpecialty = SpecialtyComboBox.SelectedItem.ToString();
            DateTime selectedDate = AppointmentDatePicker.SelectedDate.Value;

            // Находим врача по специализации
            selectedDoctor = AppConnect.model.Doctor.FirstOrDefault(d => d.Specialy.SpecialyName == selectedSpecialty);

            if (selectedDoctor == null)
            {
                MessageBox.Show("Врач с выбранной специализацией не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Получаем все временные слоты
            var allTimes = AppConnect.model.Schedule.Select(s => s.Time).ToList();

            // Получаем список занятых временных слотов
            var takenTimes = AppConnect.model.Talon
                .Where(t => t.DoctorId == selectedDoctor.Id && t.AdmissionDate == selectedDate)
                .Select(t => t.Schedule.Time)
                .ToList();

            // Очищаем ComboBox времени
            TimeComboBox.Items.Clear();

            // Добавляем временные слоты в ComboBox
            foreach (var time in allTimes)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = time.ToString(@"hh\:mm");

                if (takenTimes.Contains(time))
                {
                    item.IsEnabled = false;
                    item.Foreground = Brushes.Gray; // Серый цвет для недоступного времени
                }

                TimeComboBox.Items.Add(item);
            }
        }

        private void SubmitAppointmentBtn_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что все данные заполнены
            if (SpecialtyComboBox.SelectedItem == null || AppointmentDatePicker.SelectedDate == null || TimeComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(SnilsTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Получаем СНИЛС клиента
            string snils = SnilsTextBox.Text.Trim();

            // Ищем пациента по СНИЛС
            patient = AppConnect.model.PatientCard.FirstOrDefault(p => p.Snils == snils);

            if (patient == null)
            {
                MessageBox.Show("Пациент с указанным СНИЛС не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Получаем выбранное время
            ComboBoxItem selectedTimeItem = TimeComboBox.SelectedItem as ComboBoxItem;
            if (selectedTimeItem == null || !selectedTimeItem.IsEnabled)
            {
                MessageBox.Show("Выберите доступное время!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            TimeSpan selectedTime = TimeSpan.Parse(selectedTimeItem.Content.ToString());

            DateTime selectedDate = AppointmentDatePicker.SelectedDate.Value;

            // Проверяем, что время не занято (дополнительная проверка)
            bool isTimeTaken = AppConnect.model.Talon.Any(t =>
                t.DoctorId == selectedDoctor.Id &&
                t.AdmissionDate == selectedDate &&
                t.Schedule.Time == selectedTime);

            if (isTimeTaken)
            {
                MessageBox.Show("Выбранное время уже занято!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateAvailableTimes(); // Обновляем список доступных времен
                return;
            }

            try
            {
                // Создаем новый талон
                createdTalon = new Talon()
                {
                    DoctorId = selectedDoctor.Id,
                    PatientId = patient.Id,
                    AdmissionDate = selectedDate,
                    ScheduleId = AppConnect.model.Schedule.First(s => s.Time == selectedTime).Id,
                    OfficeId = (int)(AppConnect.model.Office.FirstOrDefault(o => o.DoctorId == selectedDoctor.Id)?.Id)
                };

                AppConnect.model.Talon.Add(createdTalon);
                AppConnect.model.SaveChanges();

                MessageBox.Show("Запись на прием успешно создана!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении талона: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
