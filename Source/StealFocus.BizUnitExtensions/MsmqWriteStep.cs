namespace StealFocus.BizUnitExtensions
{
    using System;
    using System.Messaging;
    using System.Xml;

    using BizUnit;
    using BizUnit.Common;
    using BizUnit.Xaml;

    [CLSCompliant(false)]
    public class MsmqWriteStep : TestStepBase
    {
        public MsmqWriteStep()
        {
            this.TransactionType = MessageQueueTransactionType.Single;
        }

        public string Name { get; set; }

        public string QueuePath { get; set; }

        public XmlDocument MessageBodyContent { get; set; }

        public MessageQueueTransactionType TransactionType { get; set; }

        public override void Execute(Context context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.LogInfo("MSMQ Write Step is executing.");
            using (MessageQueue messageQueue = new MessageQueue(this.QueuePath))
            {
                using (Message message = new Message(this.MessageBodyContent))
                {
                    messageQueue.Send(message, this.TransactionType);
                }
            }
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForNullReference(this.MessageBodyContent, "MessageBodyContent");
            ArgumentValidation.CheckForEmptyString(this.QueuePath, "QueuePath");
        }
    }
}
