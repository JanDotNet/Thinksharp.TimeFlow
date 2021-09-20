using System;
using System.Threading.Tasks;
using Thinksharp.TimeFlow.Engine.Actions;

namespace Thinksharp.TimeFlow.Engine.Test.TestActions
{
    public partial class UnitTest1
    {
        public class TestAction : IAction
        {
            private readonly string att;
            private static int instanceCounter = 1;
            public TestAction(string att)
            {
                this.att = att;
            }

            public string Id { get; } = Guid.NewGuid().ToString();

            Guid IAction.Id => throw new NotImplementedException();

            public Task ExecuteAsync(ExecutionContext context)
            {
                context.Parameters[$"Action_{instanceCounter++}"] = this.att;

                return Task.CompletedTask;
            }
        }

    }
}
