#   **Firesharp Core**

![](https://raw.githubusercontent.com/ziyasal/FireSharp/master/misc/logo.png)  

Firebase REST API wrapper for the .NET Core.

Changes are sent to all subscribed clients automatically, so you can
update your clients **in realtime** from the backend.

### Usage
This library uses [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) by default.

#### How can I configure FireSharp?
------------------------------

```csharp
  FiresharpFactory.Configure("https://yourfirebase.firebaseio.com/", "your_firebase_secret");

  var firebaseApp = FiresharpFactory.Create();
```

So far, supported methods are :

#### Set
```csharp

var todo = new Todo {
                name = "Execute SET",
                priority = 2
            };
SetResponse response = await _client.SetAsync("todos/set", todo);
Todo result = response.ResultAs<Todo>(); //The response will contain the data written
```
#### Push
```csharp

 var todo = new Todo {
                name = "Execute PUSH",
                priority = 2
            };
PushResponse response =await  _client.PushAsync("todos/push", todo);
response.Result.name //The result will contain the child name of the new data that was added
```
#### Get
```csharp

 FirebaseResponse response = await _client.GetAsync("todos/set");
 Todo todo=response.ResultAs<Todo>(); //The response will contain the data being retreived
```
#### Update
```csharp
var todo = new Todo {
                name = "Execute UPDATE!",
                priority = 1
            };

FirebaseResponse response =await  _client.UpdateAsync("todos/set", todo);
Todo todo = response.ResultAs<Todo>(); //The response will contain the data written
```
#### Delete
```csharp

FirebaseResponse response =await  _client.DeleteAsync("todos"); //Deletes todos collection
Console.WriteLine(response.StatusCode);
```
#### Listen **Streaming from the REST API**
```csharp
EventStreamResponse response = await _client.OnAsync("chat", (sender, args, context) => {
       System.Console.WriteLine(args.Data);
});

//Call dispose to stop listening for events
response.Dispose();
```

[official website](http://www.firebase.com/).

