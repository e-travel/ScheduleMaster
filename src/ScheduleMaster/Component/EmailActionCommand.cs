using System.Net.Mail;
using ScheduleMaster.Models.Entities;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System;
using ScheduleMaster.Sqs;

namespace ScheduleMaster.Component
{
    public class EmailActionCommand : ActionBaseCommand, ICommand
    {
        private readonly EmailActionConfiguration _configuration;
        private readonly QueueMessage[] _queueMessages;
        private readonly Regex _extractionRegex;

        public EmailActionCommand(EmailActionConfiguration configuration, QueueMessage[] queueMessages)
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
        }

        public Task<bool> ExecuteAsync()
        {
            var body = GetMessageBody();
            return SendEmailAsync(body);
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

            builder.AppendLine("The following data was present in the queue:");
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

                messages.Add(string.Join("-", data));
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

        private Task<bool> SendEmailAsync(string body)
        {
            return Task.Run(() => SendEmail(body));
        }

        private bool SendEmail(string body)
        {
            var message = new MailMessage();
            message.From = new MailAddress(_configuration.From);


            foreach (var address in _configuration.To.Split(';'))
            {
                message.To.Add(address);
            }

            foreach (var address in _configuration.CC.Split(';'))
            {
                message.CC.Add(address);
            }

            message.Subject = _configuration.Subject;
            message.Body = body;

            using (var client = new SmtpClient(_configuration.SmtpHost, _configuration.SmtpPort))
            {
                client.EnableSsl = _configuration.SmtpEnableSSL;
                client.Credentials = new NetworkCredential(_configuration.SmtpUsername, _configuration.SmtpPassword);
                client.Send(message);
            }

            return true;
        }
    }
}