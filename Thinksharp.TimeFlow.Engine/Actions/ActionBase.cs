using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Thinksharp.TimeFlow.Engine.Helper;

namespace Thinksharp.TimeFlow.Engine.Actions
{
  public abstract class ActionBase
      : IAction
  {
    public Guid Id { get; } = Guid.NewGuid();

    public Task ExecuteAsync(ExecutionContext context)
    {
      return this.ExecuteInternalAsync(context);
    }

    protected abstract Task ExecuteInternalAsync(ExecutionContext ctx);

    public static IAction FromXElement<TAction>(XElement element, ExecutionContext ctx) where TAction : IAction, new()
    {
      var properties = typeof(TAction).GetProperties();

      var action = new TAction();

      foreach (var property in properties)
      {
        if (property.Name == nameof(action.Id))
        {
          continue;
        }

        var strFromXml = element.Attribute(property.Name)?.Value ?? "";

        // try set value from parameter
        if (strFromXml.StartsWith("$"))
        {
          strFromXml = strFromXml.Substring(1);
          var valueFromParameter = ctx.GetParameter(strFromXml, property.PropertyType);
          property.SetValue(action, valueFromParameter);
        }
        // set parsed value from XML
        else
        {
          var objFromXml = strFromXml.Parse(property.PropertyType);
          var valueFromXml = objFromXml.GetValueOrThrowError();
          property.SetValue(action, valueFromXml);
        }
      }

      return action;
    }       
  }
}
