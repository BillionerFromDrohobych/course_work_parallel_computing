using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AppServer
{
    class AppServer
    {
        public static ConcurrentDictionary<string, List<string>> InvertedIndex;

        static void Main(string[] args)
        {
            InvertedIndex = new ConcurrentDictionary<string, List<string>>();
            var invertedIndexClass = new InvertedIndex();
            invertedIndexClass.BuildIndex();
            ServerConnection.CreateServer();
            Console.ReadKey();
        }
    }
}