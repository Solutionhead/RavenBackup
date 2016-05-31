using System.Linq;

namespace RavenBackup
{
    public class Program
    {
        /// <summary> 
        /// </summary>
        /// <param name="args">
        /// -export [database names]...
        ///   Will export specified databases or all if none are provided.
        /// -import (database name) [filename]
        ///   Will import specific file into specified database, or latest file if none is provided.
        /// </param>
        static void Main(string[] args)
        {
            if(args == null || !args.Any())
            {
                return;
            }

            switch(args[0].ToUpper())
            {
                case "-EXPORT":
                    var arguments = args.ToList().GetRange(1, args.Count() - 1).ToArray();
                    DatabaseHelper.ExportDatabases(arguments.Any() ? arguments : DatabaseHelper.GetDatabaseNames());
                    break;

                case "-IMPORT":
                    arguments = args.ToList().GetRange(1, args.Count() - 1).ToArray();
                    DatabaseHelper.ImportDatabase(arguments.Where((s, i) => i == 0).FirstOrDefault(), arguments.Where((s, i) => i == 1).FirstOrDefault());
                    break;
            }
        }
    }
}
