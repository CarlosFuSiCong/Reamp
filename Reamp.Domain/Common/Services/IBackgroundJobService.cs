namespace Reamp.Domain.Common.Services
{
    // Abstraction for background job scheduling (avoids direct Hangfire dependency in Domain/Application layers)
    public interface IBackgroundJobService
    {
        // Enqueue a job to be executed in the background
        string Enqueue<T>(System.Linq.Expressions.Expression<Action<T>> methodCall);
    }
}

