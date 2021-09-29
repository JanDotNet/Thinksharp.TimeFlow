using Flee.PublicTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinksharp.TimeFlow.Engine.Actions
{
  public class Calculate : ActionBase
  {
    public static class CalculationMethods
    {
      public static decimal If(bool condition, decimal trueValue, decimal falseVale)
          => condition ? trueValue : falseVale;
    }

    protected override Task ExecuteInternalAsync(ExecutionContext ctx)
    {
      ExpressionContext expressionContext = new ExpressionContext();

      expressionContext.Imports.AddType(typeof(CalculationMethods));

      // initialize variables with dummy values.
      foreach (var ts in ctx.Frame)
      {
        expressionContext.Variables[ts.Key] = 1M;
      }

      var evaluator = expressionContext.CompileGeneric<decimal>(this.Formula);
      var usedVariables = evaluator.Info.GetReferencedVariables();

      var usedFrame = ctx.Frame[usedVariables];

      var calculatedTimeSeries = TimeSeries.Factory.FromGenerator(usedFrame.Start, usedFrame.End, usedFrame.Frequency, tp =>
      {
        foreach (var ts in usedFrame)
        {
          expressionContext.Variables[ts.Key] = ts.Value[tp] ?? 0;
        }

        return evaluator.Evaluate();
      });

      ctx.Frame.Add(this.Name, calculatedTimeSeries);

      return Task.CompletedTask;
    }

    public string Name { get; private set; }
    public string Formula { get; private set; }
  }
}
