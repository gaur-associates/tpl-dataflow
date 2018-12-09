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
           //ProducerConsumer.start();
           new BatchExample().start();

           Console.ReadKey();
        }
    }
}
