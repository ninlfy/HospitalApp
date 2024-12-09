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
    /// Логика взаимодействия для PageAdminMain.xaml
    /// </summary>
    public partial class PageAdminMain : Page
    {
        public PageAdminMain()
        {
            InitializeComponent();
        }

        private void AddAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.FrameMain.Navigate(new PageAdmin.PageAddAccount());
        }

        private void AddDoctorBtn_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.FrameMain.Navigate(new PageAdmin.PageAddDoctor());
        }

        private void AddScheduleBtn_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.FrameMain.Navigate(new PageAdmin.PageAddSchedule());
        }

        private void EmployeeList_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.FrameMain.Navigate(new PageAdmin.PageEmployeeList());
        }

        private void AddOfficeBtn_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.FrameMain.Navigate(new PageAdmin.PageAddOffice());
        }
    }
}
