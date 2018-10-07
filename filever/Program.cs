using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using McMaster.Extensions.CommandLineUtils;

namespace filever
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                await CommandLineApplication.ExecuteAsync<FileVerCommand>(args);
            }
            catch (Exception e)
            {
                if (e is AggregateException ae)
                    foreach (var exception in ae.InnerExceptions)
                        await Console.Error.WriteLineAsync(exception.Message + " " + exception.InnerException?.Message);
                else
                    await Console.Error.WriteLineAsync(e.Message + " " + e.InnerException?.Message);
            }
        }
    }

    [Command("filever", Description = "Gets the Version Info for a file.")]
    internal class FileVerCommand
    {        
        [FileExists]
        [Argument(1, "file", Description = "The file to get the version information for.")]
        public string FilePath { get; set; }

        [Option("-pv|--product-ver", "Optionally display the Product Version instead of the File Version.", CommandOptionType.NoValue)]
        public bool ProductVersion { get; set; }

        [Option("-a|--show-all", "Displays all of the Version Info properties.", CommandOptionType.NoValue)]
        public bool AllInfo { get; set; }

        [UsedImplicitly]
        public async Task OnExecuteAsync(CommandLineApplication app)
        {
            if (FilePath == null)
                app.ShowHint();
            else
            {
                var extension = Path.GetExtension(FilePath);
                if (extension != ".exe" && extension != ".dll")
                    throw new ArgumentException("The file must have the .exe or .dll extension.");

                var versionInfo = FileVersionInfo.GetVersionInfo(Path.IsPathRooted(FilePath) ? FilePath : Path.Combine(Directory.GetCurrentDirectory(), FilePath));

                if (AllInfo)
                    await Utilities.PrintObjectAsync(versionInfo);
                else
                    await Console.Out.WriteLineAsync(ProductVersion
                        ? versionInfo.ProductVersion
                        : versionInfo.FileVersion);
            }
        }
    }

    internal class Utilities
    {
        public static async Task PrintObjectAsync(object o)
        {
            if (o == null)
                return;
            foreach (var property in o.GetType().GetProperties())
            {
                var value = property.GetValue(o)?.ToString();
                await Console.Out.WriteAsync(property.Name + ": ");
                await Console.Out.WriteLineAsync(string.IsNullOrEmpty(value) ? "N/A" : value);
            }
        }
    }
}
