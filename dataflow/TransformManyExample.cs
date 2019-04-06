using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace dataflow
{
    public class TransformManyExample
    {
        internal void start()
        {
            var consumerBlock = Consumer("consumer 1");
            var consumer2Block = Consumer("\t\tconsumer 2");

            var producerBlock = new TransformManyBlock<int, int>(x => Enumerable.Range(0, x));

            producerBlock.LinkTo(consumerBlock, new DataflowLinkOptions
            {
                PropagateCompletion = true,
            });

            producerBlock.LinkTo(consumer2Block, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });


            if (!producerBlock.Post(10))
            {
                Console.WriteLine("Failed Post");
            }


            producerBlock.Complete();

            producerBlock.Completion.ContinueWith(p => print_status(p, "producer"));

            consumerBlock.Completion.ContinueWith(p => print_status(p, "consumer 1"));

            consumer2Block.Completion.ContinueWith(p => print_status(p, "\t\tconsumer 2"));
        }

        private static ActionBlock<int> Consumer(string name)
        {
            return new ActionBlock<int>(input =>
            {
                Console.WriteLine($"{name} -- {input} ");
                Thread.Sleep(input);
            }, new ExecutionDataflowBlockOptions{
                BoundedCapacity = 3
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
