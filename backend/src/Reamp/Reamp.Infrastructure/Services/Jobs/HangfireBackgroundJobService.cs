using Hangfire;
using Reamp.Domain.Common.Services;
using System.Linq.Expressions;

namespace Reamp.Infrastructure.Services.Jobs
{
    // Hangfire implementation of IBackgroundJobService
    public sealed class HangfireBackgroundJobService : IBackgroundJobService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public HangfireBackgroundJobService(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        // Enqueue synchronous job
        public string Enqueue<T>(Expression<Action<T>> methodCall)
        {
            return _backgroundJobClient.Enqueue(methodCall);
        }

        // Enqueue asynchronous job (Hangfire supports Task-returning methods)
        public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
        {
            return _backgroundJobClient.Enqueue(methodCall);
        }
    }
}

