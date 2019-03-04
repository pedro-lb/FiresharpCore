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
            var items = new List<Item>();

            for (int i = 1; i <= 10; i++)
            {
                var random = new Random();

                var itemPrice = random.Next(0, 1000);
                var itemKey = random.Next();

                items.Add(new Item
                {
                    Name = $"Item {itemKey}",
                    Price = itemPrice,
                    Key = itemKey,
                });
            }

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
            }

            var itemsResponse = firebaseApp.Get(Path);
            var itemsResult = itemsResponse.ToList<Item>();
        }

        private static void ListenTests(IFirebaseClient firebaseApp)
        {
            void addedEvent(object sender, ValueAddedEventArgs args, object context)
            {
                Console.WriteLine($"Added: {args.Path}");
            }

            void changedEvent(object sender, ValueChangedEventArgs args, object context)
            {
                Console.WriteLine($"Changed: {args.Path}");
            }

            void removedEvent(object sender, ValueRemovedEventArgs args, object context)
            {
                Console.WriteLine($"Removed: {args.Path}");
            }

            var listenEvent = firebaseApp.OnAsync(Path, addedEvent, changedEvent, removedEvent).GetAwaiter().GetResult();
        }
    }
}