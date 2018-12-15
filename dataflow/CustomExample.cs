using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace dataflow
{
    internal class CustomExample
    {
       public static BufferBlock<int> SumOddNumbers()
        {
            var block = new BufferBlock<int>();
            return block;
        }

        public void start()
        {
            var consumerBlock = Consumer("consumer");

            var transformBlock = new TransformBlock<int, int>(input =>
            {
                // if (input == 1) throw new Exception("test problem");
                return input * 3;
            });

            var producerBlock = SumOddNumbers();

            transformBlock.LinkTo(producerBlock, new DataflowLinkOptions() { PropagateCompletion = true });

            producerBlock.LinkTo(consumerBlock, new DataflowLinkOptions() { PropagateCompletion = true });

            for (int i = 0; i < 10; i++)
            {
                if (!transformBlock.Post(i))
                {
                    Console.WriteLine("Failed Post");
                }
            }

            transformBlock.Complete();

            transformBlock.Completion.ContinueWith(p => print_status(p, "transformer"));

            consumerBlock.Completion.ContinueWith(p => print_status(p, "consumer"));

            Console.ReadKey();
        }
        private static ActionBlock<int> Consumer(string name)
        {
            return new ActionBlock<int>(input =>
            {
                Console.WriteLine($"in {name} -- {input} ");
                Thread.Sleep(input);
            });
        }

        private static void print_status(Task p, string name)
        {
            if (p.IsFaulted)
            {
                Console.WriteLine($" {name} faulted - {p.Exception.Flatten().InnerExceptions.Aggregate("", (s, exception) => s + " " + exception.Message)}");
            }
            else if (p.IsCanceled)
            {
                Console.WriteLine($"{name} - canceled");
            }
            else
            {
                Console.WriteLine($" {name} done");
            }
        }
    }
}