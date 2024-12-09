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
    /// Логика взаимодействия для PatientList.xaml
    /// </summary>
    public partial class PatientList : Page
    {
        public PatientList()
        {
            InitializeComponent();
            LoadPatientData();
        }

        private void LoadPatientData()
        {
            try
            {
                PatientListBox.ItemsSource = AppConnect.model.PatientCard.ToList();
            } catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных пациентов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static class SelectedPatient
        {
            public static PatientCard selectedPatient = null;
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedPatient.selectedPatient = PatientListBox.SelectedItem as PatientCard;
                if (SelectedPatient.selectedPatient != null)
                {
                    AppFrame.FrameMain.Navigate(new PageEmployee.EditPatientCard());
                }
                else
                {
                    MessageBox.Show("Выберите карту пациента для редактирования!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при попытке редактирования данных! \n {ex.Message}", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PatientListBox.SelectedItem != null)
                {
                    PatientCard selectedPatientCard = PatientListBox.SelectedItem as PatientCard;
                    using (var context = new HospitalDBEntities())
                    {
                        var existingService = context.PatientCard.Find(selectedPatientCard.Id);
                        if (existingService != null)
                        {
                            context.PatientCard.Remove(existingService);
                            context.SaveChanges();

                            PatientListBox.ItemsSource = context.PatientCard.ToArray();
                        }
                        else
                        {
                            MessageBox.Show("Что-то пошло не так.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    MessageBox.Show("Карта пациента успешно удалена!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Выберите строку для удаления!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении данных! \n {ex.Message}", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.FrameMain.Navigate(new PageEmployee.PageEmployeeAddPatient());
        }

        private void SearchBySnilsTxtb_TextChanged(object sender, TextChangedEventArgs e)
        {
            PatientListBox.ItemsSource = AppConnect.model.PatientCard.Where(p => p.Snils.Contains(SearchBySnilsTxtb.Text)).ToArray();
        }

        private void SearchByFioTxtb_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = SearchByFioTxtb.Text.ToLower();

            PatientListBox.ItemsSource = AppConnect.model.PatientCard
                .Where(p => p.LastName.ToLower().Contains(searchText) ||
                            p.FirstName.ToLower().Contains(searchText) ||
                            p.Patronymic.ToLower().Contains(searchText))
                .ToArray();
        }
    }
}
