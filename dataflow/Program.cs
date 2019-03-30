using System;
using System.Diagnostics;
using System.Threading;

using System.Threading.Tasks.Dataflow;

namespace dataflow
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("hello tpl");
            new WriteOnceExample().start();
            // ProducerConsumer.start();
            //new BatchExample().start();
            // new JoinBlockExample().start();
            // new LinkToExample().start();
            // new CancelExample().start();
            //new CustomExample().start();

        }
    }
}
