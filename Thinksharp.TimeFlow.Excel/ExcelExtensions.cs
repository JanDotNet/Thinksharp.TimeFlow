using ClosedXML.Excel;
using System;
using System.Diagnostics;
using System.IO;

namespace Thinksharp.TimeFlow
{
  public static class ExcelExtensions
  {
    public static void ShowExcel(this TimeFrame frame)
    {
      var row = 2;
      var col = 3;
      using (var workbook = new XLWorkbook())
      {
        var worksheet = workbook.Worksheets.Add("Sample Sheet");

        // header
        worksheet.Cell(1, 1).Value = "Date Time";
        worksheet.Cell(1, 2).Value = "Offset";
        foreach (var timeSeries in frame)
        {
          worksheet.Cell(1, col++).Value = timeSeries.Key;
        }
        

        foreach (var timePoint in frame.IterateTimePoints())
        {
          // header
          worksheet.Cell(row, 1).Value = timePoint;
          worksheet.Cell(row, 2).Value = timePoint.Offset.TotalHours;
          col = 3;
          foreach (var timeSeries in frame)
          {
            worksheet.Cell(row, col++).Value = timeSeries.Value[timePoint];
          }
          row++;
        }

        worksheet.Column(1).CellsUsed().Style.DateFormat.Format = "dd.MM.yyyy HH:mm";
        worksheet.ColumnsUsed().AdjustToContents();

        worksheet.SheetView.Freeze(1, 2);


        var file = Path.GetTempFileName() + ".xlsx";
        workbook.SaveAs(file);

        var process = Process.Start(file);

        process.Exited += (s, e) =>
        {
          try
          {
            File.Delete(file);
          }
          catch (Exception ex)
          {

          }
        };
      }
    }

    public static void ShowExcel(this TimeSeries timeSeries)
    {
      var row = 2;
      using (var workbook = new XLWorkbook())
      {
        var worksheet = workbook.Worksheets.Add("Sample Sheet");

        // header
        worksheet.Cell(1, 1).Value = "Date Time";
        worksheet.Cell(1, 2).Value = "Value";

        foreach (var timePoint in timeSeries)
        {
          worksheet.Cell(row, 1).Value = timePoint.Key.DateTime;
          worksheet.Cell(row, 2).Value = timePoint.Value;
          row++;
        }

        var file = Path.GetTempFileName() + ".xlsx";
        workbook.SaveAs(file);

        var process = Process.Start(file);

        process.Exited += (s, e) =>
        {
          try

          {
            File.Delete(file);
          }
          catch (Exception ex)
          {

          }
        };
      }
    }
  }
}
