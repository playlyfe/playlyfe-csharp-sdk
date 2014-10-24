Playlyfe C# SDK[![NuGet Status](http://nugetstatus.com/playlyfe.png)](http://nugetstatus.com/packages/playlyfe)
=================  
This is the official OAuth 2.0 C# client SDK for the Playlyfe API.  
It supports the `client_credentials` and `authorization code` OAuth 2.0 flows.    
For a complete API Reference checkout [Playlyfe Developers](https://dev.playlyfe.com/docsharp/api) for more information.

Requires
--------
.NET >= 4.0 

Install
----------
You can direcly download the playlyfe.dll file and reference it in your project
or if you are using nuget
```csharp
nuget install playlyfe
```

Using
-----
### Create a client  
  If you haven't created a client for your game yet just head over to [Playlyfe](http://playlyfe.com) and login into your account, and go to the game settings and click on client  
  **1.Client Credentials Flow**  
    In the client page click on whitelabel client  
    ![alt text](./images/client.png "")

  **2.Authorization Code Flow**  
    In the client page click on backend client and specify the redirect uri this will be the url where you will be redirected to get the token
    ![alt text](./images/auth.png "")

> Note: If you want to test the sdk in staging you can click the Test Client button.

  And then note down the client id and client secret you will need it later for using it in the sdk

The Playlyfe class allows you to make rest api calls like GET, POST, .. etc
Example: GET
```csharp
// To get infomation of the player johny
player = Playlyfe.get(
  route: "/player",
  query: new Dictionary<string, string> () { {"player_id", "student1" }}
);
Console.WriteLine(player["id"]);
Console.WriteLine(player["scores"]);

// To get all available processes with query
processes = Playlyfe.get(
  route: "/processes",
  query: new Dictionary<string, string> () {{"player_id", "student1"}}
)
Console.WriteLine(processes["total"]);
```

Example: POST
```csharp
// To start a process
process =  Playlyfe.post(
  route: "/definitions/processes/collect",
  query: new Dictionary<string, string> () { {"player_id", "johny"} },
  body: new { name = "My First Process" }
);

//To play a process
Playlyfe.post(
  route: "/processes/"+process_id+"/play",
  query: new Dictionary<string, string> () { {"player_id", "johny"} },
  body: new { trigger = trigger_name }
);
```

# Documentation
## Init
You can initiate a client by giving the client_id and client_secret params
```csharp
Playlyfe.init(
    client_id: ""
    client_secret: ""
    type: "client" or "code"
    redirect_uri: "The url to redirect to" //only for auth code flow
    store: token => { Console.WriteLine("storing"); }  // The lambda which will persist the access token to a database. You have to persist the token to a database if you want the access token to remain the same in every request
    load:  delegate { 
        var dict = new Dictionary<string, string>(); 
        return dict;
    } // The lambda which will load the access token. This is called internally by the sdk on every request so that the access token can be persisted #between requests
)
```
In development the sdk caches the access token in memory so you don"t need to provide the store and load lambdas. But in production it is highly recommended to persist the token to a database. It is very simple and easy to do it with redis. You can see the test cases for more examples.
```csharp
    using PlaylyfeSDK;

    Playlyfe.init(
      client_id: "",
      client_secret: "",
      type: "client",
      store: null,
      load: null
    )
```

## API
```csharp
Playlyfe.api(
    method: "GET" // The request method can be GET/POST/PUT/PATCH/DELETE
    route: "" // The api route to get data from
    query: Dictionary<string, string> // The query params that you want to send to the route
    raw: false // Whether you want the response to be in raw string form or json
)
```

## Get
```csharp
Playlyfe.get(
    route: "" // The api route to get data from
    query: Dictionary<string, string> // The query params that you want to send to the route
    raw: false // Whether you want the response to be in raw string form or json
)
```
## Post
```csharp
Playlyfe.post(
    route: "" // The api route to post data to
    query: Dictionary<string, string> // The query params that you want to send to the route
    body: new {} // The data you want to post to the api this will be automagically converted to json
)
```
## Patch
```csharp
Playlyfe.patch(
    route: "" // The api route to patch data
    query: Dictionary<string, string> // The query params that you want to send to the route
    body: new {} // The data you want to update in the api this will be automagically converted to json
)
```
## Put
```csharp
Playlyfe.put(
    route: "" // The api route to put data
    query: Dictionary<string, string> // The query params that you want to send to the route
    body: new {} // The data you want to update in the api this will be automagically converted to json
)
```
## Delete
```csharp
Playlyfe.delete(
    route: "" // The api route to delete the component
    query: Dictionary<string, string> // The query params that you want to send to the route
)
```
## Get Login Url
```csharp
string Playlyfe.get_login_url()
//This will return the url to which the user needs to be redirected for the user to login. You can use this directly in your views.
```

## Exchange Code
```csharp
void Playlyfe.exchange_code(code)
//This is used in the auth code flow so that the sdk can get the access token.
//Before any request to the playlyfe api is made this has to be called atleast once. 
//This should be called in the the route/controller which you specified in your redirect_uri
```

## Errors
A ```PlaylyfeException``` is thrown whenever an error occurs in each call.The Error contains a name and message field which can be used to determine the type of error that occurred.

License
=======
Playlyfe C# SDK v0.0.5  
http://dev.playlyfe.com/  
Copyright(c) 2013-2014, Playlyfe IT Solutions Pvt. Ltd, support@playlyfe.com  

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:  

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.  

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
