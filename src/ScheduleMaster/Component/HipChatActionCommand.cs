using ScheduleMaster.Models.Entities;
using ETravel.SqsService.Sqs;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using HipchatApiV2;
using System;
using HipchatApiV2.Enums;
using System.Threading.Tasks;

namespace ScheduleMaster.Component
{
    public class HipChatActionCommand : ActionBaseCommand, ICommand
    {
        private readonly HipchatActionConfiguration _configuration;
        private readonly QueueMessage[] _queueMessages;
        private readonly Regex _extractionRegex;
        private readonly string _queueName;

        public HipChatActionCommand(HipchatActionConfiguration configuration, string queueName,QueueMessage[] queueMessages)
        {
            _configuration = configuration;
            _queueMessages = queueMessages;
            if (!string.IsNullOrWhiteSpace(configuration.RegularExpression) && IsValidRegex(configuration.RegularExpression))
            {
                _extractionRegex = new Regex(configuration.RegularExpression);
            }
            else
            {
                _extractionRegex = null;
            }
            _queueName = queueName;
        }

        public Task<bool> ExecuteAsync()
        {
            var body = GetMessageBody();
            return SendHipchatNotificationAsync(body);
        }

        private string GetMessageBody()
        {
            var rawMessages = _queueMessages.Select(p => p.MessageData).ToArray();

            if (string.IsNullOrWhiteSpace(_configuration.RegularExpression))
            {
                return GetMessageBodyFromTemplate(rawMessages);
            }
            else
            {
                return GetMessageBodyFromTemplate(GetMessages(rawMessages));
            }
        }

        private string GetMessageBodyFromTemplate(string[] messages)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("{0} ", _configuration.Mentions);
            builder.AppendFormat("The following data was present in the queue {0}:", _queueName);
            builder.AppendLine();
            builder.AppendLine();

            foreach (var message in messages)
            {
                builder.AppendLine(message);
            }

            builder.AppendLine();
            builder.AppendLine("Please proceed on your end with handling the issue!");

            return builder.ToString();
        }

        private string[] GetMessages(string[] rawMessages)
        {
            var messages = new List<string>();

            foreach (var rawMessage in rawMessages)
            {
                if(_extractionRegex == null)
                {
                    return rawMessages;
                }

                var match = _extractionRegex.Match(rawMessage);

                // if one does not match then we should return the messages in raw format
                if (!match.Success)
                {
                    return rawMessages;
                }

                var data = GetMatchedData(match);

                if (data.Length == 0)
                {
                    return rawMessages;
                }

                messages.Add(string.Join(" ", data));
            }

            return messages.ToArray();
        }

        private string[] GetMatchedData(Match match)
        {
            var data = new List<string>();

            GroupCollection groups = match.Groups;

            foreach (string groupName in _extractionRegex.GetGroupNames().Skip(1))
            {
                data.Add(string.Format("{0}={1}", groupName, groups[groupName].Value));
            }

            return data.ToArray();
        }

        private Task<bool> SendHipchatNotificationAsync(string body)
        {
            return Task.Run(() => SendHipchatNotification(body));
        }

        private bool SendHipchatNotification(string body)
        {
            var client = new HipchatClient(_configuration.ApiKey);

            var existingRoomId = client.GetRoom(_configuration.RoomName);

            var roomColor = (RoomColors)Enum.Parse(typeof(RoomColors), _configuration.NotificationColor, true);

            var sendMessageResult = client.SendNotification(existingRoomId.Id, body, roomColor, true, HipchatMessageFormat.Text);

            if (!sendMessageResult)
            {
                throw new ApplicationException(string.Format("Could not send notification to room {0}", _configuration.RoomName));
            }

            return true;
        }
    }
}