using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace dataflow
{
    public class SingleProducerExample
    {
        static public void start()
        {
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            var producerBlock = producer();
            var consumer_1 = consumer("consumer 1");

            producerBlock.LinkTo(consumer_1, linkOptions);

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 500000; i++)
            {
                if (!producerBlock.SendAsync(i).Result)
                {
                    Console.WriteLine("Failed Post");
                }
            }
            producerBlock.Complete();
            consumer_1.Completion.ContinueWith(p =>
            {
                sw.Stop();
                Console.WriteLine($"done - {sw.ElapsedMilliseconds}");
            });
        }
        static TransformBlock<int, int> producer()
        {
            var block = new TransformBlock<int, int>(input => input * 2);
            return block;
        }
        static ActionBlock<int> consumer(string name)
        {
            var block = new ActionBlock<int>((timeout) => { },
            new ExecutionDataflowBlockOptions { SingleProducerConstrained = true }
                );

            return block;
        }
    }
}
