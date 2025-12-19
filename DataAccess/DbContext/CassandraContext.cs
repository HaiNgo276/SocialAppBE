using Cassandra;
using Microsoft.Extensions.Configuration;

namespace DataAccess.DbContext
{
    public class CassandraContext : IDisposable
    {
        private readonly Cluster _cluster;
        public ISession Session { get; }
        
        public CassandraContext(IConfiguration configuration)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isProduction = environment == "Production";
            
            if (isProduction)
            {
                // DataStax Astra Cloud configuration
                var contactPoint = Environment.GetEnvironmentVariable("Cassandra__ContactPoint")
                    ?? configuration["Cassandra:ContactPoint"];
                var clientId = Environment.GetEnvironmentVariable("Cassandra__ClientId")
                    ?? configuration["Cassandra:ClientId"];
                var clientSecret = Environment.GetEnvironmentVariable("Cassandra__ClientSecret")
                    ?? configuration["Cassandra:ClientSecret"];
                var keyspace = Environment.GetEnvironmentVariable("Cassandra__Keyspace")
                    ?? configuration["Cassandra:Keyspace"] ?? "fricon";

                if (string.IsNullOrEmpty(contactPoint) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    throw new InvalidOperationException("Cassandra:ContactPoint, ClientId and ClientSecret must be configured for production.");
                }
                    
                _cluster = Cluster.Builder()
                    .AddContactPoint(contactPoint)
                    .WithPort(29042) // Astra secure port
                    .WithCredentials(clientId, clientSecret) // Use ClientId as username, ClientSecret as password
                    .WithSSL()
                    .Build();
                    
                Session = _cluster.Connect(keyspace);
                
                // Create tables if not exist in cloud
                CreateTablesIfNotExist();
            }
            else
            {
                // Local development configuration
                _cluster = Cluster.Builder()
                    .AddContactPoint("127.0.0.1")
                    .Build();
                    
                Session = _cluster.Connect();

                // Tạo keyspace nếu chưa có (local only)
                Session.Execute(@"CREATE KEYSPACE IF NOT EXISTS fricon WITH replication = {'class': 'SimpleStrategy', 'replication_factor': 1};");
                Session.Execute("USE fricon;");
                
                CreateTablesIfNotExist();
            }
        }

        private void CreateTablesIfNotExist()
        {
            try
            {
                Session.Execute(@"
                    CREATE TABLE IF NOT EXISTS user_feed_seen (
                      user_id UUID,
                      post_id UUID,
                      feed_id UUID,
                      seen_at TIMESTAMP,
                      PRIMARY KEY ((user_id), feed_id)
                    );
                ");

                Session.Execute(@"
                    CREATE TABLE IF NOT EXISTS user_feed_unseen (
                      user_id UUID,
                      post_id UUID,
                      created_at TIMESTAMP,
                      feed_id UUID,
                      PRIMARY KEY ((user_id), created_at, feed_id)
                    ) WITH CLUSTERING ORDER BY (created_at DESC, feed_id ASC);
                ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating tables: {ex.Message}");
            }
        }

        public void Dispose()
        {
            Session?.Dispose();
            _cluster?.Dispose();
        }
    }
}
