using System.Threading.Tasks;

namespace Thinksharp.TimeFlow.Engine.Actions
{
  public class ReSample : ActionBase
  {
    protected override Task ExecuteInternalAsync(ExecutionContext ctx)
    {
      ctx.Frame.ReSample(this.Period, this.AggregationType);

      return Task.CompletedTask;
    }

    public Period Period { get; private set; }

    public AggregationType AggregationType { get; private set; }
  }
}
