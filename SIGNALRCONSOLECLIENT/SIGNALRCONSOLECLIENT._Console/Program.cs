﻿using Microsoft.AspNet.SignalR.Client;
using System;
using System.IO;

namespace SIGNALRCONSOLECLIENT._Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Set connection
            var hubConnection = new HubConnection("http://localhost:8080");

            //To enable client-side logging, set the TraceLevel and TraceWriter properties on the connection object.
            //Logging client events to a text file under SIGNALRCONSOLECLIENT._Console\bin\Debug
            var writer = new StreamWriter("clientLog.txt");
            writer.AutoFlush = true;
            hubConnection.TraceLevel = TraceLevels.All;
            hubConnection.TraceWriter = writer;

            //Make proxy to hub based on hub name on server
            IHubProxy myHub = hubConnection.CreateHubProxy("MyHub");

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

            #region Handle Connection Lifetime Events
            //Raised when the client detects a slow or frequently dropping connection.
            hubConnection.ConnectionSlow += () => Console.WriteLine("Connection slow");
            //Raised when the connection has disconnected.
            hubConnection.Closed += () => Console.WriteLine("Connection stopped");
            //Raised when the underlying transport has reconnected.
            hubConnection.Reconnected += () => Console.WriteLine("Reconnected");
            //Raised when the underlying transport begins reconnecting.
            hubConnection.Reconnecting += () => Console.WriteLine("Reconnecting");
            //Raised when the connection state changes. Provides the old state and the new state.
            //ConnectionState Enumeration: Connecting, Connected, Reconnecting, Disconnected
            hubConnection.StateChanged += (change) => Console.WriteLine("ConnectionState changed from: " + change.OldState + " to " + change.NewState);
            //Raised when any data is received on the connection. Provides the received data.
            hubConnection.Received += (data) => Console.WriteLine("Connection received" + data);
            #endregion Handle Connection Lifetime Events

            //To handle errors that SignalR raises, you can add a handler for the Error event on the connection object.
            hubConnection.Error += ex => Console.WriteLine("SignalR error: {0}", ex.Message);

            Console.Read();
            hubConnection.Stop();
        }
    }
}
