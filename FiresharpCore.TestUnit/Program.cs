using FiresharpCore.EventStreaming;
using FiresharpCore.Interfaces;
using System;
using System.Collections.Generic;

namespace FiresharpCore.TestUnit
{
    internal class Program
    {
        public static string Path = "item";

        private static void Main(string[] args)
        {
            FiresharpFactory.Configure(Credentials.BasePath, Credentials.AuthSecret);

            var firebaseApp = FiresharpFactory.Create();

            ObjectTests(firebaseApp);
            ListenTests(firebaseApp);

            Console.ReadKey();
        }

        private static void ObjectTests(IFirebaseClient firebaseApp)
        {
            var itemIDs = new List<string>();

            var items = new List<Item>()
            {
                new Item()
                {
                    Name = "Item X",
                    Price = 100,
                    Key = 1,
                },
                new Item()
                {
                    Name = "Item Y",
                    Price = 250,
                    Key = 2,
                },
                new Item()
                {
                    Name = "Item Z",
                    Price = 500,
                    Key = 3,
                },
            };

            foreach (var item in items)
            {
                var pushResponse = firebaseApp.Push(Path, item);

                var itemId = pushResponse.GetID();

                itemIDs.Add(itemId);
            }

            foreach (var itemId in itemIDs)
            {
                var itemResponse = firebaseApp.Get(Path, QueryBuilder.New(itemId));

                var itemResult = itemResponse.FirstOrDefault<Item>();

                var itemsResult = itemResponse.ToList<Item>();
            }
        }

        private static void ListenTests(IFirebaseClient firebaseApp)
        {
            ValueAddedEventHandler addedEvent = (sender, args, context) =>
            {
                Console.WriteLine("Added: " + args.Data);
            };

            ValueChangedEventHandler changedEvent = (sender, args, context) =>
            {
                Console.WriteLine("Changed: " + args.Data);
            };

            ValueRemovedEventHandler removedEvent = (sender, args, context) =>
            {
                Console.WriteLine("Removed: " + args.Path);
            };

            var listenEvent = firebaseApp.OnAsync(Path, addedEvent, changedEvent, removedEvent).GetAwaiter().GetResult();
        }
    }
}