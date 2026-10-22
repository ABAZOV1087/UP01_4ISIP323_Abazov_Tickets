using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class DiagrammPage : Page
    {
        public DiagrammPage()
        {
            InitializeComponent();
            ChartPayments.ChartAreas.Add(new ChartArea("Main"));

            var currentSeries = new Series("Продажи билетов")
            {
                IsValueShownAsLabel = true
            };
            ChartPayments.Series.Add(currentSeries);

            CmbUser.ItemsSource = Core.Context.User.ToList();
            CmbDiagram.ItemsSource = Enum.GetValues(typeof(SeriesChartType));

            if (CmbUser.Items.Count > 0) CmbUser.SelectedIndex = 0;
            CmbDiagram.SelectedItem = SeriesChartType.Column;
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (CmbUser.SelectedItem is User currentUser && CmbDiagram.SelectedItem is SeriesChartType currentType)
            {
                Series currentSeries = ChartPayments.Series.FirstOrDefault();
                currentSeries.ChartType = currentType;
                currentSeries.Points.Clear();

                var categoriesList = Core.Context.Category.ToList();
                foreach (var category in categoriesList)
                {
                    currentSeries.Points.AddXY(category.Name,
                        Core.Context.TicketSale.ToList()
                            .Where(u => u.UserID == currentUser.ID && u.CategoryID == category.ID)
                            .Sum(u => u.Price * u.Num));
                }
            }
        }

        private void BtnExportWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var allUsers = Core.Context.User.ToList();
                var allCategories = Core.Context.Category.ToList();

                Type wordType = Type.GetTypeFromProgID("Word.Application");
                if (wordType == null) { Core.ShowError("Microsoft Word не установлен!"); return; }

                dynamic application = Activator.CreateInstance(wordType);
                dynamic document = application.Documents.Add();

                foreach (var user in allUsers)
                {
                    dynamic userParagraph = document.Paragraphs.Add();
                    dynamic userRange = userParagraph.Range;
                    userRange.Text = user.FIO;
                    userRange.Font.Name = "Times New Roman";
                    userRange.Font.Size = 16;
                    userRange.Font.Bold = 1;
                    userRange.ParagraphFormat.Alignment = 1;
                    userRange.InsertParagraphAfter();
                    document.Paragraphs.Add();

                    dynamic tableParagraph = document.Paragraphs.Add();
                    dynamic tableRange = tableParagraph.Range;
                    dynamic paymentsTable = document.Tables.Add(tableRange, allCategories.Count() + 1, 2);
                    paymentsTable.Borders.InsideLineStyle = 1;
                    paymentsTable.Borders.OutsideLineStyle = 1;
                    paymentsTable.Range.Cells.VerticalAlignment = 1;

                    dynamic cellRange = paymentsTable.Cell(1, 1).Range;
                    cellRange.Text = "Категория";
                    cellRange = paymentsTable.Cell(1, 2).Range;
                    cellRange.Text = "Сумма расходов";

                    paymentsTable.Rows[1].Range.Font.Name = "Times New Roman";
                    paymentsTable.Rows[1].Range.Font.Size = 14;
                    paymentsTable.Rows[1].Range.Bold = 1;
                    paymentsTable.Rows[1].Range.ParagraphFormat.Alignment = 1;

                    for (int i = 0; i < allCategories.Count(); i++)
                    {
                        var currentCategory = allCategories[i];
                        cellRange = paymentsTable.Cell(i + 2, 1).Range;
                        cellRange.Text = currentCategory.Name;
                        cellRange.Font.Name = "Times New Roman";
                        cellRange.Font.Size = 12;

                        cellRange = paymentsTable.Cell(i + 2, 2).Range;
                        decimal sum = user.TicketSale.ToList()
                            .Where(u => u.CategoryID == currentCategory.ID)
                            .Sum(u => u.Num * u.Price);
                        cellRange.Text = sum.ToString("N2") + " руб.";
                        cellRange.Font.Name = "Times New Roman";
                        cellRange.Font.Size = 12;
                    }
                    document.Paragraphs.Add();

                    TicketSale maxPayment = user.TicketSale.OrderByDescending(u => u.Price * u.Num).FirstOrDefault();
                    if (maxPayment != null)
                    {
                        dynamic maxPaymentParagraph = document.Paragraphs.Add();
                        dynamic maxPaymentRange = maxPaymentParagraph.Range;
                        maxPaymentRange.Text = $"Самый дорогостоящий платеж - {maxPayment.Name} за {(maxPayment.Price * maxPayment.Num).ToString("N2")} руб. от {maxPayment.Date.ToString("dd.MM.yyyy")}";
                        maxPaymentRange.Font.Name = "Times New Roman";
                        maxPaymentRange.Font.Size = 13;
                        maxPaymentRange.Font.Bold = 1;
                        maxPaymentRange.Font.Color = 128;
                        maxPaymentRange.InsertParagraphAfter();
                    }
                    document.Paragraphs.Add();

                    TicketSale minPayment = user.TicketSale.OrderBy(u => u.Price * u.Num).FirstOrDefault();
                    if (minPayment != null)
                    {
                        dynamic minPaymentParagraph = document.Paragraphs.Add();
                        dynamic minPaymentRange = minPaymentParagraph.Range;
                        minPaymentRange.Text = $"Самый дешевый платеж - {minPayment.Name} за {(minPayment.Price * minPayment.Num).ToString("N2")} руб. от {minPayment.Date.ToString("dd.MM.yyyy")}";
                        minPaymentRange.Font.Name = "Times New Roman";
                        minPaymentRange.Font.Size = 13;
                        minPaymentRange.Font.Bold = 1;
                        minPaymentRange.Font.Color = 32768;
                        minPaymentRange.InsertParagraphAfter();
                    }

                    if (user != allUsers.LastOrDefault())
                        document.Words.Last.InsertBreak(7);
                }

                foreach (dynamic section in document.Sections)
                {
                    dynamic footer = section.Footers[1];
                    footer.PageNumbers.Add(1);

                    dynamic headerRange = section.Headers[1].Range;
                    headerRange.ParagraphFormat.Alignment = 1;
                    headerRange.Font.ColorIndex = 1;
                    headerRange.Font.Size = 10;
                    headerRange.Text = DateTime.Now.ToString("dd/MM/yyyy");
                }

                application.Visible = true;
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                document.SaveAs2(Path.Combine(desktop, "Payments.docx"));
                document.SaveAs2(Path.Combine(desktop, "Payments.pdf"), 17);
            }
            catch (Exception ex)
            {
                Core.ShowError("Ошибка выгрузки в Word: " + ex.Message);
            }
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var allUsers = Core.Context.User.OrderBy(u => u.FIO).ToList();
                Type excelType = Type.GetTypeFromProgID("Excel.Application");
                if (excelType == null)
                {
                    Core.ShowError("Microsoft Excel не установлен!");
                    return;
                }

                dynamic application = Activator.CreateInstance(excelType);
                application.SheetsInNewWorkbook = allUsers.Count();
                dynamic workbook = application.Workbooks.Add();

                for (int i = 0; i < allUsers.Count(); i++)
                {
                    int startRowIndex = 1;
                    dynamic worksheet = application.Worksheets.Item[i + 1];
                    string sheetName = allUsers[i].FIO.Length > 30 ? allUsers[i].FIO.Substring(0, 30) : allUsers[i].FIO;
                    worksheet.Name = sheetName;

                    worksheet.Cells[startRowIndex, 1] = "Дата платежа";
                    worksheet.Cells[startRowIndex, 2] = "Название";
                    worksheet.Cells[startRowIndex, 3] = "Стоимость";
                    worksheet.Cells[startRowIndex, 4] = "Количество";
                    worksheet.Cells[startRowIndex, 5] = "Сумма";

                    dynamic colHeaderRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, 5]];
                    colHeaderRange.HorizontalAlignment = -4108;
                    colHeaderRange.Font.Bold = true;
                    startRowIndex++;

                    var userCategories = allUsers[i].TicketSale
                        .OrderBy(u => u.Date)
                        .GroupBy(u => u.Category)
                        .OrderBy(u => u.Key.Name);

                    foreach (var groupCategory in userCategories)
                    {
                        dynamic headerRange = worksheet.Range[worksheet.Cells[startRowIndex, 1], worksheet.Cells[startRowIndex, 5]];
                        headerRange.Merge();
                        headerRange.Value = groupCategory.Key.Name;
                        headerRange.HorizontalAlignment = -4108;
                        headerRange.Font.Italic = true;
                        startRowIndex++;

                        int categoryFirstRow = startRowIndex;

                        foreach (var payment in groupCategory)
                        {
                            worksheet.Cells[startRowIndex, 1] = payment.Date.ToString("dd.MM.yyyy");
                            worksheet.Cells[startRowIndex, 2] = payment.Name;
                            worksheet.Cells[startRowIndex, 3] = (double)payment.Price;
                            worksheet.Cells[startRowIndex, 4] = (double)payment.Num;
                            worksheet.Cells[startRowIndex, 5].Formula = $"=C{startRowIndex}*D{startRowIndex}";
                            startRowIndex++;
                        }

                        dynamic sumRange = worksheet.Range[worksheet.Cells[startRowIndex, 1], worksheet.Cells[startRowIndex, 4]];
                        sumRange.Merge();
                        sumRange.Value = "ИТОГО:";
                        sumRange.HorizontalAlignment = -4152;
                        sumRange.Font.Bold = true;

                        worksheet.Cells[startRowIndex, 5].Formula = $"=SUM(E{categoryFirstRow}:E{startRowIndex - 1})";
                        worksheet.Cells[startRowIndex, 5].Font.Bold = true;
                        startRowIndex++;
                    }

                    if (startRowIndex > 2)
                    {
                        dynamic rangeBorders = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[startRowIndex - 1, 5]];
                        rangeBorders.Borders.LineStyle = 1;
                    }
                    worksheet.Columns.AutoFit();
                }

                application.Visible = true;
                application.DisplayAlerts = false;
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                workbook.SaveAs(Path.Combine(desktop, "Payments.xlsx"));
                application.DisplayAlerts = true;
            }
            catch (Exception ex)
            {
                Core.ShowError("Ошибка выгрузки в Excel: " + ex.Message);
            }
        }


    }
}