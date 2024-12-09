using HospitalApp.ApplicationData;
using HospitalApp.Control;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace HospitalApp.EmployeeStats
{
    public partial class WorkLoadStatistics : Page
    {
        public List<string> DoctorNames { get; set; }
        public Func<double, string> Formatter { get; set; }
        private DateTime? selectedDate;
        private bool isMonthly;

        public WorkLoadStatistics()
        {
            InitializeComponent();
            isMonthly = true; // По умолчанию выбор по месяцам
            DateSelector.SelectedDate = DateTime.Now;
            LoadDoctorWorkloadData();
            SetupChart();
        }

        private void LoadDoctorWorkloadData()
        {
            var date = selectedDate ?? DateTime.Now;
            var month = isMonthly ? date.Month : (int?)null;
            var year = date.Year;

            var doctorStats = AppConnect.model.Doctor
                .Join(AppConnect.model.Specialy,
                      doctor => doctor.SpecialyId,
                      specialy => specialy.Id,
                      (doctor, specialy) => new
                      {
                          FullName = doctor.FirstName + " " + doctor.LastName + " " + doctor.Patronymic,
                          SpecialyName = specialy.SpecialyName,
                          MonthlyAppointments = AppConnect.model.Talon.Count(t => t.DoctorId == doctor.Id && (isMonthly ? t.AdmissionDate.Month == month : true) && t.AdmissionDate.Year == year),
                          YearlyAppointments = AppConnect.model.Talon.Count(t => t.DoctorId == doctor.Id && t.AdmissionDate.Year == year)
                      })
                .ToList();

            DoctorWorkloadGrid.ItemsSource = doctorStats;
            DoctorNames = doctorStats.Select(d => d.FullName).ToList();
        }

        private void SetupChart()
        {
            var date = selectedDate ?? DateTime.Now;
            var month = isMonthly ? date.Month : (int?)null;
            var year = date.Year;
            var doctorStats = AppConnect.model.Doctor
                .Join(AppConnect.model.Specialy,
                      doctor => doctor.SpecialyId,
                      specialy => specialy.Id,
                      (doctor, specialy) => new
                      {
                          FullName = doctor.FirstName + " " + doctor.LastName + " " + doctor.Patronymic,
                          MonthlyAppointments = AppConnect.model.Talon.Count(t => t.DoctorId == doctor.Id && (isMonthly ? t.AdmissionDate.Month == month : true) && t.AdmissionDate.Year == year),
                          YearlyAppointments = AppConnect.model.Talon.Count(t => t.DoctorId == doctor.Id && t.AdmissionDate.Year == year)
                      })
                .ToList();

            DoctorWorkloadChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = isMonthly ? $"Талоны за {date:MMMM yyyy}" : $"Талоны за {year} год",
                    Values = new ChartValues<int>(doctorStats.Select(d => isMonthly ? d.MonthlyAppointments : d.YearlyAppointments)),
                    DataLabels = true,
                    LabelPoint = point => point.Y.ToString(),
                    Fill = new SolidColorBrush(Color.FromRgb(0, 120, 215))
                }
            };

            Formatter = value => value.ToString("N0");
            DoctorWorkloadChart.AxisX[0].Labels = DoctorNames;
        }

        private void PeriodSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isMonthly = (PeriodSelector.SelectedIndex == 0); // 0 - Месяц, 1 - Год
            SetupChart();
        }

        private void DateSelector_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedDate = DateSelector.SelectedDate;
            SetupChart();
        }

        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создание FlowDocument для печати
                FlowDocument doc = new FlowDocument
                {
                    PageHeight = 1200,
                    PageWidth = 1600
                };

                // Создание заголовка
                Paragraph header = new Paragraph(new Run("Список хронических больных"));
                header.FontSize = 20;
                header.FontWeight = FontWeights.Bold;
                header.TextAlignment = TextAlignment.Center;
                doc.Blocks.Add(header);

                // Создание таблицы для данных сотрудников
                Table employeeTable = new Table();
                doc.Blocks.Add(employeeTable);
                employeeTable.TextAlignment = TextAlignment.Center;

                // Определение столбцов таблицы
                employeeTable.Columns.Add(new TableColumn() { Width = new GridLength(100) });
                employeeTable.Columns.Add(new TableColumn() { Width = new GridLength(100) });

                // Создание заголовка таблицы
                TableRowGroup tableHeader = new TableRowGroup();
                employeeTable.RowGroups.Add(tableHeader);

                TableRow headerRow = new TableRow();
                tableHeader.Rows.Add(headerRow);

                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Фамилия"))) { FontWeight = FontWeights.Bold });
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Имя"))) { FontWeight = FontWeights.Bold });
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Отчество"))) { FontWeight = FontWeights.Bold });
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Дата рождения"))) { FontWeight = FontWeights.Bold });

                // Заполнение данных из базы
                TableRowGroup dataRows = new TableRowGroup();
                employeeTable.RowGroups.Add(dataRows);

                var employees = AppConnect.model.PatientCard.ToList();
                foreach (var employee in employees)
                {
                    TableRow row = new TableRow();
                    dataRows.Rows.Add(row);

                    row.Cells.Add(new TableCell(new Paragraph(new Run(employee.LastName))));
                    row.Cells.Add(new TableCell(new Paragraph(new Run(employee.FirstName))));
                    row.Cells.Add(new TableCell(new Paragraph(new Run(employee.Patronymic))));
                    string dateBirth = (employee.DateBirth).ToString();
                    row.Cells.Add(new TableCell(new Paragraph(new Run(dateBirth))));
                }

                // Отправка документа на печать
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    IDocumentPaginatorSource paginatorSource = doc;
                    printDialog.PrintDocument(paginatorSource.DocumentPaginator, "Печать списка Хронических больных");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при печати: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
