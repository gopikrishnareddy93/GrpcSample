using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Utils;
using GrpcSample.Shared;

namespace GrpcSample.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            var server = new Grpc.Core.Server
            {
                Ports = { { "127.0.0.1", 5000, ServerCredentials.Insecure } },
                Services =
                {
                    ServerServiceDefinition.CreateBuilder()
                        .AddMethod(Descriptors.Method, async (requestStream, responseStream, context) =>
                        {
                            await requestStream.ForEachAsync(async additionRequest =>
                            {
                                // handle incoming request
                                // push response into stream
                                Console.WriteLine($"Recieved addition request, number1 = {additionRequest.X} --- number2 = {additionRequest.Y}");
                                await responseStream.WriteAsync(new AdditionResponse {Output = additionRequest.X + additionRequest.Y});
                            });
                        })
                        .Build()
                }
            };

            server.Start();

            Console.WriteLine($"Server started under [127.0.0.1:5000]. Press Enter to stop it...");
            Console.ReadLine();

            await server.ShutdownAsync();
        }
    }
}