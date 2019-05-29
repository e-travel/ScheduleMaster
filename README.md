# Schedule Master

## A simple job scheduler based on the excellent library [hangfire.io](http://hangfire.io)

This implementation allows the user to configure via UI jobs that will run on a schedule using [hangfire.io](http://hangfire.io)

### The following jobs can be scheduled

- Monitoring a Amazon SQS Queue for messages by providing all necessary information like
    - Name, a user-friendly name
    - Description, a job description
    - SqsAccessKey, the sqs access key
    - SqsSecretKey, the sqs secret key
    - SqsRegion, the sqs region
    - SqsQueueUrl, the sqs queue url
    - LongPollingTimeSeconds, the long-polling timeout in seconds
    - NumberOfDequeueMessages, the maximum number of messages to get per polling
    - CronExpression, a [cron expression](https://en.wikipedia.org/wiki/Cron) describing the schedule
    - DeleteMessageAfterSuccess, if the message should be deleted after successful processing
    - IsEnabled, if the job is scheduled to run


### The following action can be taken for each message

- Email Action, defines the parameters for sending a email containing one or more messages
  - Name, a user-friendly name
  - RegularExpression, a regular expression which extracts the message for the body. Check section below. If ommited raw messages are sent.
  - Active, if the action should be run or not
  - From, the email from
  - To, the receipients of the email (separated by ``;`` if multiple)
  - CC, the cc receipients of the email (separated by ``;`` if multiple)
  - Subject, the subject of the email
  - SmtpHost, the SMTP hostname
  - SmtpPort, the SMTP port
  - SmtpUsername, the SMTP username
  - SmtpPassword, the SMTP password
  - SmtpEnableSSL, SMTP SSL enabled or not

- Hipchat Action, defines the parameters for sending a email containing one or more messages

  - Name, a user-friendly name
  - RegularExpression, a regular expression which extracts the message for the body. Check section below. If ommited raw messages are sent.
  - Active, if the action should be run or not
  - ApiKey, the HipChat API key
  - Mentions, the mentions of the message (separated by space eg ``@all @user1``)
  - NotificationColor, the color of the notification (``Yellow``,``Green``,``Purple``,``Gray``,``Red``)
  - RoomName, the name of the room

## Message extraction with regular expressions

[Regular expressions](https://en.wikipedia.org/wiki/Regular_expression) are a easy way to extract data from a message using named groups like ``(?<firstname>\w+)``. If the expression matches i will return all
matched groups with name and value ``firstname=John``. If it don't matches it will return the raw message.

## Development

The solution is developed in Visual Studio 2015. As a backend you can use Sql Server 2016 Express.

## Contributing

Bug reports and pull requests are welcome on GitHub at
https://github.com/e-travel/ScheduleMaster. This project is intended
to be a safe, welcoming space for collaboration, and contributors are
expected to adhere to the
[Contributor Covenant](http://contributor-covenant.org) code of conduct.


## License

The application is available as open source under the terms of the
[MIT License](http://opensource.org/licenses/MIT).

## Thanks to the innovation team

- George Gkogkolis
- Nikos Sideris
- Sotirios Mantziaris
