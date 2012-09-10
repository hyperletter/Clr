﻿using System;
using System.Net;
using DispatcherUtility;
using Hyperletter.Typed;

namespace DispatcherBindTest {
    public class BindProgram {
        public static void Main() {
            var socket = new TypedUnicastSocket(new DefaultTypedHandlerFactory(), new JsonTransportSerializer());
            socket.Register<TestMessage>(IncomingTestMessage);
            socket.Bind(IPAddress.Any, 8900);

            for(int i = 0; i < 100; i++) {
                string message = "Message from BindProgram " + i;

                // Asynchronous
                Console.WriteLine(DateTime.Now + " SENDING MESSAGE (NONBLOCKING): " + message);
                socket.Send<TestMessage, TestMessage>(new TestMessage {Message = message}, AnswerCallback);

                // Blocking
                // By default batching is turned on with a timeout period of 1 second, this will throttle the speed in this demo
                // To change this behaviour send custom settings into TypedUnicastSocket constructor
                Console.WriteLine(DateTime.Now + " SENDING MESSAGE (BLOCKING)   : " + message);
                IAnswerable<TestMessage> reply = socket.Send<TestMessage, TestMessage>(new TestMessage {Message = message});
                Console.WriteLine("RECEIVED ANSWER (BLOCKING): " + reply.Message.Message);
            }

            Console.WriteLine("Waiting for messages (Press enter to continue)...");
            Console.ReadLine();
        }

        // Asynchronous answer callback
        private static void AnswerCallback(ITypedSocket socket, AnswerCallbackEventArgs<TestMessage, TestMessage> args) {
            Console.WriteLine("RECEIVED ANSWER (NONBLOCKING): " + args.Answer.Message.Message);
        }

        // Generic delegate callback on incoming messages
        private static void IncomingTestMessage(ITypedSocket typedSocket, IAnswerable<TestMessage> answerable) {
            //Console.WriteLine(DateTime.Now + " RECEIVED MESSAGE: " + answerable.Message.Message);
        }
    }
}