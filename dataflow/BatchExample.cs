

using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace dataflow
{
    internal class BatchExample
    {
        public void start()
        {
            var producerBlock = producer();

            for (int i = 0; i < 10; i++)
            {
                if (!producerBlock.SendAsync(i).Result)
                {
                    Console.WriteLine($"send async failed - {i}");
                }
            }

            producerBlock.Complete();

            // for (int i = 0; i < 4; i++)
            // {
            //     var result = producerBlock.ReceiveAsync().Result;
            //     var str = String.Join(" ", result);

            //     Console.WriteLine($"received- {str}");
            // }

            var consumerBlock = consumer("gaur");
            producerBlock.LinkTo(consumerBlock);
        }

        private BatchBlock<int> producer()
        {
            var block = new BatchBlock<int>(3);

            return block;
        }
         static  ActionBlock<IEnumerable<int>> consumer(string name)
        {
            var block = new ActionBlock<IEnumerable<int>>(
                (input) =>
                {
                     var str = String.Join(" ", input);
                    Console.WriteLine($"in action block {name} - {str}");
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