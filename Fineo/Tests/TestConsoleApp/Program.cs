using System;
using System.Linq;
using System.Threading.Tasks;

namespace TestConsoleApp
{
    
    public sealed class Circle
    {
        private double radius;

        public double Calculate(Func<double, double> op)
        {
            return op(radius);
        }
    }

    class Program
    {
        private static string result;

        static void Main()
        {
            SaySomething();
            Console.WriteLine(result);
        }

        static async Task<string> SaySomething()
        {
            await Task.Delay(5);
            result = "Hello world!";
            return "Something";
        }
    }
}
