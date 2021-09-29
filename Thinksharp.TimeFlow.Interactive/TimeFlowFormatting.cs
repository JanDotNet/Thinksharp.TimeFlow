using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Formatting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Microsoft.DotNet.Interactive.Formatting.PocketViewTags;

namespace Thinksharp.TimeFlow
{
  public static class TimeFlowFormatting
  {
    public static void RegisterTimeSeries()
    {
      Formatter.Register<TimeSeries>((ts, writer) =>
      {
        var headers = new List<IHtmlContent>();
        headers.Add(th(i("time")));
        headers.Add(th(i("value")));
        var rows = new List<List<IHtmlContent>>();
        foreach (var tp in ts)
        {
          var cells = new List<IHtmlContent>();
          cells.Add(td(tp.Key));
          cells.Add(td(tp.Value));
          rows.Add(cells);
        }

        var originalRowCount = rows.Count;
        if (rows.Count > 20)
        {
          rows = rows.Take(5).Concat(rows.Skip(rows.Count - 5)).ToList();
          var cells = new List<IHtmlContent>();
          cells.Add(td("..."));
          cells.Add(td($"{originalRowCount - 10} more"));
          rows.Insert(5, cells);
        }

        var t = table(
            thead(
                headers),
            tbody(
                rows.Select(
                    r => tr(r))));

        writer.Write(t);
      }, "text/html");
    }

    public static void RegisterTimeFrame()
    {
      Formatter.Register<TimeFrame>((tf, writer) =>
      {
        var headers = new List<IHtmlContent>();
        headers.Add(th(i("time")));
        foreach (var ts_name in tf.Select(ts => ts.Key))
        {
          headers.Add(th(i(ts_name)));
        }
        var rows = new List<List<IHtmlContent>>();
        foreach (var tp in tf.EnumerateTimePoints())
        {
          var cells = new List<IHtmlContent>();
          cells.Add(td(tp));
          foreach (var ts in tf.Select(p => p.Value))
          {
            cells.Add(td(ts[tp]));
          }
          rows.Add(cells);
        }

        var originalRowCount = rows.Count;
        if (rows.Count > 32)
        {
          rows = rows.Take(10).Concat(rows.Skip(rows.Count - 10)).ToList();
          var cells = new List<IHtmlContent>();
          cells.Add(td("..."));
          cells.Add(td($"{originalRowCount - 20} more"));
          rows.Insert(10, cells);
        }

        var t = table(
            thead(
                headers),
            tbody(
                rows.Select(
                    r => tr(r))));

        writer.Write(t);
      }, "text/html");
    }
  }

  public class TimeFlowKernelExtensions : IKernelExtension
  {
    public Task OnLoadAsync(Kernel kernel)
    {
      TimeFlowFormatting.RegisterTimeSeries();

      TimeFlowFormatting.RegisterTimeFrame();

      if (KernelInvocationContext.Current != null)
      {
        KernelInvocationContext.Current.Display("TimeFlow formatter registered.", "text/markdown");
      }

      return Task.CompletedTask;
    }
  }
}
