using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using ClosedXML.Excel; // Не забудьте установить NuGet-пакет ClosedXML

public static class ExportHelper
{
    public static void ExportToExcel(DataTable dataTable)
    {
        try
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(dataTable, "Данные");
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Сохранить как Excel файл"
                };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    workbook.SaveAs(saveFileDialog.FileName);
                    MessageBox.Show("Данные успешно экспортированы!");
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при экспорте: {ex.Message}");
        }
    }
}