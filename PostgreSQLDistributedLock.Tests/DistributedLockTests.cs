using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace PostgreSQLDistributedLock.Tests
{
    public class DistributedLockTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly string _connectionString;

        public DistributedLockTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _connectionString = $"Host=localhost;Username=dbUser;Password='password';" +
                                $"Database=distributed-lock-db;";
        }

        [Fact]
        public async Task DistributedLockIsAcquiredSuccessfully()
        {
            using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
            var logger = _testOutputHelper.BuildLoggerFor<DistributedLockTests>();
            var distributedLock = new DistributedLock(_connectionString, _testOutputHelper.BuildLoggerFor<DistributedLock>());
            Func<Task> anExclusiveLockTask = async () =>
            {
                logger.LogInformation("Executing a long running task... ");
                await Task.Delay(50);
            };
            const long lockId = 50000;

            await distributedLock.TryExecuteInDistributedLock(lockId, anExclusiveLockTask);
        }
    }
}