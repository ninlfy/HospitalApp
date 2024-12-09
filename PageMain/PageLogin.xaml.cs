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

namespace HospitalApp.PageMain
{
    /// <summary>
    /// Логика взаимодействия для PageLogin.xaml
    /// </summary>
    public partial class PageLogin : Page
    {
        public PageLogin()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            HospitalDBEntities model = new HospitalDBEntities();
            try
            {
                var userobj = model.User.FirstOrDefault(x => x.Username == LoginTxtb.Text && x.Password == PasswordPsb.Password);
                if (userobj == null)
                {
                    MessageBox.Show("Такого пользователя нет!", "Ошибка при авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                } else
                {
                    ApplicationData.AccountHelpClass.Id = userobj.Id;
                    switch (userobj.RoleId)
                    {
                        case 1:
                            AppFrame.FrameMain.Navigate(new PageAdmin.PageAdminMain());
                            break;
                        case 2:
                            AppFrame.FrameMain.Navigate(new PageEmployee.PageEmployeeMain());
                            break;
                        default:
                            MessageBox.Show("Данные не обнаружены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;
                    }
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message.ToString(), "Критическая ошибка работы приложения", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
