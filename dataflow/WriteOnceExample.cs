using System;
using System.Threading.Tasks.Dataflow;

namespace dataflow
{
    public class WriteOnceExample
    {
        public void start()
        {

            var block = new WriteOnceBlock<int>(x => x);

            for (int i = 0; i < 10; i++)
            {
                if (!block.Post(i))
                {
                    Console.WriteLine($"Failed Post - {i}");
                }
            }
            for (int i = 0; i < 10; i++)
            {
                var x = block.Receive();

                Console.WriteLine($"Received - {x}");

            }
        }
    }
}
