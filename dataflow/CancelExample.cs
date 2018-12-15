using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace dataflow
{
    internal class CancelExample
    {
        public void start()
        {
            var consoleTask = Task.Run(() =>
             {
                 while (true)
                 {
                     var input = Console.ReadKey().KeyChar;

                     if (input == 'c')
                     {
                         Console.WriteLine("Cancel request");
                     }
                     else if (input == 'o')
                     {
                         break;
                     }
                 }
             });

            var producerBlock = producer();

            for (int i = 0; i < 10; i++)
            {
                if (!producerBlock.Post(i))
                {
                    Console.WriteLine($"Post failed - {i}");
                }
            }

            var consumerBlock = consumer("gaur");
            producerBlock.LinkTo(consumerBlock, new DataflowLinkOptions { PropagateCompletion = true });

            producerBlock.Complete();

            consumerBlock.Completion.ContinueWith(p => print_status(p, "consumer"));

            consoleTask.Wait();
        }
        private static void print_status(Task p, string name)
        {
            if (p.IsFaulted)
            {
                Console.WriteLine($" {name} faulted - {p.Exception.Flatten().InnerExceptions.Aggregate("", (s, exception) => s + " " + exception.Message)}");
            }
            else
            {
                Console.WriteLine($" {name} done");
            }
        }

        private BufferBlock<int> producer()
        {
            var block = new BufferBlock<int>();

            return block;
        }
        static ActionBlock<int> consumer(string name)
        {
            var block = new ActionBlock<int>(
                (input) =>
                {
                    Console.WriteLine($"in action block {name} - {input}");
                    Thread.Sleep(1000);
                });

            return block;
        }
    }
}