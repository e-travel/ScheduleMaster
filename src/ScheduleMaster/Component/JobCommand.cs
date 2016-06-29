using ScheduleMaster.DataAccess;
using ScheduleMaster.Models.Entities;
using ScheduleMaster.Sqs;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleMaster.Component
{
    public class JobCommand
    {
        public bool Execute(int jobId)
        {
            var jobConfiguration = new ScheduleMasterContext().JobConfigurations.Include(c => c.ActionConfigurations).SingleOrDefault(p=>p.Id == jobId);

            using (var sqsClient = CreateSqsClient(jobConfiguration))
            {
                var messages = sqsClient.Pop(jobConfiguration.NumberOfDequeueMessages);

                if(messages.Count == 0)
                {
                    return false;
                }

                var commands = GetCommands(jobConfiguration, messages.ToArray());
                
                var tasks = new List<Task<bool>>();

                foreach (var command in commands)
                {
                    tasks.Add(command.ExecuteAsync());
                }

                Task.WaitAll(tasks.ToArray());

                if(tasks.Any(p=>p.Result == false))
                {
                    return false;
                }

                if (jobConfiguration.DeleteMessageAfterSuccess)
                {
                    DeleteMessagesFromQueue(sqsClient, messages);
                }
            }

            return true;
        }

        private ISqsClient CreateSqsClient(JobConfiguration configuration)
        {
            return new SqsClient(configuration.SqsAccessKey, configuration.SqsSecretKey, configuration.SqsRegion, 
                                  configuration.SqsQueueUrl, configuration.LongPollingTimeSeconds, 
                                  configuration.NumberOfDequeueMessages);
        }

        private ICommand[] GetCommands(JobConfiguration jobConfiguration, QueueMessage[] messages)
        {
            var commands = new List<ICommand>();

            foreach (var configuration in jobConfiguration.ActionConfigurations.Where(p=>p.Active))
            {
                var emailConfiguration = configuration as EmailActionConfiguration;

                if (emailConfiguration != null)
                {
                    commands.Add(new EmailActionCommand(emailConfiguration, messages));
                }

                var hipchatConfiguration = configuration as HipchatActionConfiguration;

                if (hipchatConfiguration != null)
                {
                    commands.Add(new HipChatActionCommand(hipchatConfiguration, 
                                                          jobConfiguration.SqsQueueUrl.Substring(jobConfiguration.SqsQueueUrl.LastIndexOf('/') + 1), 
                                                          messages));
                }
            }
            
            return commands.ToArray();
        }

        private void DeleteMessagesFromQueue(ISqsClient client, IList<QueueMessage> messages)
        {
            try
            {
                foreach (var message in messages)
                {
                    client.Delete(message.MessageId);
                }
            }
            catch(Exception)
            {
                // ugly, don't judge!!!
            }
        }
    }
}