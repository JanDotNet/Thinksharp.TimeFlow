
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Thinksharp.TimeFlow.Engine.Actions;
using Thinksharp.TimeFlow.Excel.Actions;
using static Thinksharp.TimeFlow.Engine.Test.TestActions.UnitTest1;

namespace Thinksharp.TimeFlow.Engine.Test
{
  [TestClass]
  public partial class TimeFlowEngineTest
  {
    public TestContext TestContext { get; set; }

    [TestMethod]
    public async Task TestEngine()
    {
      var engine = new TimeFlowEngine();

      var xml = File.ReadAllText($"Xml\\{TestContext.TestName}.xml");

      engine.RegisterAction("Action1", (xAction, aCtx) => new TestAction(xAction.Attribute("Att").Value));

      var ctx = await engine.ExecuteAsync(xml, new Dictionary<string, string>());

      Assert.IsTrue(ctx.Parameters.TryGetValue("Action_1", out var att1));
      Assert.AreEqual("01", att1);

      Assert.IsTrue(ctx.Parameters.TryGetValue("Action_2", out var att2));
      Assert.AreEqual("02", att2);
    }

    [TestMethod]
    public async Task TestTimeSeriesFromValue()
    {
      var engine = new TimeFlowEngine();

      var xml = File.ReadAllText($"Xml\\{TestContext.TestName}.xml");

      engine.RegisterAction<TimeSeriesFromValue>();

      var ctx = await engine.ExecuteAsync(xml, new Dictionary<string, string>());

      var ts = TimeSeries.Factory.FromValue(3, new DateTime(2021, 01, 01), new DateTime(2021, 12, 31), Period.Day);


      Assert.IsTrue(ts == ctx.Frame["TS1"]);
    }

    [TestMethod]
    public async Task TestCalculated()
    {
      var engine = new TimeFlowEngine();

      var xml = File.ReadAllText($"Xml\\{TestContext.TestName}.xml");

      engine.RegisterAction<TimeSeriesFromValue>();
      engine.RegisterAction<Calculate>();

      var ctx = await engine.ExecuteAsync(xml, new Dictionary<string, string>());

      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), new DateTime(2021, 12, 31), Period.Day);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2021, 01, 01), new DateTime(2021, 12, 31), Period.Day);
      var ts3 = ts1 + ts2;


      Assert.IsTrue(ts1 == ctx.Frame["TS1"]);
      Assert.IsTrue(ts2 == ctx.Frame["TS2"]);
      Assert.IsTrue(ts3 == ctx.Frame["TS3"]);
    }

    [TestMethod]
    public async Task TestExcelExport()
    {
      var engine = new TimeFlowEngine();

      var xml = File.ReadAllText($"Xml\\{TestContext.TestName}.xml");

      engine.RegisterAction<TimeSeriesFromValue>();
      engine.RegisterAction<Calculate>();
      engine.RegisterAction<WriteToExcel>();

      var ctx = await engine.ExecuteAsync(xml, new Dictionary<string, string>());
    }
  }
}
