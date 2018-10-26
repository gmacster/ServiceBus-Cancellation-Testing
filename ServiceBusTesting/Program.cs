using System;
using System.Collections.Generic;

using CommandLine;

using Microsoft.Azure.ServiceBus;

namespace ServiceBusTesting
{
    class Options
    {
        [Option('s', "ServiceBus Connection String", Required = true)]
        public string ConnectionString { get; set; }

        [Option('n', "A list of sequence numbers to try and cancel", Required = true)]
        public IEnumerable<long> SequenceNumbers { get; set; }
    }

    class Program
    {
        static  void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(
                    options =>
                        {
                            var connectionStringBuilder = new ServiceBusConnectionStringBuilder(options.ConnectionString);

                            var topicClient = new TopicClient(connectionStringBuilder);

                            foreach (var sequenceNumber in options.SequenceNumbers)
                            {
                                try
                                {
                                    topicClient.CancelScheduledMessageAsync(sequenceNumber).GetAwaiter().GetResult();
                                }
                                catch (MessageNotFoundException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                        });
        }
    }
}
