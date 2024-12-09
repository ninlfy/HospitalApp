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

namespace HospitalApp.PageAdmin
{
    /// <summary>
    /// Логика взаимодействия для PageAddOffice.xaml
    /// </summary>
    public partial class PageAddOffice : Page
    {
        public PageAddOffice()
        {
            InitializeComponent();
            LoadDoctors();
        }

        private void LoadDoctors()
        {
            var doctors = AppConnect.model.Doctor.ToList();
            DoctorComboBox.ItemsSource = doctors;
        }

        private void AddOfficeBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DoctorComboBox.SelectedValue == null || string.IsNullOrWhiteSpace(OfficeNumberTxtb.Text))
                {
                    MessageBox.Show("Пожалуйста, выберите врача и введите номер кабинета!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int selectedDoctorId = (int)DoctorComboBox.SelectedValue;

                Office newOffice = new Office()
                {
                    DoctorId = selectedDoctorId, 
                    OfficeNumber = int.Parse(OfficeNumberTxtb.Text)
                };
                AppConnect.model.Office.Add(newOffice);
                AppConnect.model.SaveChanges(); 

                MessageBox.Show("Кабинет успешно добавлен!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                DoctorComboBox.SelectedIndex = -1;
                OfficeNumberTxtb.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении кабинета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
