using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Raven.Abstractions.Data;
using Raven.Abstractions.Smuggler;
using Raven.Abstractions.Smuggler.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Smuggler;

namespace RavenBackup
{
    public static class DatabaseHelper
    {
        public static void ExportDatabases(params string[] databaseNames)
        {
            if(databaseNames == null || !databaseNames.Any())
            {
                throw new ArgumentException("databaseNames required.");
            }

            var timestamp = DateTime.UtcNow;
            var exceptions = new List<Exception>();
            var api = new SmugglerDatabaseApi(new SmugglerDatabaseOptions
                {
                    Incremental = false
                });
            using(var store = new DocumentStore
                {
                    Url = Configuration.RavenUrl
                })
            {
                store.Initialize();
                try
                {
                    foreach(var database in databaseNames)
                    {
                        using(var session = store.OpenSession(database))
                        {
                            RavenQueryStatistics stats;
                            var expected = session.Query<dynamic>().Statistics(out stats).Any() ? stats.TotalResults : 0;
                            var result = Export(api, database, timestamp);
                            if(result.NumberOfExportedDocuments < expected)
                            {
                                throw new Exception(string.Format("Exporting '{0}' - expected {1} documents but exported {2}.", database, expected, result.NumberOfExportedDocuments));
                            }

                            BackupCleaner.CleanupBackups(timestamp, database);
                        }
                    }
                }
                catch(Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if(exceptions.Any())
            {
                TextMessage.SendMessage(exceptions.Select(x => x.Message).Aggregate("RavenDB Backup Errors:", (c, n) => string.Format("{0}\n{1}", c, n)));
            }
            else
            {
                TextMessage.SendMessage("RavenDB Backup Success.");
            }
        }

        public static void ImportDatabase(string databaseName, string backup = null)
        {
            if(string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentNullException("databaseName is required.");
            }

            var directoryInfo = new DirectoryInfo(Path.Combine(Configuration.DestinationPath, databaseName));
            if(!directoryInfo.Exists)
            {
                throw new Exception(string.Format("Backup directory for database '{0}' does not exist.", databaseName));
            }

            var files = directoryInfo.GetFiles("*.raven", SearchOption.TopDirectoryOnly);
            var backupFile = string.IsNullOrWhiteSpace(backup) ?
                files.OrderByDescending(f => f.LastWriteTimeUtc).FirstOrDefault() :
                files.FirstOrDefault(f => f.Name == backup) ?? files.FirstOrDefault(f => f.Name == string.Format("{0}.raven", backup));
            if(backupFile != null)
            {
                var api = new SmugglerDatabaseApi(new SmugglerDatabaseOptions
                    {
                        Incremental = false
                    });
                
                var task = api.ImportData(new SmugglerImportOptions<RavenConnectionStringOptions>
                    {
                        FromFile = backupFile.FullName,
                        To = new RavenConnectionStringOptions
                            {
                                DefaultDatabase = databaseName,
                                Url = Configuration.RavenUrl
                            }
                    });
                task.Wait();
            }
        }

        private static OperationState Export(SmugglerDatabaseApi api, string databaseName, DateTime timestamp)
        {
            var directory = Directory.CreateDirectory(Path.Combine(Configuration.DestinationPath, databaseName));

            return api.ExportData(new SmugglerExportOptions<RavenConnectionStringOptions>
                {
                    ToFile = Path.Combine(directory.FullName, string.Format("{0}.raven", timestamp.ToString("yyyyMMdd HHmmss"))),
                    From = new RavenConnectionStringOptions
                        {
                            DefaultDatabase = databaseName,
                            Url = Configuration.RavenUrl
                        }
                }).Result;
        }

        public static string[] GetDatabaseNames()
        {
            using(var store = new DocumentStore
                {
                    Url = Configuration.RavenUrl
                })
            {
                store.Initialize();
                return store.DatabaseCommands.GlobalAdmin.GetDatabaseNames(int.MaxValue);
            }
        }
    }
}