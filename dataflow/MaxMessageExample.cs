using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace dataflow
{
    public class MaxMessageExample
    {
        internal void start()
        {
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            var producerBlock = new BroadcastBlock<int>(p => p);

            List<ActionBlock<int>> consumerBlocks = new List<ActionBlock<int>>();
            for (int i = 0; i < 10; i++)
            {
                ActionBlock<int> item = Consumer(i);
                consumerBlocks.Add(item);
                producerBlock.LinkTo(item, linkOptions);
            }

            for (int i = 0; i < 100; i++)
            {
                if (!producerBlock.Post(i))
                {
                    Console.WriteLine("Failed Post");
                }
            }

            producerBlock.Complete();

            Task.WaitAll(consumerBlocks.Select(p => p.Completion).ToArray());
        }
        private ActionBlock<int> Consumer(int name)
        {
            return new ActionBlock<int>(input =>
            {
                Console.Write($"{ name }");
            }, new ExecutionDataflowBlockOptions { MaxMessagesPerTask = 3 });
        }
    }
}
