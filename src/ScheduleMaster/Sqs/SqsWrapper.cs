using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using ETravel.SqsService.Sqs;
using log4net;

namespace ScheduleMaster.Sqs
{
    public interface ISqsWrapper : IDisposable
    {
        IList<QueueMessage> Dequeue(int numberOfMessages);
        void Delete(QueueMessage message);
    }

    public class SqsWrapper : ISqsWrapper
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SqsWrapper));
        private bool _disposed;
        private SqsQueue _queue;

        public SqsWrapper(string sqsAccessKey, string sqsSecretKey, string sqsRegion, string sqsQueueUrl,
                          int longPollingTimeSeconds, int numberOfDequeueMessages)
        {
            if (string.IsNullOrEmpty(sqsAccessKey))
            {
                throw new ArgumentException("AccessKey is null or empty", "sqsAccessKey");
            }

            if (string.IsNullOrEmpty(sqsSecretKey))
            {
                throw new ArgumentException("SecretKey is null or empty", "sqsSecretKey");
            }

            if (string.IsNullOrEmpty(sqsRegion))
            {
                throw new ArgumentException("SqsRegion is null or empty", "sqsRegion");
            }

            if (string.IsNullOrEmpty(sqsQueueUrl))
            {
                throw new ArgumentException("SqsQueueUrl is null or empty", "sqsQueueUrl");
            }

            if (longPollingTimeSeconds <= 0)
            {
                throw new ArgumentException("Long polling time is invalid", "longPollingTimeSeconds");
            }

            if (numberOfDequeueMessages <= 0)
            {
                throw new ArgumentException("Number of dequeue messages is invalid", "numberOfDequeueMessages");
            }

            _queue = new SqsQueue(sqsAccessKey, sqsSecretKey, sqsRegion, sqsQueueUrl, longPollingTimeSeconds,
                numberOfDequeueMessages);
            _queue.Configure();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IList<QueueMessage> Dequeue(int numberOfMessages)
        {
            Contract.Requires(numberOfMessages > 0);

            if (numberOfMessages <= 0)
            {
                throw new ArgumentException("Number of messages should be a positive number", "numberOfMessages");
            }

            var stopwatch = Stopwatch.StartNew();
            var messages = _queue.Pop(numberOfMessages);
            stopwatch.Stop();
            Logger.InfoFormat("Got {0} requests. TimeTaken={1}ms", messages.Count, stopwatch.ElapsedMilliseconds);
            return messages;
        }

        public void Delete(QueueMessage message)
        {
            _queue.Delete((message.MessageId));
        }

        ~SqsWrapper()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_queue != null)
                {
                    _queue.Dispose();
                    _queue = null;
                }
            }

            // release any unmanaged objects
            // set the object references to null
            _disposed = true;
        }
    }
}