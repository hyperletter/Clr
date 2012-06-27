﻿using System;
using System.Net;
using System.Text;
using System.Threading;
using Hyperletter.Abstraction;
using Hyperletter.Core;

namespace ConnectTest
{
    class Program
    {
        public static object SyncRoot = new object();
        static void Main(string[] args) {
            var hs = new UnicastSocket();

            int y = 0;
            hs.Sent += letter => {
                lock (SyncRoot) {
                    y++;
                    if (y%20000 == 0) {
                        Console.WriteLine("->" + y);
                    }
                }
            };
            
            int z = 0;
            hs.Received += letter => {
                z++;
                if(z % 20000 == 0)
                    Console.WriteLine("<-" + z);
            };
            
            hs.Discarded += hs_Discarded;
            hs.Requeued += letter => Console.WriteLine("REQUEUED " + Encoding.Unicode.GetString(letter.Parts[0].Data)) ;

            hs.Connect(IPAddress.Parse("127.0.0.1"), 8001);
            //hs.Connect(IPAddress.Parse("127.0.0.1"), 8002);
            //hs.Connect(IPAddress.Parse("127.0.0.1"), 8003);
            
            string line;
            while ((line = Console.ReadLine()) != null) {
                if (line == "exit")
                    return;
                
                if(line == "s")
                    Console.WriteLine(y);
                else if (line == "k")
                    hs.Dispose();
                else
                    for (int i = 0; i < 1000000; i++) {
                        hs.Send(new Letter() { Options = LetterOptions.Ack | LetterOptions.Requeue, Type = LetterType.User, Parts = new IPart[] { new Part { PartType = PartType.User, Data = Encoding.Unicode.GetBytes("Hej " + i) } } });
                        //hs.Send(new Letter() { Type = LetterType.User, Parts = new IPart[] { new Part { PartType = PartType.User, Data = Encoding.Unicode.GetBytes("Hej " + i) } } });
                        //Thread.Sleep(90);
                    }
            }
        }

        static void hs_Discarded(Binding arg1, ILetter arg2) {
            Console.WriteLine(arg1 + " " + arg2);
        }
    }
}
