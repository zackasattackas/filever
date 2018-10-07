using System;
using System.Threading.Tasks;

namespace filever
{
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