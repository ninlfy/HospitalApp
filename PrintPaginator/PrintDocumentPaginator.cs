using System;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;

public class PrintDocumentPaginator : DocumentPaginator
{
    private readonly string printContent;
    private readonly Typeface typeface;
    private readonly double fontSize;

    // Задаем размеры страницы A4 в единицах WPF (1 дюйм = 96 пикселей)
    private const double PageWidth = 8.27 * 96; // Ширина A4 в пикселях
    private const double PageHeight = 11.69 * 96; // Высота A4 в пикселях

    public PrintDocumentPaginator(string content)
    {
        printContent = content;
        typeface = new Typeface("Arial");
        fontSize = 12;
    }

    public override DocumentPage GetPage(int pageNumber)
    {
        var formattedText = new FormattedText(printContent, System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black, VisualTreeHelper.GetDpi(new DrawingVisual()).PixelsPerDip);

        var visual = new DrawingVisual();
        using (var drawingContext = visual.RenderOpen())
        {
            drawingContext.DrawText(formattedText, new Point(0, 0));
        }

        return new DocumentPage(visual);
    }

    public override bool IsPageCountValid => true;
    public override int PageCount => 1; // Для простоты, печатаем только одну страницу
    public override Size PageSize { get; set; } = new Size(PageWidth, PageHeight); // Устанавливаем размер страницы
    public override IDocumentPaginatorSource Source => null; // Можно настроить, если есть источник

    public override void ComputePageCount()
    {
        // Здесь можно установить количество страниц, если контент большой
    }
}