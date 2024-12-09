using HospitalApp.ApplicationData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
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
using System.Windows.Shapes;

namespace HospitalApp.PageEmployee
{
    /// <summary>
    /// Логика взаимодействия для PrintTalonWindow.xaml
    /// </summary>
    public partial class PrintTalonWindow : Window
    {
        private string printContent; // Хранит контент для печати

        public PrintTalonWindow(Talon talon)
        {
            InitializeComponent();

            // Получаем информацию о талоне
            var doctor = AppConnect.model.Doctor.FirstOrDefault(d => d.Id == talon.DoctorId);
            var patient = AppConnect.model.PatientCard.FirstOrDefault(p => p.Id == talon.PatientId);
            var schedule = AppConnect.model.Schedule.FirstOrDefault(s => s.Id == talon.ScheduleId);
            var office = AppConnect.model.Office.FirstOrDefault(o => o.Id == talon.OfficeId); // Корректируем доступ к кабинету

            // Заполняем данные на форме
            DoctorNameTextBlock.Text = $"{doctor.FirstName} {doctor.LastName} {doctor.Patronymic}";
            PatientNameTextBlock.Text = $"{patient.LastName} {patient.FirstName} {patient.Patronymic}";
            SnilsTextBlock.Text = patient.Snils;
            AppointmentDateTextBlock.Text = talon.AdmissionDate.ToShortDateString();
            AppointmentTimeTextBlock.Text = schedule.Time.ToString(@"hh\:mm");
            OfficeNumberTextBlock.Text = office.OfficeNumber.ToString();

            // Формируем контент для печати
            printContent = $"Талон на прием\n\n" +
                           $"Врач: {DoctorNameTextBlock.Text}\n" +
                           $"Пациент: {PatientNameTextBlock.Text} (СНИЛС: {SnilsTextBlock.Text})\n" +
                           $"Дата приема: {AppointmentDateTextBlock.Text}\n" +
                           $"Время: {AppointmentTimeTextBlock.Text}\n" +
                           $"Кабинет: {OfficeNumberTextBlock.Text}";
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            var paginator = new PrintDocumentPaginator(printContent);

            // Печать
            printDialog.PrintDocument(paginator, "Печать талона");
        }
    }
}
