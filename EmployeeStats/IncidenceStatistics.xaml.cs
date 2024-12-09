using HospitalApp.ApplicationData;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace HospitalApp.EmployeeStats
{
    public partial class IncidenceStatistics : Page
    {
        public List<string> YearLabels { get; set; }
        public List<string> MonthLabels { get; set; }
        public Func<double, string> Formatter { get; set; }
        private int selectedYear;
        private int? selectedMonth;

        public IncidenceStatistics()
        {
            InitializeComponent();
            LoadYears();
            LoadMonths();
            selectedYear = DateTime.Now.Year; // Установка текущего года по умолчанию
            YearSelector.SelectedItem = selectedYear;
            MonthSelector.SelectedItem = DateTime.Now.Month; // Установка текущего месяца по умолчанию
            LoadSicknessData(selectedYear, selectedMonth);
            SetupChart(selectedYear, selectedMonth);
        }

        private void LoadYears()
        {
            // Загружаем доступные годы из данных
            var years = AppConnect.model.Talon
                .Select(t => t.AdmissionDate.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToList();

            foreach (var year in years)
            {
                YearSelector.Items.Add(year);
            }
        }

        private void LoadMonths()
        {
            // Загружаем доступные месяцы
            MonthLabels = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
                .Where(m => !string.IsNullOrEmpty(m))
                .ToList();

            foreach (var month in MonthLabels)
            {
                MonthSelector.Items.Add(month);
            }
        }

        private void LoadSicknessData(int year, int? month)
        {
            // Группируем талоны по годам и месяцам и считаем количество
            var sicknessStats = AppConnect.model.Talon
                .Where(t => t.AdmissionDate.Year == year && (!month.HasValue || t.AdmissionDate.Month == month))
                .GroupBy(t => new { t.AdmissionDate.Year, t.AdmissionDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    SicknessCount = g.Count()
                })
                .ToList();

            // Присваиваем данные DataGrid
            SicknessStatsGrid.ItemsSource = sicknessStats.Select(x => new
            {
                Year = x.Year,
                Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month),
                SicknessCount = x.SicknessCount
            }).ToList();

            // Обновляем метки месяцев для диаграммы
            MonthLabels = sicknessStats.Select(x => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month)).Distinct().ToList();
        }

        private void SetupChart(int year, int? month)
        {
            var sicknessStats = AppConnect.model.Talon
                .Where(t => t.AdmissionDate.Year == year && (!month.HasValue || t.AdmissionDate.Month == month))
                .GroupBy(t => t.AdmissionDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    SicknessCount = g.Count()
                })
                .ToList();

            SicknessChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = $"Заболеваемость за {year} {month?.ToString() ?? ""}",
                    Values = new ChartValues<int>(sicknessStats.Select(s => s.SicknessCount)),
                    DataLabels = true,
                    LabelPoint = point => point.Y.ToString(),
                    Fill = new SolidColorBrush(Color.FromRgb(0, 120, 215))
                }
            };

            Formatter = value => value.ToString("N0");
            SicknessChart.AxisX[0].Labels = sicknessStats.Select(s => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(s.Month)).ToList();
        }

        private void YearSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YearSelector.SelectedItem != null)
            {
                selectedYear = (int)YearSelector.SelectedItem;
                LoadSicknessData(selectedYear, selectedMonth);
                SetupChart(selectedYear, selectedMonth);
            }
        }

        private void MonthSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MonthSelector.SelectedItem != null)
            {
                selectedMonth = MonthSelector.SelectedIndex + 1; // Индексы месяцев начинаются с 0
                LoadSicknessData(selectedYear, selectedMonth);
                SetupChart(selectedYear, selectedMonth);
            }
        }

        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            // Формируем документ для печати
            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(50);
            doc.ColumnWidth = double.PositiveInfinity;
            doc.FontFamily = new System.Windows.Media.FontFamily("Arial");
            doc.FontSize = 14;

            // Создаем заголовок
            Paragraph header = new Paragraph(new Run("Статистика заболеваемости"));
            header.FontSize = 18;
            header.FontWeight = FontWeights.Bold;
            header.TextAlignment = TextAlignment.Center;
            doc.Blocks.Add(header);

            // Создаем таблицу для данных из DataGrid
            Table table = new Table();
            table.CellSpacing = 0;
            table.BorderBrush = System.Windows.Media.Brushes.Black;
            table.BorderThickness = new Thickness(1);

            // Определяем колонки таблицы
            for (int i = 0; i < SicknessStatsGrid.Columns.Count; i++)
            {
                table.Columns.Add(new TableColumn() { Width = new GridLength(200) });
            }

            // Заголовок таблицы
            TableRowGroup headerRowGroup = new TableRowGroup();
            TableRow headerRow = new TableRow();
            foreach (var column in SicknessStatsGrid.Columns)
            {
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run(column.Header.ToString())))
                {
                    FontWeight = FontWeights.Bold
                });
            }
            headerRowGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerRowGroup);

            // Заполняем данные из DataGrid
            TableRowGroup dataRowGroup = new TableRowGroup();
            foreach (var item in SicknessStatsGrid.Items)
            {
                TableRow dataRow = new TableRow();
                foreach (var column in SicknessStatsGrid.Columns)
                {
                    var cellContent = column.GetCellContent(item) as TextBlock;
                    string cellText = cellContent != null ? cellContent.Text : string.Empty;
                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(cellText))));
                }
                dataRowGroup.Rows.Add(dataRow);
            }
            table.RowGroups.Add(dataRowGroup);

            // Добавляем таблицу в документ
            doc.Blocks.Add(table);

            // Отправляем документ на печать
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Печать статистики заболеваемости");
            }
        }
    }
}
