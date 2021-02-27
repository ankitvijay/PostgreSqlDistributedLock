using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace PostgreSQLDistributedLock
{
    public class DistributedLock
    {
        private readonly ILogger<DistributedLock> _logger;
        private NpgsqlConnection _connection;

        public DistributedLock(string connectionString, ILogger<DistributedLock> logger)
        {
            _logger = logger;
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            _connection = new NpgsqlConnection(builder.ToString());
            _connection.Open();
        }

        public async Task<bool> TryExecuteInDistributedLock(long lockId, Func<Task> func)
        {
            var hasLockedAcquired = await TryAcquireLockAsync(lockId);

            if (!hasLockedAcquired)
            {
                return false;
            }

            try
            {
                await func();
            }
            finally
            {
                await UnlockAsync(lockId);
            }

            return true;
        }

        private async Task<bool> TryAcquireLockAsync(long lockId)
        {
            var sessionLockCommand = $"SELECT pg_try_advisory_lock({lockId})";
            _logger.LogInformation("Trying to acquire session lock for Lock Id {@LockId}", lockId);
            var commandQuery = new NpgsqlCommand(sessionLockCommand, _connection);
            var result = await commandQuery.ExecuteScalarAsync();
            if (result != null && bool.TryParse(result.ToString(), out var lockAcquired) && lockAcquired)
            {
                _logger.LogInformation("Lock {@LockId} acquired", lockId);
                return true;
            }

            _logger.LogInformation("Lock {@LockId} rejected", lockId);
            return false;
        }

        private async Task UnlockAsync(long lockId)
        {
            var transactionLockCommand = $"SELECT pg_advisory_unlock({lockId})";
            _logger.LogInformation("Releasing session lock for {@LockId}", lockId);
            var commandQuery = new NpgsqlCommand(transactionLockCommand, _connection);
            await commandQuery.ExecuteScalarAsync();
        }
    }
}