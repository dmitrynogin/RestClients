# RestClient

This library generates code to sent REST API requests by wrapping `HttpClient` and lets you operate at a higher level of abstraction. It only assumes one thing: request and response bodies are JSON.

## Usage

Create a public interface and decorate it with `SiteAttribute`:

```
[Site("http://www.nactem.ac.uk")]
public interface IAcromine
{

}
```

`Site` attribute defines the root web address of the API.

Specify API endpoints to consume by defining methods and decorating them with one of these four attributes:
* GetAttribute
* PostAttribute
* PutAttribute
* DeleteAttribute
```
[Site("http://www.nactem.ac.uk")]
public interface IAcromine
{
    [Get("software/acromine/dictionary.py?sf={0}")]
    Task<Definition[]> DefineAsync(string abbreviation);
}
```
The HTTP Verb attribute defines the relative URL to invoke with the option to substitute parameter values from the function argument list into the URL. Expected return type is `Task` or `Task<T>` where `T` is a JSON.NET deserializable .NET class which matches the response payload. Method name is ignored.

Define the class where the JSON response payload will be mapped to. Using dictionary]( http://www.nactem.ac.uk/software/acromine/rest.html) public API as an example:
```
public class Definition
{
    public string SF { get; set; }
    public LFS[] LFS { get; set; }
}
```

Invoke the service:
```
var acromine = RestClient.Create<IAcromine>();
var def = await acromine.DefineAsync("USA");
Console.WriteLine(def[0].LFS[0].LF); // prints "United States of America"
```

For `Post` and `Put` requests, JSON payload can be sent by marking one of the method parameters with `BodyAttribute` (see example below).

## Advanced Example
```
[Site("http://jsonplaceholder.typicode.com", Error = typeof(TypicodeError))]
public interface ITypicode
{
    [Get("posts")]
    Task<BlogPost[]> GetAsync();

    [Get("posts/{0}")]
    Task<BlogPost> GetAsync(int id);

    [Post("posts")]
    Task<BlogPost> PostAsync([Body] BlogPost data);

    [Put("posts/{0}")]
    Task<BlogPost> PutAsync(int id, [Body] BlogPost data);

    [Delete("posts/{0}")]
    Task DeleteAsync(int id);

    [Get("posts/{0}")]
    [Header("X-API-KEY: 1234567")]
    [Header("X-ID: {0}")]
    [Header("Cache-Control: {1}, max-age={2}")]
    Task<BlogPost> GetAsync(int id, out string cacheControl, out int maxAge);
}
```
Optional `Error` parameter of `SiteAttribute` allows an error responses (as defined as any response code that doesn't start with a `2`) to be deserialized and mapped to an instance of that class and available by catching `RestException<TError>`.

`HeaderAttribute` specifies request and response headers. Header content without substitution placeholders represents a request header to be sent. Headers referencing `in` and `ref` parameters will be sent to the server. Headers referencing `out` and `ref` parameters will be parsed out from the response and the values will be assigned to the arguments of the function.

Service type is available without instantiation to simplify IoC container configuation:
```
Type type = RestClient.Emit<IAcromine>();
```

For more eamples, see the [Demo](https://github.com/dmitrynogin/RestClients/blob/master/Demo/Program.cs).
