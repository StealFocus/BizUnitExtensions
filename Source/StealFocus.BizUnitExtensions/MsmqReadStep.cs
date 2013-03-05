namespace StealFocus.BizUnitExtensions
{
    using System;
    using System.Messaging;
    
    using BizUnit;
    using BizUnit.Common;
    using BizUnit.Xaml;

    [CLSCompliant(false)]
    public class MsmqReadStep : TestStepBase
    {
        public MsmqReadStep()
        {
            this.TransactionType = MessageQueueTransactionType.Single;
        }

        public string Name { get; set; }

        public string QueuePath { get; set; }

        public int TimeoutInMilliseconds { get; set; }

        public MessageQueueTransactionType TransactionType { get; set; }

        public override void Execute(Context context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.LogInfo("MSMQ Read Step is executing.");
            
            // XmlDocument messageBody = new XmlDocument();
            using (MessageQueue messageQueue = new MessageQueue(this.QueuePath))
            {
                TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, this.TimeoutInMilliseconds);
                Message message = messageQueue.Receive(timeSpan, this.TransactionType);
                if (message == null)
                {
                    throw new BizUnitExtensionsException("No message was received from the message queue.");
                }

                // messageBody.LoadXml(message.Body.ToString());
                foreach (SubStepBase subStep in this.SubSteps)
                {
                    subStep.Execute(message.BodyStream, context);
                }
            }

            /*
            using (MemoryStream memoryStream = new MemoryStream())
            {
                messageBody.Save(memoryStream);
                foreach (SubStepBase subStep in this.SubSteps)
                {
                    subStep.Execute(memoryStream, context);
                }
            }
            */
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(this.QueuePath, "QueuePath");
        }
    }
}
