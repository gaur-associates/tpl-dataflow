using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace dataflow
{
    public class Counter
    {
        public int count;
    }
    public class SchedulerExample
    {
        private ConcurrentExclusiveSchedulerPair scheduler;

        internal void start()
        {
            scheduler = new ConcurrentExclusiveSchedulerPair();

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            var consumerBlock = Consumer("consumer 1");
            var consumer2Block = Consumer("\t\tconsumer 2");

            var producerBlock = new BroadcastBlock<Counter>(p => p);

            producerBlock.LinkTo(consumerBlock, linkOptions);

            producerBlock.LinkTo(consumer2Block, linkOptions);

            var cnt = new Counter();

            if (!producerBlock.Post(cnt))
            {
                Console.WriteLine("Failed Post");
            }

            producerBlock.Complete();

            producerBlock.Completion.ContinueWith(p => print_status(p, "producer"));

            var t1 = consumerBlock.Completion.ContinueWith(p => print_status(p, "consumer 1"));

            var t2 = consumer2Block.Completion.ContinueWith(p => print_status(p, "\t\tconsumer 2"));

            Task.WaitAll(t1, t2);
            Console.WriteLine("result ->" + cnt.count);
        }
        static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        private ActionBlock<Counter> Consumer(string name)
        {
            return new ActionBlock<Counter>(input =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(RandomNumber(1, 2));
                    input.count++;
                }
                Console.WriteLine($"{name} -- {input.count} ");
            }, new ExecutionDataflowBlockOptions { TaskScheduler = scheduler.ExclusiveScheduler });
        }
        private void print_status(Task p, string name)
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
