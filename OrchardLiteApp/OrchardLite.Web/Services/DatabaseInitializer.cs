using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace OrchardLite.Web.Services
{
    public class DatabaseInitializer
    {
        private readonly IConfiguration _configuration;

        public DatabaseInitializer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString(bool includeDatabase = true)
        {
            var connStr = _configuration.GetConnectionString("OrchardLiteDB");
            
            connStr = connStr
                .Replace("${DB_HOST}", Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost")
                .Replace("${DB_PORT}", Environment.GetEnvironmentVariable("DB_PORT") ?? "3306")
                .Replace("${DB_NAME}", includeDatabase ? (Environment.GetEnvironmentVariable("DB_NAME") ?? "OrchardLiteDB") : "")
                .Replace("${DB_USER}", Environment.GetEnvironmentVariable("DB_USER") ?? "root")
                .Replace("${DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "password");
            
            if (!includeDatabase)
            {
                connStr = connStr.Replace("Database=;", "");
            }
            
            return connStr;
        }

        public void Initialize()
        {
            try
            {
                Console.WriteLine("Initializing database...");
                
                // First, connect without database to create it
                using (var connection = new MySqlConnection(GetConnectionString(false)))
                {
                    connection.Open();
                    var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "OrchardLiteDB";
                    using (var cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS `{dbName}`", connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                
                // Then connect to the database to create tables and seed data
                using (var connection = new MySqlConnection(GetConnectionString(true)))
                {
                    connection.Open();

                    // Create table
                    var createTableSql = @"
                        CREATE TABLE IF NOT EXISTS ContentItems (
                            Id INT AUTO_INCREMENT PRIMARY KEY,
                            Title VARCHAR(255) NOT NULL,
                            Summary TEXT,
                            Body LONGTEXT,
                            ContentType VARCHAR(50) NOT NULL DEFAULT 'BlogPost',
                            AuthorId INT,
                            PublishedDate DATETIME NOT NULL,
                            ViewCount INT DEFAULT 0,
                            IsPublished BOOLEAN DEFAULT TRUE,
                            CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                            INDEX idx_published_date (PublishedDate),
                            INDEX idx_content_type (ContentType)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci";

                    using (var cmd = new MySqlCommand(createTableSql, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Check if data exists
                    using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM ContentItems", connection))
                    {
                        var count = Convert.ToInt32(cmd.ExecuteScalar());
                        
                        if (count == 0)
                        {
                            Console.WriteLine("Seeding database with sample data...");
                            SeedData(connection);
                        }
                    }
                }

                Console.WriteLine("Database initialization complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex.Message}");
                throw;
            }
        }

        private void SeedData(MySqlConnection connection)
        {
            var topics = new[] { "AWS Migration", "Database Migration", "Cloud Architecture", "DevOps Practices", 
                                "Containerization", "Microservices", "Security Best Practices", "Cost Optimization" };
            var contentTypes = new[] { "BlogPost", "Page", "Article", "Tutorial" };
            var random = new Random();

            var insertSql = @"INSERT INTO ContentItems (Title, Summary, Body, ContentType, AuthorId, PublishedDate, ViewCount) 
                             VALUES (@Title, @Summary, @Body, @ContentType, @AuthorId, @PublishedDate, @ViewCount)";

            for (int i = 1; i <= 100; i++)
            {
                var topic = topics[random.Next(topics.Length)];
                var contentType = contentTypes[random.Next(contentTypes.Length)];
                var viewCount = random.Next(1, 200);
                var authorId = random.Next(1, 4);
                
                // Random date between 2017-2019
                var startDate = new DateTime(2017, 1, 1);
                var range = (new DateTime(2019, 12, 31) - startDate).Days;
                var publishDate = startDate.AddDays(random.Next(range));

                var title = $"{topic} Guide {i}";
                var summary = $"Comprehensive guide covering {topic.ToLower()} best practices and implementation strategies for enterprise applications.";
                var body = $"<h1>{title}</h1><p>This {contentType.ToLower()} provides detailed insights into {topic.ToLower()} for modern cloud architectures.</p><h2>Key Benefits</h2><ul><li>Improved scalability and performance</li><li>Enhanced security and compliance</li><li>Cost-effective solutions</li><li>Streamlined operations</li></ul>";

                using (var cmd = new MySqlCommand(insertSql, connection))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Summary", summary);
                    cmd.Parameters.AddWithValue("@Body", body);
                    cmd.Parameters.AddWithValue("@ContentType", contentType);
                    cmd.Parameters.AddWithValue("@AuthorId", authorId);
                    cmd.Parameters.AddWithValue("@PublishedDate", publishDate);
                    cmd.Parameters.AddWithValue("@ViewCount", viewCount);
                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Sample data seeded successfully (100 records)");
        }
    }
}
