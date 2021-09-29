using Microsoft.DotNet.Interactive;
using System;
using System.Linq;
using XPlot.Plotly;

namespace Thinksharp.TimeFlow
{
  public static class TimeFlowExtensions
  {
    public static DisplayedValue Plot(this TimeSeries timeSeries, string chartTitle = null, string xAxisTitel = null, string yAxisTitle = null)
    {
      var chart = Chart.Plot(
        new Scatter
        {          
            x = timeSeries.TimePoints,
            y = timeSeries.Values,
            mode = "lines"
        });

      var chartLayout = new Layout.Layout
      {
        title = chartTitle,
        xaxis = new Xaxis { title = xAxisTitel ?? $"Time ({timeSeries.Frequency})" },
        yaxis = new Yaxis { title = yAxisTitle ?? "Values" }
      };

      chart.WithLayout(chartLayout);

      return chart.Display();
    }

    public static DisplayedValue Plot(this TimeFrame timeFrame, string chartTitle = null, string xAxisTitel = null, string yAxisTitle = null)
    {
      var charts = timeFrame.Select(ts => new Scatter
      {
        name = ts.Key,
        x = ts.Value.TimePoints,
        y = ts.Value.Values,
        mode = "lines"
      }).ToList();

      var chart = Chart.Plot(charts);

      var chartLayout = new Layout.Layout
      {
        title = chartTitle,
        xaxis = new Xaxis { title = xAxisTitel ?? $"Time (Freq: {timeFrame.Frequency})" },
        yaxis = new Yaxis { title = yAxisTitle ?? "Values" }
      };

      chart.WithLayout(chartLayout);

      return chart.Display();
    }
  }
}
