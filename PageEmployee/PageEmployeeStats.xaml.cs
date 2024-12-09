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
    /// Логика взаимодействия для PageEmployeeStats.xaml
    /// </summary>
    public partial class PageEmployeeStats : Page
    {
        public PageEmployeeStats()
        {
            InitializeComponent();
        }

        private void ChronicalStatsBtn_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.FrameMain.Navigate(new EmployeeStats.ChronicalStatistics());
        }

        private void DoctorStatsBtn_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.FrameMain.Navigate(new EmployeeStats.WorkLoadStatistics());
        }

        private void IssuesStatsBtn_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.FrameMain.Navigate(new EmployeeStats.IncidenceStatistics());
        }
    }
}
