# WALTER Release 2024.01.8 or later

Introducing the WALTER Framework: **W**orkable **A**lgorithms for **L**ocation-aware **T**ransmission, **E**ncryption **R**esponse. 
Designed for modern developers, WALTER is a groundbreaking suite of NuGet packages crafted for excellence in .NET Standard 2.0, 2.1, Core 3.1, and .NET 6, 7, 8, as well as C++ environments. Emphasizing 100% AoT support and reflection-free operations, this framework is the epitome of performance and stability.

Whether you're tackling networking, encryption, or secure communication, WALTER offers unparalleled efficiency and precision in processing, making it an essential tool for developers who prioritize speed and memory management in their applications.

## About this Nuget Package
This NuGet package contains helpful extension methods for any .NET project targeting anything from the Web to MAUI, Native Windows, Linux, or Mac. Even though this NuGet package is not intended for direct use, it does come with some handy features.
You can download the cmpiled help file [Here](https://github.com/vesnx/WALTER/blob/main/Help/Walter/24.1.8/Walter.chm) or look at the verion you need by browsing the release numbers.

**When Using compiled Help files you need to right click them and select `Unblock` else the contant will be blank.**

We have generated HTML help in the HTML folder for you to download. 

## Ahead-Of-Time (AOT) Compilation Compliant
The NuGet package can be used with projects that are trimming and use AoT. However, Newtonsoft Json is not AoT compliant as it uses reflection; therefore, Newtonsoft extension methods are excluded from AoT support.


### Json Extensions
We offer helpfull extension methods for when you need to process Large JSON files and can't load the whole file in memory as well as exception free processing of json files, strings and streams using the `IsValidJson<T>()` extension method

The `JsonStreamReader<T>` class in the `Walter` namespace is a powerful utility for deserializing JSON data from streams in .NET. 
This class provides a seamless way to read JSON content instance by instance, handling exceptions gracefully, 
and is compliant with various .NET versions including .NET Standard 2.0, 2.1, Core 3.1, and .NET 6, 7, 8. It's 
designed to work efficiently.

## Usage

The class is used for reading JSON data from a stream, with error handling mechanisms to capture any exceptions during the deserialization process.
### Process a file row by row so you do not have to have the whole file in memory while processing it.
```c#
using var stream = File.OpenRead("AppData\\LinkedIn.json");
var sr = new Walter.JsonStreamReader<TCPIPNetwork>(stream,TCPIPNetworkJsonContext.Default.TCPIPNetwork);
int i=0;
await foreach (TCPIPNetwork item in sr.ReadAsync())
{
    i++;
    if(sr.Errors.Count>0)
    {
        _logger.LogError(sr.Errors[i]
                        ,"File line {line}, failed so entry {i} is skipped as it failed due to {error}"
                        ,sr.Errors[i].LineNumber,i+1,sr.Errors[i].Message);
    }

}
```

### Process data from a webservice
The sample shows fetching a JSON stream from a web service streaming endpoint and processing the data row by row:
```c#
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;

// ... other necessary namespaces

public async Task ProcessJsonDataFromWebServiceAsync()
{
    using var httpClient = new HttpClient();
    try
    {
        var response = await httpClient.GetStreamAsync("https://example.com/api/datastream");
        
        var sr = new Walter.JsonStreamReader<TCPIPNetwork>(response, TCPIPNetworkJsonContext.Default.TCPIPNetwork);
        int i = 0;
        await foreach (TCPIPNetwork item in sr.ReadAsync())
        {
            i++;
            // Process each item
            // ...

            if (sr.Errors.Count > 0)
            {
                _logger.LogError(sr.Errors[i - 1],
                                 "File line {line}, failed so entry {i} is skipped as it failed due to {error}",
                                 sr.Errors[i - 1].LineNumber, i, sr.Errors[i - 1].Message);
            }
        }
    }
    catch (HttpRequestException e)
    {
        _logger.LogError("Error fetching data from web service: {Message}", e.Message);
    }
}
```

### Read a file in one time and process all entries skip those that would fail

```c#
using var stream = File.OpenRead("TestData\\LinkedIn.json");
var sr = new Walter.JsonStreamReader<List<TCPIPNetwork>>(stream,TCPIPNetworkListJsonContext.Default.ListTCPIPNetwork);
var list = sr.Read();

foreach (var item in list)
{
 ...
}
```

#### System.Net.Json extensions

#### Newtonsoft.Json extensions



### AoT Exceptions Extension for .NET

The `ExceptionsExtension` class is a powerful utility for .NET developers, enhancing exception handling with additional diagnostic information. 
This extension provides methods to extract class names, method names, file names, and approximate line numbers from exceptions especially usefull in AoT. 

#### Features
- **ClassName**: Retrieves the class name where the exception originated.
- **MethodName**: Obtains the method name that generated the exception.
- **FileName**: Gets the filename of the class that generated the exception.
- **CodeLineNumber**: Provides the actual or approximate line number where the exception was thrown.

#### Usage

Here is a examples of how to use the `ExceptionsExtension` method:
```c#
try
{
    _ = File.Open("A:\\doesNotExist.txt", FileMode.Open);
}
catch (Exception e)
{
    //if the binary is AoT compiled the line number is this line
    _logger.LogError(e, "{Class}.{method} (line {line}) failed with a {exception}:{message}",e.ClassName(), e.MethodName(), e.CodeLineNumber(),e.GetType().Name,e.Message);
}

```



Visit www.asp-waf.com for more information.
			
