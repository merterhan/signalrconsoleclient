using Microsoft.AspNet.SignalR.Client;
using System;

namespace SIGNALRCONSOLECLIENT._Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Set connection
            var hubConnection = new HubConnection("http://localhost:8080");

            //To enable client-side logging, set the TraceLevel and TraceWriter properties on the connection object.
            hubConnection.TraceLevel = TraceLevels.All;
            hubConnection.TraceWriter = Console.Out;

            //Make proxy to hub based on hub name on server
            var myHub = hubConnection.CreateHubProxy("MyHub");

            //Start connection
            hubConnection.Start().ContinueWith(task =>
             {
                 if (task.IsFaulted)

                     Console.WriteLine("There was an error opening the connection:{0}", task.Exception.GetBaseException());
                 else
                     Console.WriteLine("Connected");
             }).Wait();

            myHub.Invoke<string>("Send", "HELLO World ").ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error calling send: {0}",
                                      task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine(task.Result);
                }
            });
            myHub.On<string>("addMessage", param =>
            {
                Console.WriteLine(param);
            });

            myHub.Invoke<string>("DoSomething", "I'm doing something!!!").Wait();


            Console.Read();
            hubConnection.Stop();
        }
    }
}
