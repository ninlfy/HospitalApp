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
using static HospitalApp.PageEmployee.PatientList;

namespace HospitalApp.PageEmployee
{
    /// <summary>
    /// Логика взаимодействия для EditPatientCard.xaml
    /// </summary>
    public partial class EditPatientCard : Page
    {
        public EditPatientCard()
        {
            InitializeComponent();
            LastNameTxtb.Text = SelectedPatient.selectedPatient.LastName.ToString();
            FirstNameTxtb.Text = SelectedPatient.selectedPatient.FirstName.ToString();
            PatronymicTxtb.Text = SelectedPatient.selectedPatient.Patronymic.ToString();
            SNILSTxtb.Text = SelectedPatient.selectedPatient.Snils.ToString();
            birthDatePicker.SelectedDate = SelectedPatient.selectedPatient.DateBirth;
            ChronicalCmb.SelectedValue = SelectedPatient.selectedPatient.ChronicalId;
            DispensaryCheckBox.IsChecked = SelectedPatient.selectedPatient.DispensaryId == 1;
            LoadChronical();
        }

        private void LoadChronical()
        {
            var chronical = AppConnect.model.Chronical.ToList();
            ChronicalCmb.ItemsSource = chronical;
        }

        private void RegPatientBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, что выбранный пациент не null
                if (SelectedPatient.selectedPatient == null)
                {
                    MessageBox.Show("Пациент не выбран!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Получаем данные из текстовых полей и чекбоксов и сохраняем в объекте SelectedPatient.selectedPatient
                SelectedPatient.selectedPatient.LastName = LastNameTxtb.Text;
                SelectedPatient.selectedPatient.FirstName = FirstNameTxtb.Text;
                SelectedPatient.selectedPatient.Patronymic = PatronymicTxtb.Text;
                SelectedPatient.selectedPatient.Snils = SNILSTxtb.Text;

                if (birthDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату рождения пациента!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                SelectedPatient.selectedPatient.DateBirth = birthDatePicker.SelectedDate.Value;

                // Обновляем состояние чекбоксов в базе данных
                SelectedPatient.selectedPatient.ChronicalId = (int)ChronicalCmb.SelectedValue;
                SelectedPatient.selectedPatient.DispensaryId = DispensaryCheckBox.IsChecked == true ? 1 : 2;

                // Сохраняем изменения в базе данных
                AppConnect.model.SaveChanges();

                MessageBox.Show("Данные пациента успешно обновлены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных пациента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SNILSTxtb_TextChanged(object sender, TextChangedEventArgs e)
        {
            string rawSnils = new string(SNILSTxtb.Text.Where(char.IsDigit).ToArray());

            if (rawSnils.Length > 11) rawSnils = rawSnils.Substring(0, 11);

            string formattedSnils = rawSnils;
            if (rawSnils.Length >= 3)
            {
                formattedSnils = rawSnils.Insert(3, "-");
            }
            if (rawSnils.Length >= 6)
            {
                formattedSnils = formattedSnils.Insert(7, "-");
            }
            if (rawSnils.Length >= 9)
            {
                formattedSnils = formattedSnils.Insert(11, " ");
            }

            int currentSelectionStart = SNILSTxtb.SelectionStart;

            SNILSTxtb.Text = formattedSnils;

            SNILSTxtb.SelectionStart = currentSelectionStart;
        }

        private void SNILSTxtb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }
    }
}
