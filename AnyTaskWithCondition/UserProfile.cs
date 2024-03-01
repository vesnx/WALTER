using System.Net;

using Walter;
namespace FirstTaskWithCondition;

internal class UserProfile
{
    /// <summary>
    /// This example demonstrates handling potential real-life scenarios, 
    /// such as domain outages, with efficient task cancellation and logging.
    /// </summary>

    public UserProfile()
    {
        //from the Walter NuGet package,inverse dependency injection
        if (!Inverse.TryResolve<IHttpClientFactory>(out var factory))
            throw new InvalidOperationException("Dependency injection is not used in this application");

        //get I logger or natively log to the operating systems default log,
        //in windows that's the event log (no nuget packages needed)
        var logger = Inverse.GetLogger<UserProfile>();

        var httpClient = factory.CreateClient();


        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        //feed the extension method a non existing url and generate a exception
        var urls = new string[]
        {
                "https://api.doesnotexist.org",//A URL that will likely generate an exception.
                "https://api.ipify.org",
                "https://api.seeip.org",
                "http://api.ipaddress.com/myip"
        };

        //create the tasks using the GetStringAsync method of the HttpClient
        Task<string>[] whatsMyIp = urls.Select(url => GenerateRequests(url, cts.Token)).ToArray();
        try
        {
            //use the Array of tasks extension to get the first successful answering URL
            //we will test the success by looking at the ipString string returned by each whatsMyIp
            //task and accept it as the answer if IPAddress.TryParse returns true and we can parse the result from the web call
            if (whatsMyIp.TryAny(cts, condition: (ipString) => IPAddress.TryParse(ipString, out _), out var address))
            {
                PublicIpAddress = address;
            }
        }
        catch (Exception ex) 
        {
            //log the details including the line number and file in AoT
            logger?.LogException(ex);
        }

        //delay wrapper
        Task<string> GenerateRequests(string url, CancellationToken token)
        {
            Thread.Sleep(100);//make sure the request doesn't start before it's safe to do
            return httpClient.GetStringAsync(url, token);
        }

    }

    public string? PublicIpAddress { get; }
}

