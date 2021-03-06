using System;
using System.Diagnostics;
using System.Threading;

using System.Threading.Tasks.Dataflow;

namespace dataflow
{
    class ProducerConsumer
    {
        static public void start()
        {
            Console.WriteLine("Hello World!");

            var producerBlock = producer();

            var consumer_1 = consumer("consumer 1");
            var consumer_2 = consumer("consumer 2");

            producerBlock.LinkTo(consumer_1);
            producerBlock.LinkTo(consumer_2);

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 10; i++)
            {
                if (!  producerBlock.SendAsync(i).Result)
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

            consumer_2.Completion.ContinueWith(p =>
            {
                sw.Stop();
                Console.WriteLine($"done - {sw.ElapsedMilliseconds}");
            });

        }

        static TransformBlock<int, int> producer()
        {
            var block = new TransformBlock<int, int>(input => input * 2
            , new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 5
            }
            );

            return block;
        }

        static  ActionBlock<int> consumer(string name)
        {
            var block = new ActionBlock<int>(
                (timeout) =>
                {
                    Thread.Sleep(timeout);
                    Console.WriteLine($"in action block {name} - {timeout}");
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 2,
                    BoundedCapacity = 2

                });

            return block;
        }
    }
}
