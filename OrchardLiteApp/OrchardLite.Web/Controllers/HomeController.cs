using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using OrchardLite.Web.Models;

namespace OrchardLite.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString()
        {
            var connStr = _configuration.GetConnectionString("OrchardLiteDB");
            
            // Replace environment variables
            connStr = connStr
                .Replace("${DB_HOST}", Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost")
                .Replace("${DB_PORT}", Environment.GetEnvironmentVariable("DB_PORT") ?? "3306")
                .Replace("${DB_NAME}", Environment.GetEnvironmentVariable("DB_NAME") ?? "OrchardLiteDB")
                .Replace("${DB_USER}", Environment.GetEnvironmentVariable("DB_USER") ?? "root")
                .Replace("${DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "password");
            
            return connStr;
        }

        public IActionResult Index()
        {
            try
            {
                var contentItems = new List<ContentItem>();
                int totalRecords = 0;

                using (var connection = new MySqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    // Get total count
                    using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM ContentItems", connection))
                    {
                        totalRecords = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Get recent posts
                    using (var cmd = new MySqlCommand("SELECT * FROM ContentItems ORDER BY PublishedDate DESC LIMIT 20", connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contentItems.Add(new ContentItem
                            {
                                Id = reader.GetInt32("Id"),
                                Title = reader.GetString("Title"),
                                Summary = reader.IsDBNull(reader.GetOrdinal("Summary")) ? "" : reader.GetString("Summary"),
                                Body = reader.IsDBNull(reader.GetOrdinal("Body")) ? "" : reader.GetString("Body"),
                                ContentType = reader.GetString("ContentType"),
                                AuthorId = reader.GetInt32("AuthorId"),
                                PublishedDate = reader.GetDateTime("PublishedDate"),
                                ViewCount = reader.GetInt32("ViewCount"),
                                IsPublished = reader.GetBoolean("IsPublished"),
                                CreatedDate = reader.GetDateTime("CreatedDate")
                            });
                        }
                    }
                }

                ViewBag.TotalRecords = totalRecords;
                ViewBag.DbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
                ViewBag.DbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
                
                // Dynamically detect .NET version
                ViewBag.DotNetVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
                
                // Dynamically detect database type from connection string
                var dbHost = ViewBag.DbHost.ToString().ToLower();
                if (dbHost.Contains("aurora") || dbHost.Contains("cluster"))
                {
                    ViewBag.DatabaseType = "Aurora Serverless v2";
                }
                else if (dbHost.Contains("rds"))
                {
                    ViewBag.DatabaseType = "RDS MySQL 8.0";
                }
                else
                {
                    ViewBag.DatabaseType = "MySQL Database";
                }

                return View(contentItems);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.DbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
                return View("Error");
            }
        }

        public IActionResult AllContent()
        {
            try
            {
                var contentItems = new List<ContentItem>();

                using (var connection = new MySqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    using (var cmd = new MySqlCommand("SELECT * FROM ContentItems ORDER BY PublishedDate DESC", connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contentItems.Add(new ContentItem
                            {
                                Id = reader.GetInt32("Id"),
                                Title = reader.GetString("Title"),
                                Summary = reader.IsDBNull(reader.GetOrdinal("Summary")) ? "" : reader.GetString("Summary"),
                                Body = reader.IsDBNull(reader.GetOrdinal("Body")) ? "" : reader.GetString("Body"),
                                ContentType = reader.GetString("ContentType"),
                                AuthorId = reader.GetInt32("AuthorId"),
                                PublishedDate = reader.GetDateTime("PublishedDate"),
                                ViewCount = reader.GetInt32("ViewCount"),
                                IsPublished = reader.GetBoolean("IsPublished"),
                                CreatedDate = reader.GetDateTime("CreatedDate")
                            });
                        }
                    }
                }

                ViewBag.TotalRecords = contentItems.Count;
                return View(contentItems);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        public IActionResult Health()
        {
            // Dynamically detect environment
            var dbHost = (Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost").ToLower();
            var dotnetVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            var isTransformed = dotnetVersion.Contains(".NET 8") || dotnetVersion.Contains(".NET Core 8");
            var isAurora = dbHost.Contains("aurora") || dbHost.Contains("cluster");
            
            var health = new
            {
                status = "OK",
                phase = isTransformed ? "Phase 2 - Transformed State" : "Phase 1 - Current State",
                dotnetVersion = dotnetVersion,
                deploymentType = "CloudFormation Automated",
                databaseType = isAurora ? "Aurora Serverless v2" : "RDS MySQL 8.0",
                databaseHost = dbHost,
                platform = "ECS Fargate",
                timestamp = DateTime.UtcNow.ToString("o")
            };

            return Json(health);
        }
    }
}
