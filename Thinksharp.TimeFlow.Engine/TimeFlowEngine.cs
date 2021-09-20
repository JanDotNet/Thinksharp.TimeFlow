using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Thinksharp.TimeFlow.Engine.Actions;

namespace Thinksharp.TimeFlow.Engine
{
  public class TimeFlowEngine
  {
    private Dictionary<string, Func<XElement, ExecutionContext, IAction>> actionFactories = new Dictionary<string, Func<XElement, ExecutionContext, IAction>>();

    public async Task<ExecutionContext> ExecuteAsync(string xml, Dictionary<string, string> parameters)
    {
      var xDoc = GetXDocument(xml);

      // query parameters
      var defaultParameters = xDoc.Element("TimeSeriesTransformation").Element("Parameters")?.Elements("Paramater");
      var xActions = xDoc.Element("TimeSeriesTransformation").Element("Actions").Elements();

      parameters = new Dictionary<string, string>(parameters, StringComparer.InvariantCultureIgnoreCase);

      foreach (var xParameter in defaultParameters ?? new XElement[0])
      {
        var pName = xParameter.Attribute("Name")?.Value;
        var pValue = xParameter.Attribute("Value")?.Value;

        if (!parameters.ContainsKey(pName))
        {
          parameters.Add(pName, pValue);
        }
      }

      var executionContext = new ExecutionContext(parameters);

      // create all actions
      var actions = new List<IAction>();
      foreach (var xAction in xActions)
      {
        var name = xAction.Name.LocalName;

        if (!this.actionFactories.TryGetValue(name, out var actionFactory))
        {
          throw new FormatException($"Missing registration for action with name '{name}'.");
        }

        var action = actionFactory(xAction, executionContext);

        actions.Add(action);        
      }

      // execute actions
      foreach (var action in actions)
      {
        await action.ExecuteAsync(executionContext);
      }

      return executionContext;
    }

    private XDocument GetXDocument(string xml)
    {
      using (var reader = new StringReader(xml))
      {
        return XDocument.Load(reader);
      }
    }

    public void RegisterAction(string name, Func<XElement, ExecutionContext, IAction> actionFactory)
        => actionFactories.Add(name, actionFactory);

    public void RegisterAction<TAction>() where TAction : ActionBase, new()
        => actionFactories.Add(typeof(TAction).Name, ActionBase.FromXElement<TAction>);

    public void RegisterAction<TAction>(Func<XElement, ExecutionContext, TAction> actionFactory) where TAction : ActionBase, new()
        => actionFactories.Add(typeof(TAction).Name, actionFactory);
  }
}
