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
    /// Логика взаимодействия для PageEmployeeMain.xaml
    /// </summary>
    public partial class PageEmployeeMain : Page
    {
        public PageEmployeeMain()
        {
            InitializeComponent();
        }

        private void AdmissionBtn_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.FrameMain.Navigate(new PageEmployee.AddAppointment());
        }

        private void PatientRegistrationBtn_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.FrameMain.Navigate(new PageEmployee.PageEmployeeAddPatient());
        }

        private void StatsBtn_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.FrameMain.Navigate(new PageEmployee.PageEmployeeStats());
        }

        private void PatientListBtn_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.FrameMain.Navigate(new PageEmployee.PatientList());
        }
    }
}
