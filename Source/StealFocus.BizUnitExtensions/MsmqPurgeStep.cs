namespace StealFocus.BizUnitExtensions
{
    using System;
    using System.Messaging;

    using BizUnit;
    using BizUnit.Xaml;

    [CLSCompliant(false)]
    public class MsmqPurgeStep : TestStepBase
    {
        public string Name { get; set; }

        public string QueuePath { get; set; }

        public override void Execute(Context context)
        {
            using (MessageQueue messageQueue = new MessageQueue(this.QueuePath))
            {
                messageQueue.Purge();
            }
        }

        public override void Validate(Context context)
        {
            // Do nothing
        }
    }
}
