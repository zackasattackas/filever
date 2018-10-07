using System;
using System.Threading.Tasks;
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
}
