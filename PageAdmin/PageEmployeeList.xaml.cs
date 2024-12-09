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
    /// Логика взаимодействия для PageEmployeeList.xaml
    /// </summary>
    public partial class PageEmployeeList : Page
    {
        public PageEmployeeList()
        {
            InitializeComponent();
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            try
            {
                var employees = (from user in AppConnect.model.User
                                 join role in AppConnect.model.Role
                                 on user.RoleId equals role.Id
                                 select new
                                 {
                                     FirstName = user.FirstName,
                                     Name = user.Name,
                                     Patronymic = user.Patronymic,
                                     Login = user.Username,
                                     Password = user.Password,
                                     RoleName = role.RoleName
                                 }).ToList();

                EmployeeDataGrid.ItemsSource = employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных сотрудников: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintEmployeeListBtn_Click(object sender, RoutedEventArgs e)
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
                Paragraph header = new Paragraph(new Run("Список сотрудников"));
                header.FontSize = 20;
                header.FontWeight = FontWeights.Bold;
                header.TextAlignment = TextAlignment.Center;
                doc.Blocks.Add(header);

                // Создание таблицы для данных сотрудников
                Table employeeTable = new Table();
                doc.Blocks.Add(employeeTable);

                // Определение столбцов таблицы
                employeeTable.Columns.Add(new TableColumn() { Width = new GridLength(100) });
                employeeTable.Columns.Add(new TableColumn() { Width = new GridLength(100) });
                employeeTable.Columns.Add(new TableColumn() { Width = new GridLength(100) });
                employeeTable.Columns.Add(new TableColumn() { Width = new GridLength(100) });
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
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Логин"))) { FontWeight = FontWeights.Bold });
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Пароль"))) { FontWeight = FontWeights.Bold });
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Роль"))) { FontWeight = FontWeights.Bold });

                // Заполнение данных из базы
                TableRowGroup dataRows = new TableRowGroup();
                employeeTable.RowGroups.Add(dataRows);

                var employees = AppConnect.model.User.ToList();
                foreach (var employee in employees)
                {
                    TableRow row = new TableRow();
                    dataRows.Rows.Add(row);

                    row.Cells.Add(new TableCell(new Paragraph(new Run(employee.FirstName))));
                    row.Cells.Add(new TableCell(new Paragraph(new Run(employee.Name))));
                    row.Cells.Add(new TableCell(new Paragraph(new Run(employee.Patronymic))));
                    row.Cells.Add(new TableCell(new Paragraph(new Run(employee.Username))));
                    row.Cells.Add(new TableCell(new Paragraph(new Run(employee.Password))));
                    row.Cells.Add(new TableCell(new Paragraph(new Run(AppConnect.model.Role.FirstOrDefault(r => r.Id == employee.RoleId)?.RoleName ?? ""))));
                }

                // Отправка документа на печать
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    IDocumentPaginatorSource paginatorSource = doc;
                    printDialog.PrintDocument(paginatorSource.DocumentPaginator, "Печать списка сотрудников");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при печати: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
