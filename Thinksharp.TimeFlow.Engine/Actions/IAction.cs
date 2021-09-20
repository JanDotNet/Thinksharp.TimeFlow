using System;
using System.Threading.Tasks;

namespace Thinksharp.TimeFlow.Engine.Actions
{
    public interface IAction
    {
        Guid Id { get; }

        Task ExecuteAsync(ExecutionContext context);
    }
}
