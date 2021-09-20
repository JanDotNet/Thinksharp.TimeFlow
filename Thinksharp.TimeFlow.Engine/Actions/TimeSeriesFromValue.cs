using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thinksharp.TimeFlow.Engine.Actions
{
    public class TimeSeriesFromValue : ActionBase
    {
        protected override Task ExecuteInternalAsync(ExecutionContext ctx)
        {
            var ts = TimeSeries.Factory.FromValue(Value, From, To, Period);
            ctx.Frame.Add(this.Name, ts);

            return Task.CompletedTask;
        }

        public string Name { get; private set; }
        public decimal? Value { get; private set; }
        public DateTime From { get; private set; }
        public DateTime To { get; private set; }
        public Period Period { get; private set; }
    }
}
