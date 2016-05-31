using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.Abstractions.Smuggler;
using Raven.Abstractions.Smuggler.Data;
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

            var api = new SmugglerDatabaseApi(new SmugglerDatabaseOptions
                {
                    OperateOnTypes = ItemType.Documents | ItemType.Indexes | ItemType.Attachments | ItemType.Transformers,
                    Incremental = false
                });

            var tasks = databaseNames.Select(n => Export(api, n)).ToList();
            tasks.ForEach(t => t.Wait());
        }

        public static void ImportDatabase(string databaseName, string backup = null)
        {
            if(string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentNullException("databaseName is required.");
            }

            var directoryInfo = new DirectoryInfo(Path.Combine(Configuration.DestinationPath, databaseName));
            var files = directoryInfo.GetFiles("*.raven", SearchOption.TopDirectoryOnly);
            var backupFile = string.IsNullOrWhiteSpace(backup) ?
                files.OrderByDescending(f => f.LastWriteTimeUtc).FirstOrDefault() :
                files.FirstOrDefault(f => f.Name == backup) ?? files.FirstOrDefault(f => f.Name == string.Format("{0}.raven", backup));
            if(backupFile != null)
            {
                var api = new SmugglerDatabaseApi(new SmugglerDatabaseOptions
                    {
                        OperateOnTypes = ItemType.Documents | ItemType.Indexes | ItemType.Attachments | ItemType.Transformers,
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

        private static Task<OperationState> Export(SmugglerDatabaseApi api, string databaseName)
        {
            var path = Path.Combine(Configuration.DestinationPath, databaseName);
            Directory.CreateDirectory(path);

            return api.ExportData(new SmugglerExportOptions<RavenConnectionStringOptions>
                {
                    ToFile = Path.Combine(path, string.Format("{0}.raven", DateTime.UtcNow.ToString("yyyyMMdd HHmmss"))),
                    From = new RavenConnectionStringOptions
                        {
                            DefaultDatabase = databaseName,
                            Url = Configuration.RavenUrl
                        }
                });
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