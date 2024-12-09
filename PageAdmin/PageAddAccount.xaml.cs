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
    /// Логика взаимодействия для PageAddAccount.xaml
    /// </summary>
    public partial class PageAddAccount : Page
    {
        public PageAddAccount()
        {
            InitializeComponent();
            RoleComboBox.ItemsSource = AppConnect.model.Role.ToList();
            RoleComboBox.DisplayMemberPath = "RoleName"; 
            RoleComboBox.SelectedValuePath = "Id";       
        }

        private void CreateAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AppConnect.model.User.Any(x => x.Username == LoginTxtb.Text))
            {
                MessageBox.Show("Пользователь с таким логином уже существует!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (AppConnect.model.User.Any(x => x.FirstName == FirstNameTxtb.Text && x.Name == NameTxtb.Text && x.Patronymic == PatronymicTxtb.Text))
            {
                MessageBox.Show("Пользователь с таким ФИО уже существует!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                User userobj = new User()
                {
                    FirstName = FirstNameTxtb.Text,
                    Name = NameTxtb.Text,
                    Patronymic = PatronymicTxtb.Text,
                    Username = LoginTxtb.Text,
                    Password = PaswordTxtb.Password,
                    RoleId = (int)RoleComboBox.SelectedValue 
                };

                AppConnect.model.User.Add(userobj);
                AppConnect.model.SaveChanges();
                MessageBox.Show("Данные успешно добавлены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Ошибка при добавлении данных!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
