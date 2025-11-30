using Hangfire;
using Reamp.Domain.Common.Services;

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

        public string Enqueue<T>(System.Linq.Expressions.Expression<Action<T>> methodCall)
        {
            return _backgroundJobClient.Enqueue(methodCall);
        }
    }
}

