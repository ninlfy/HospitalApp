using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;

namespace HospitalApp.Control
{
    public class DoctorWorkloadPaginator : DocumentPaginator
    {
        private readonly List<DoctorWorkloadData> doctorData; // Данные для печати
        private readonly Typeface typeface;
        private readonly double fontSize;

        // Задаем размеры страницы A4 в единицах WPF (1 дюйм = 96 пикселей)
        private const double PageWidth = 8.27 * 96; // Ширина A4 в пикселях
        private const double PageHeight = 11.69 * 96; // Высота A4 в пикселях

        public DoctorWorkloadPaginator(List<DoctorWorkloadData> data)
        {
            doctorData = data;
            typeface = new Typeface("Arial");
            fontSize = 12;
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            // Создаем визуал для рендеринга страницы
            var visual = new DrawingVisual();
            using (var drawingContext = visual.RenderOpen())
            {
                // Устанавливаем начальные координаты для текста
                double y = 50; // Отступ сверху
                double x = 50; // Отступ слева

                // Заголовок страницы
                var headerText = new FormattedText("Список загруженности врачей",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, typeface, fontSize + 4, Brushes.Black, VisualTreeHelper.GetDpi(visual).PixelsPerDip);
                drawingContext.DrawText(headerText, new Point(x, y));
                y += 50; // Смещаемся вниз для начала таблицы

                // Отрисовка заголовков таблицы
                string[] headers = { "ФИО", "Специальность", "Талонов за месяц", "Талонов за год" };
                double[] columnWidths = { 250, 200, 150, 150 };

                for (int i = 0; i < headers.Length; i++)
                {
                    var columnHeader = new FormattedText(headers[i],
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black, VisualTreeHelper.GetDpi(visual).PixelsPerDip);
                    drawingContext.DrawText(columnHeader, new Point(x, y));
                    x += columnWidths[i];
                }

                y += 30; // Смещаемся вниз для данных
                x = 50; // Возвращаемся к начальной координате по X

                // Отрисовка данных врачей
                foreach (var doctor in doctorData)
                {
                    drawingContext.DrawText(new FormattedText(doctor.FullName,
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black, VisualTreeHelper.GetDpi(visual).PixelsPerDip),
                        new Point(x, y));

                    drawingContext.DrawText(new FormattedText(doctor.SpecialyName,
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black, VisualTreeHelper.GetDpi(visual).PixelsPerDip),
                        new Point(x + columnWidths[0], y));

                    drawingContext.DrawText(new FormattedText(doctor.MonthlyAppointments.ToString(),
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black, VisualTreeHelper.GetDpi(visual).PixelsPerDip),
                        new Point(x + columnWidths[0] + columnWidths[1], y));

                    drawingContext.DrawText(new FormattedText(doctor.YearlyAppointments.ToString(),
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black, VisualTreeHelper.GetDpi(visual).PixelsPerDip),
                        new Point(x + columnWidths[0] + columnWidths[1] + columnWidths[2], y));

                    y += 30; // Смещаемся вниз для следующего врача
                    if (y > PageHeight - 100) // Переход на новую страницу, если места недостаточно
                        break;
                }
            }

            return new DocumentPage(visual, new Size(PageWidth, PageHeight), new Rect(new Size(PageWidth, PageHeight)), new Rect(new Size(PageWidth, PageHeight)));
        }

        public override bool IsPageCountValid => true;
        public override int PageCount => 1; // Для простоты считаем, что все данные вмещаются на одной странице
        public override Size PageSize { get; set; } = new Size(PageWidth, PageHeight);
        public override IDocumentPaginatorSource Source => null;
    }

    public class DoctorWorkloadData
    {
        public string FullName { get; set; }
        public string SpecialyName { get; set; }
        public int MonthlyAppointments { get; set; }
        public int YearlyAppointments { get; set; }
    }
}
