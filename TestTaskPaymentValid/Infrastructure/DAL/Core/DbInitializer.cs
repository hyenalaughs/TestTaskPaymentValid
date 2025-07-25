using Microsoft.Data.SqlClient;

namespace TestTaskPaymentValid.Infrastructure.DAL.Core
{
    public class DbInitializer
    {
        public static async Task InitializeDatabaseAsync(IConfiguration configuration, ILogger logger)
        {
            var masterConnection = configuration.GetConnectionString("Master");
            var maxRetries = 10;
            var delay = TimeSpan.FromSeconds(5);

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    using (var conn = new SqlConnection(masterConnection))
                    {
                        await conn.OpenAsync();
                        using var cmd = conn.CreateCommand();
                        cmd.CommandText = @"
                    IF DB_ID('PaymentsDb') IS NULL
                    BEGIN
                        CREATE DATABASE PaymentsDb;
                    END";
                        await cmd.ExecuteNonQueryAsync();
                        logger.LogInformation("DbInitializer: database created or exists.");
                        return; 
                    }
                }
                catch (SqlException ex)
                {
                    logger.LogWarning($"DbInitializer: БД недоступна, попытка {i + 1} из {maxRetries}. Ошибка: {ex.Message}");
                    await Task.Delay(delay);
                }
            }

            var errorMsg = $"DbInitializer: не удалось подключиться к базе после {maxRetries} попыток.";
            logger.LogError(errorMsg);
            throw new Exception(errorMsg);
        }


        public static async Task InitializeAsync(IConfiguration configuration, 
                                            ILogger logger)
        {
            var defaultConnection = configuration.GetConnectionString("Default");

            // WARNING: Возможна ошибка и слеждует пересмотреть таблицы.
            const string createTableSql = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PaymentTransactions' AND xtype='U')
            BEGIN
                CREATE TABLE PaymentTransactions (
                    Id UNIQUEIDENTIFIER PRIMARY KEY,
                    RequestId UNIQUEIDENTIFIER NULL, 
                    Status INT NOT NULL,
                    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                    ProcessedAt DATETIME2 NULL
                )
            END
    
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PaymentRequest' AND xtype='U')
            BEGIN 
                CREATE TABLE PaymentRequest (
                    Id UNIQUEIDENTIFIER PRIMARY KEY,
                    Amount DECIMAL(18,2) NOT NULL,
                    Currency INT NOT NULL,
                    SourceAccount NVARCHAR(100) NOT NULL,
                    DestinationAccount NVARCHAR(100) NOT NULL,
                    MetadataJson NVARCHAR(200)
                )
            END

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='IdempotencyKeys' AND xtype='U')
            BEGIN
                CREATE TABLE IdempotencyKeys (
                    Id UNIQUEIDENTIFIER PRIMARY KEY,
                    TransactionId UNIQUEIDENTIFIER,
                    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
                )
            END
            ";

            try
            {
                using var conn = new SqlConnection(defaultConnection);
                await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = createTableSql;
                await cmd.ExecuteNonQueryAsync();

                logger.LogInformation("DbInitializer done;");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DbInitializer error.");
                throw;
            }
        }
    }
}
