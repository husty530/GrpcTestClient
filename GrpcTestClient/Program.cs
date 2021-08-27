﻿using System;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using Grpc.Net.Client;

namespace GrpcTestClient
{
    class Program
    {

        static void Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client =  new Vehicle.VehicleClient(channel);
            var message = new Request { Name = "HokudaiVehicle" };
            Console.WriteLine($"client --> server : {message}");
            var stream = client.GetStream(message).ResponseStream;
            var connector = Observable.Range(0, int.MaxValue, ThreadPoolScheduler.Instance)
                .Select(async i =>
                {
                    if (await stream.MoveNext(CancellationToken.None))
                    {
                        Console.WriteLine($"client <-- server : {stream.Current.Count}");
                    }
                })
                .Subscribe();
            Console.ReadKey();
            connector.Dispose();
        }

    }
}
