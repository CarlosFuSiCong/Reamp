namespace Reamp.Application.Common.Services
{
    // Abstraction for background job scheduling (avoids direct Hangfire dependency in Application layer)
    public interface IBackgroundJobService
    {
        // Enqueue a job to be executed in the background
        string Enqueue<T>(System.Linq.Expressions.Expression<Action<T>> methodCall);

        // Enqueue a job with a specific ID (for idempotency)
        string Enqueue<T>(System.Linq.Expressions.Expression<Action<T>> methodCall, string jobId);
    }
}



