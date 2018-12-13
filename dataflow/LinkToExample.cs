using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace dataflow
{
    internal class LinkToExample
    {
        internal void start()
        {
            var consumerBlock = Consumer("consumer 1");
            var consumer2Block = Consumer("consumer 2");

            var producerBlock = new BufferBlock<int>();

            producerBlock.LinkTo(consumerBlock, new DataflowLinkOptions
            {
                PropagateCompletion = true,
                MaxMessages = 4
            });

            producerBlock.LinkTo(consumer2Block, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });

            for (int i = 0; i < 10; i++)
            {
                if (!producerBlock.Post(i))
                {
                    Console.WriteLine("Failed Post");
                }
            }

            producerBlock.Complete();

            producerBlock.Completion.ContinueWith(p => print_status(p, "producer"));

            consumerBlock.Completion.ContinueWith(p => print_status(p, "consumer 1"));

            consumer2Block.Completion.ContinueWith(p => print_status(p, "consumer 2"));
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
            else
            {
                Console.WriteLine($" {name} done");
            }
        }
    }
}