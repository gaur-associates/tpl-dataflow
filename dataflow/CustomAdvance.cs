using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace dataflow
{
    public class CustomAdvance : IPropagatorBlock<int, int>
    {
        private BufferBlock<int> _source;
        private ActionBlock<int> _target;
        private int _sum;

        public CustomAdvance()
        {
            _source = new BufferBlock<int>();
            _sum = 0;

            _target = new ActionBlock<int>(async (input) =>
           {
               if (input % 2 != 0)
               {
                   _sum += input;

                   await _source.SendAsync(_sum);
               }
           });
        }

        public Task Completion => _source.Completion;

        public void Complete()
        {
            _target.Complete();
        }

        public int ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<int> target, out bool messageConsumed)
        {
            return ((ISourceBlock<int>)_source).ConsumeMessage(messageHeader, target, out messageConsumed);
        }

        public void Fault(Exception exception)
        {
            ((ITargetBlock<int>)_target).Fault(exception);
        }

        public IDisposable LinkTo(ITargetBlock<int> target, DataflowLinkOptions linkOptions)
        {
            return ((ISourceBlock<int>)_source).LinkTo(target, linkOptions);
        }

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, int messageValue, ISourceBlock<int> source, bool consumeToAccept)
        {
            return ((ITargetBlock<int>)_target).OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<int> target)
        {
            ((ISourceBlock<int>)_source).ReleaseReservation(messageHeader, target);
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<int> target)
        {
            return ((ISourceBlock<int>)_source).ReserveMessage(messageHeader, target);
        }
    }
}


