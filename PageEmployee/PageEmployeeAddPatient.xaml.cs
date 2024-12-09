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
    /// Логика взаимодействия для PageEmployeeAddPatient.xaml
    /// </summary>
    public partial class PageEmployeeAddPatient : Page
    {
        public PageEmployeeAddPatient()
        {
            InitializeComponent();
            LoadChronical();
        }

        private void LoadChronical()
        {
            var chronical = AppConnect.model.Chronical.ToList();
            ChronicalCmb.ItemsSource = chronical;
        }

        private void RegPatientBtn_Click(object sender, RoutedEventArgs e)
        {
            string snils = SNILSTxtb.Text;
            
            if (AppConnect.model.PatientCard.Any(x => x.FirstName == FirstNameTxtb.Text &&
                                                      x.LastName == LastNameTxtb.Text &&
                                                      x.Patronymic == PatronymicTxtb.Text))
            {
                MessageBox.Show("Пациент с такими ФИО уже зарегистрирован!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                //int chronicalId = ChronicalIssuesCheckBox.IsChecked == true ? 1 : 2;
                int dispensaryId = DispensaryCheckBox.IsChecked == true ? 1 : 2;

                if (birthDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату рождения пациента!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                PatientCard pcobj = new PatientCard()
                {
                    FirstName = FirstNameTxtb.Text,
                    LastName = LastNameTxtb.Text,
                    Patronymic = PatronymicTxtb.Text,
                    DateBirth = birthDatePicker.SelectedDate.Value, 
                    Snils = snils,
                    ChronicalId = (int)ChronicalCmb.SelectedValue, 
                    DispensaryId = dispensaryId 
                };

                AppConnect.model.PatientCard.Add(pcobj);
                AppConnect.model.SaveChanges();

                MessageBox.Show("Пациент успешно зарегистрирован!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации пациента: {ex.ToString()}", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
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
