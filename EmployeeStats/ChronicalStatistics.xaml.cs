using HospitalApp.ApplicationData;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace HospitalApp.EmployeeStats
{
    public partial class ChronicalStatistics : Page
    {

        public ChronicalStatistics()
        {
            InitializeComponent();
            LoadDiseases();
            LoadPatients();
            BuildChart();
        }

        /// <summary>
        /// Загрузка списка заболеваний в ComboBox
        /// </summary>
        private void LoadDiseases()
        {
            try
            {
                var diseases = AppConnect.model.Chronical
                    .Select(c => new { c.Id, c.ChronicalName })
                    .OrderBy(c => c.ChronicalName)
                    .ToList();

                DiseaseFilter.ItemsSource = diseases;
                DiseaseFilter.DisplayMemberPath = "ChronicalName";
                DiseaseFilter.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки списка заболеваний: {ex.Message}");
            }
        }

        /// <summary>
        /// Загрузка пациентов в таблицу
        /// </summary>
        private void LoadPatients(int? diseaseId = null)
        {
            try
            {
                var query = AppConnect.model.PatientCard
                    .Where(p => p.ChronicalId != null);

                if (diseaseId.HasValue)
                {
                    query = query.Where(p => p.ChronicalId == diseaseId.Value);
                }

                var patients = query
                .ToList() // Выполняем запрос к базе данных
                .Select(p => new
                 {
                 FullName = p.FirstName + " " + p.LastName + " " + p.Patronymic,
                 BirthDate = p.DateBirth.ToString("dd.MM.yyyy"), // Здесь ToString уже выполняется в памяти
                 ChronicalName = p.Chronical.ChronicalName
                }).ToList();

                PatientsDataGrid.ItemsSource = patients;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        /// <summary>
        /// Построение диаграммы
        /// </summary>
        private void BuildChart()
        {
            try
            {
                // Группировка данных по заболеваниям
                var data = AppConnect.model.PatientCard
                    .Where(p => p.ChronicalId != null)
                    .GroupBy(p => p.Chronical.ChronicalName)
                    .Select(g => new
                    {
                        DiseaseName = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(g => g.Count)
                    .ToList();

                // Данные для диаграммы
                var values = new ChartValues<int>();
                var labels = new List<string>();

                foreach (var item in data)
                {
                    values.Add(item.Count);
                    labels.Add(item.DiseaseName);
                }

                // Установка данных диаграммы
                SicknessChart.Series = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Количество пациентов",
                        Values = values
                    }
                };

                SicknessChart.AxisX[0].Labels = labels;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка построения диаграммы: {ex.Message}");
            }
        }

        private void DiseaseFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DiseaseFilter.SelectedValue is int selectedDiseaseId)
            {
                LoadPatients(selectedDiseaseId);
            }
        }

        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            // Создаем документ для печати
            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(50);
            doc.ColumnWidth = double.PositiveInfinity;
            doc.FontFamily = new System.Windows.Media.FontFamily("Arial");
            doc.FontSize = 14;

            // Заголовок документа
            Paragraph header = new Paragraph(new Run("Список пациентов с хроническими заболеваниями"));
            header.FontSize = 18;
            header.FontWeight = FontWeights.Bold;
            header.TextAlignment = TextAlignment.Center;
            doc.Blocks.Add(header);

            // Таблица для данных
            Table table = new Table();
            table.CellSpacing = 0;
            table.BorderBrush = System.Windows.Media.Brushes.Black;
            table.BorderThickness = new Thickness(1);

            // Определяем колонки таблицы
            foreach (var column in PatientsDataGrid.Columns)
            {
                table.Columns.Add(new TableColumn());
            }

            // Заголовок таблицы
            TableRowGroup headerRowGroup = new TableRowGroup();
            TableRow headerRow = new TableRow();
            foreach (var column in PatientsDataGrid.Columns)
            {
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run(column.Header.ToString())))
                {
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center
                });
            }
            headerRowGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerRowGroup);

            // Данные таблицы
            TableRowGroup dataRowGroup = new TableRowGroup();
            foreach (var item in PatientsDataGrid.Items)
            {
                if (item != null)
                {
                    TableRow dataRow = new TableRow();
                    foreach (var column in PatientsDataGrid.Columns)
                    {
                        var cellContent = column.GetCellContent(item) as TextBlock;
                        string cellText = cellContent != null ? cellContent.Text : string.Empty;
                        dataRow.Cells.Add(new TableCell(new Paragraph(new Run(cellText))));
                    }
                    dataRowGroup.Rows.Add(dataRow);
                }
            }
            table.RowGroups.Add(dataRowGroup);

            // Добавляем таблицу в документ
            doc.Blocks.Add(table);

            // Печать
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Печать списка пациентов");
            }
        }

        private void ResetFiltersBtn_Click(object sender, RoutedEventArgs e)
        {
            DiseaseFilter.SelectedIndex = -1;
            LoadPatients();
        }
    }
}
