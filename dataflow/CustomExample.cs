using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace dataflow
{
    internal class CustomExample
    {
        public static IPropagatorBlock<int, int> SumOddNumbers()
        {
            var output = new BufferBlock<int>();
            int sum = 0;

            var target = new ActionBlock<int>(async (input) =>
           {
               if (input % 2 != 0)
               {
                   sum += input;

                   await output.SendAsync(sum);
               }
           });

            target.Completion.ContinueWith(p =>
            {
                output.Complete();
            });

            return DataflowBlock.Encapsulate<int, int>(target, output);

        }

        public void start()
        {
            var consumerBlock = Consumer("consumer");

            var transformBlock = new TransformBlock<int, int>(input =>
            {
                // if (input == 1) throw new Exception("test problem");
                return input * 3;
            });

            var producerBlock = new CustomAdvance();

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

            Console.Read();
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