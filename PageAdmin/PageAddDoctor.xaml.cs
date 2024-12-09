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
    /// Логика взаимодействия для PageAddDoctor.xaml
    /// </summary>
    public partial class PageAddDoctor : Page
    {
        public PageAddDoctor()
        {
            InitializeComponent();
            LoadSpecialities();
        }

        private void LoadSpecialities()
        {
            var specialities = AppConnect.model.Specialy.ToList();
            SpecialyComboBox.ItemsSource = specialities;
        }

        private void AddDoctorBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AppConnect.model.Doctor.Any(x => x.FirstName == FirstNameTxtb.Text &&
                                         x.LastName == LastNameTxtb.Text &&
                                         x.Patronymic == PatronymicTxtb.Text))
            {
                MessageBox.Show("Врач с таким ФИО уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                int selectedSpecialtyId = (int)SpecialyComboBox.SelectedValue;

                Doctor doctor = new Doctor()
                {
                    FirstName = FirstNameTxtb.Text,
                    LastName = LastNameTxtb.Text,
                    Patronymic = PatronymicTxtb.Text,
                    SpecialyId = selectedSpecialtyId
                };

                AppConnect.model.Doctor.Add(doctor);
                AppConnect.model.SaveChanges();

                MessageBox.Show("Врач успешно добавлен!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении врача: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
