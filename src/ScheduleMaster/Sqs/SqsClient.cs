using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ScheduleMaster.Sqs
{
    public interface ISqsClient : IDisposable
    {
        void Delete(string messageId);
        IList<QueueMessage> Pop(int numberOfMessages);
    }

    public class SqsClient : ISqsClient
    {
        private readonly IAmazonSQS _client;
        private readonly string _sqsQueueUrl;
        private readonly int _waitTimeSeconds;
        private readonly int _minimumNumberOfMessages;

        public SqsClient(string sqsAccessKey, string sqsSecretKey, string sqsRegion, string sqsQueueUrl, int waitTimeSeconds, int minimumNumberOfMessages)
        {
            _sqsQueueUrl = sqsQueueUrl;
            _waitTimeSeconds = waitTimeSeconds;
            _minimumNumberOfMessages = minimumNumberOfMessages;
            _client = new AmazonSQSClient(sqsAccessKey, sqsSecretKey, RegionEndpoint.GetBySystemName(sqsRegion));
        }

        public IList<QueueMessage> Pop(int numberOfMessages)
        {
            numberOfMessages = Math.Min(_minimumNumberOfMessages, numberOfMessages);

            try
            {
                var msg = _client.ReceiveMessage(new ReceiveMessageRequest
                {
                    MaxNumberOfMessages = numberOfMessages,
                    QueueUrl = _sqsQueueUrl,
                    WaitTimeSeconds = _waitTimeSeconds
                });

                if (msg != null && msg.Messages != null && msg.Messages.Count >= 1)
                {
                    var lst = new List<QueueMessage>();
                    foreach (var message in msg.Messages)
                    {
                        lst.Add(new QueueMessage { MessageData = message.Body, MessageId = message.ReceiptHandle });
                    }

                    return lst;
                }

                return new List<QueueMessage>();
            }
            catch
            {
                // Log the error, backoff for a while but do *NOT* raise an exception.
                Thread.Sleep(2500);
                return new List<QueueMessage>();
            }
        }

        public void Delete(string messageId)
        {
            _client.DeleteMessage(new DeleteMessageRequest(_sqsQueueUrl, messageId));
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(_client != null)
                    {
                        _client.Dispose();
                    }
                }

                disposedValue = true;
            }
        }
        
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}