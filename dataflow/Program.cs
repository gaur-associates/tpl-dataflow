using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace dataflow
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var block = new ActionBlock<int>((timeout) =>
            {
                Thread.Sleep(timeout);
                Console.WriteLine("in action block");
            }, new ExecutionDataflowBlockOptions {
                MaxDegreeOfParallelism = 2
            });
            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 10; i++)
            {
                if (block.Post(1000))
                {
                    Console.WriteLine("Succesful post");
                }
                else
                {
                    Console.WriteLine("Failed Post");
                }
            }
            block.Complete();
            block.Completion.ContinueWith(p =>
            {
                sw.Stop();
                Console.WriteLine($"done - {sw.ElapsedMilliseconds}");
            });
            Console.ReadKey();
        }
    }
}
