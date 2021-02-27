using System;
using System.Collections.Generic;
using System.Linq;
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
        public void DistributedLockIsAcquiredSuccessfully()
        {
            // Arrange
            using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
            var logger = _testOutputHelper.BuildLoggerFor<DistributedLockTests>();

            async Task AnExclusiveLockTask(int node)
            {
                logger.LogInformation("Executing a long running task for Node {Node}", node);
                // Add 5 second delay
                await Task.Delay(5000);
            }

            const long lockId = 50000;

            // Simulate with 5 nodes
            var nodes = Enumerable.Range(1, 5).ToList();
            Parallel.ForEach(nodes, async node =>
            {
                // Act and Arrange
                logger.LogInformation("Executing Note {Node}", node);
                var distributedLock = new DistributedLock(_connectionString, _testOutputHelper.BuildLoggerFor<DistributedLock>());
                await distributedLock.TryExecuteInDistributedLock(lockId, () => AnExclusiveLockTask(node));
            });
        }
    }
}