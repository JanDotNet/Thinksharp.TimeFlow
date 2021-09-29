using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Thinksharp.TimeFlow.Engine;
using Thinksharp.TimeFlow.Engine.Actions;

namespace Thinksharp.TimeFlow.Excel.Actions
{
  public enum TimeAxisOrientation
  {
    Horizontal,
    Vertical
  }

  public class WriteToExcel : ActionBase
  {
    public string ExcelFile { get; set; }
    public string SheetName { get; set; }
    public TimeAxisOrientation Orientation { get; set; }


    protected override Task ExecuteInternalAsync(ExecutionContext ctx)
    {
      using (var workbook = new XLWorkbook())
      {
        var sheetName = string.IsNullOrEmpty(this.SheetName) ? "Data" : this.SheetName;

        var worksheet = workbook.Worksheets.Add(sheetName);

        switch (this.Orientation)
        {
          case TimeAxisOrientation.Horizontal:
            // write header
            var col = 1;
            var row = 1;
            worksheet.Cell(row, col++).Value = "";
            foreach (var ts in ctx.Frame.EnumerateTimePoints())
            {
              worksheet.Cell(row, col++).Value = ts.DateTime;
            }

            // write data
            foreach (var ts in ctx.Frame)
            {
              row++;
              col = 1;
              worksheet.Cell(row, col++).Value = ts.Key;
              foreach (var tp in ctx.Frame.EnumerateTimePoints())
              {
                worksheet.Cell(row, col++).Value = ts.Value[tp] ?? 0;
              }
            }
            break;
          case TimeAxisOrientation.Vertical:
            // write header
            col = 1;
            row = 1;
            foreach (var ts in ctx.Frame)
            {
              worksheet.Cell(row, col++).Value = ts.Key;
            }
            
            // write data
            foreach (var tp in ctx.Frame.EnumerateTimePoints())
            {
              row++;
              col = 1;
              worksheet.Cell(row, col++).Value = tp.DateTime;
              foreach (var ts in ctx.Frame)
              {
                worksheet.Cell(row, col++).Value = ts.Value[tp] ?? 0;
              }
            }
            break;
        }

        if (ctx.Frame.Frequency.Unit < PeriodUnit.Day)
        {
          worksheet.Column(1).CellsUsed().Style.DateFormat.Format = "dd.MM.yyyy HH:mm";
        }
        worksheet.ColumnsUsed().AdjustToContents();

        worksheet.SheetView.Freeze(1, 2);

        workbook.SaveAs(this.ExcelFile);

        return Task.CompletedTask;
      }
    }
  }
}
