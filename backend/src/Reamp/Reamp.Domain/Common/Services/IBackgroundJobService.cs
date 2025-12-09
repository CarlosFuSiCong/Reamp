using System.Linq.Expressions;

namespace Reamp.Domain.Common.Services
{
    // Abstraction for background job scheduling (avoids direct Hangfire dependency in Domain/Application layers)
    public interface IBackgroundJobService
    {
        // Enqueue a synchronous job to be executed in the background
        string Enqueue<T>(Expression<Action<T>> methodCall);
        
        // Enqueue an asynchronous job to be executed in the background
        string Enqueue<T>(Expression<Func<T, Task>> methodCall);
    }
}

