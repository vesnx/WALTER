using Walter;
using System.Net;
namespace FirstTaskWithCondition;

internal class UserProfile
{
    public UserProfile()
    {
        if (!Inverse.TryResolve<IHttpClientFactory>(out var factory))
            throw new InvalidOperationException("Dependency injection is not used in this application");

        //get I logger or natively log to the operating systems default log, in windows that's the event log (no nuget packages needed)
        var logger= Inverse.GetLogger<UserProfile>();

        var httpClient = factory.CreateClient();
        

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var urls = new string[]
        {
                "https://api.doesnotexist.org",//feed the extension method a non existing url and generate a exception
                "https://api.ipify.org",
                "https://api.seeip.org",
                "http://api.ipaddress.com/myip"
        };

        //create the tasks using the GetStringAsync method of the HttpClient
        Task<string>[] whatsMyIp = urls.Select(url => httpClient.GetStringAsync(url, cts.Token)).ToArray();

        //use the CancellationTokenSource extension to get the first successful answering URL
        //we will test the success by looking at the ipString string returned by each whatsMyIp
        //task and accept it as the answer if IPAddress.TryParse returns true and we can parse the result from the web call
        int successfulIndex = cts.FirstTaskWithCondition(tasks: whatsMyIp
                                                        , condition: (ipString) => IPAddress.TryParse(ipString, out _)
                                                        );

        if (successfulIndex != -1)
        {
            //we can directly assign the result from the index

            PublicIpAddress = whatsMyIp[successfulIndex].Result;
        }
        else
        {
            throw new Exception("No IP address resolved, is there no internet access?");
        }

    }

    public string? PublicIpAddress { get; }
}

