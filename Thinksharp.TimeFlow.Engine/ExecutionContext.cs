using System;
using System.Collections.Generic;
using Thinksharp.TimeFlow.Engine.Helper;

namespace Thinksharp.TimeFlow.Engine
{
  public class ExecutionContext
  {
    public ExecutionContext(Dictionary<string, string> paramaters)
    {
      this.Parameters = paramaters;
    }

    public TimeFrame Frame { get; } = new TimeFrame();

    public Dictionary<string, string> Parameters { get; } = new Dictionary<string, string>();

    public TValue GetParameter<TValue>(string name)
      => (TValue)GetParameter(name, typeof(TValue));

    public object GetParameter(string name, Type parameterType)
    {
      if (!this.Parameters.TryGetValue(name, out var valueStr))
      {
        throw new InvalidOperationException($"Parameter '{name}' is not defined.");
      }

      var result = valueStr.Parse(parameterType);

      return result.GetValueOrThrowError();
    }
  }
}
