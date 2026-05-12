using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson_6_streaming
{
    public class YieldExample
    {
        public IEnumerable<int> GetNumbers()
        {
            Console.WriteLine("from method start");
            yield return 1;
            Console.WriteLine("from method after 1");
            yield return 2;
            Console.WriteLine("from method after 2");
            yield return 3;
            Console.WriteLine("from method after 3");
            yield return 4;
        }

        public void RunGetNumbers()
        {
            foreach (var num in GetNumbers())
            {
                Console.WriteLine($"from main: {num}");
            }

        }


        public async IAsyncEnumerable<int> GetNumbersAsync()
        {
            yield return 1;
            await Task.Delay(3000);

            yield return 2;
            await Task.Delay(3000);

            yield return 3;
        }

        public async Task RunGetNumbersAsync()
        {
            await foreach (var num in GetNumbersAsync())
            {
                Console.WriteLine(num);
            }
        }

        public static async Task Main()
        {
            var example = new YieldExample();

            //Console.WriteLine("Synchronous yield:");
            //example.RunGetNumbers();

            Console.WriteLine();
            Console.WriteLine("Asynchronous yield:");
            await example.RunGetNumbersAsync();
            Console.WriteLine("End");
        }
    }

}
