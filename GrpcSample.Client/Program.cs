using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Utils;
using GrpcSample.Shared;

namespace GrpcSample.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            var channel = new Channel("127.0.0.1", 5000, ChannelCredentials.Insecure);
            var invoker = new DefaultCallInvoker(channel);
            using (var call = invoker.AsyncDuplexStreamingCall(Descriptors.Method, null, new CallOptions{}))
            {
                var responseCompleted = call.ResponseStream
                    .ForEachAsync(async response => 
                    {
                        Console.WriteLine($"Output: {response.Output}");
                    });
                
                Console.Write("Enter number 1: ");
                int x = Convert.ToInt32(Console.ReadLine());
                Console.Write("Enter number 2: ");
                int y = Convert.ToInt32(Console.ReadLine());

                await call.RequestStream.WriteAsync(new AdditionRequest { X = x, Y = y});
                Console.ReadLine();

                await call.RequestStream.CompleteAsync();
                await responseCompleted;
            }

            Console.WriteLine("Press enter to stop...");
            Console.ReadLine();

            await channel.ShutdownAsync();
        }
    }
}