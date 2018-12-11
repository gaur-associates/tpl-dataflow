

using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace dataflow
{
    internal class JoinBlockExample
    {
        public void start()
        {
            var transform_double = transform(2);
            var transform_triple = transform(3);

            var producerBlock = producer();

            transform_double.LinkTo(producerBlock.Target1, new DataflowLinkOptions { PropagateCompletion = true });
            transform_triple.LinkTo(producerBlock.Target2, new DataflowLinkOptions { PropagateCompletion = true });

            var consumerBlock = consumer("gaur");
            producerBlock.LinkTo(consumerBlock, new DataflowLinkOptions { PropagateCompletion = true });

            for (int i = 0; i < 10; i++)
            {
                if (!transform_double.SendAsync(i).Result)
                {
                    Console.WriteLine($"send async failed - {i}");
                }
                if (!transform_triple.SendAsync(i).Result)
                {
                    Console.WriteLine($"send async failed - {i}");
                }
            }
            transform_double.Complete();
            transform_triple.Complete();

            consumerBlock.Completion.ContinueWith( p =>
            {
                Console.WriteLine($"faulted  - {p.IsFaulted}  completed = {p.IsCompleted}  cancel - {p.IsCanceled}");
            });

        }

        private JoinBlock<int, int> producer()
        {
            var block = new JoinBlock<int, int>();

            return block;
        }
        static TransformBlock<int, int> transform(int mult)
        {
            var block = new TransformBlock<int, int>(input =>
            {
                Console.WriteLine($"transform {mult} x {input}");
                return input * mult;
            }

            );

            return block;
        }

        static ActionBlock<Tuple<int, int>> consumer(string name)
        {
            var block = new ActionBlock<Tuple<int, int>>(
                (input) =>
                {
                    var str = String.Join(" ", input);

                    if (str == "(2, 3)")
                    {
                        // throw new Exception("simulate error");
                    }
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