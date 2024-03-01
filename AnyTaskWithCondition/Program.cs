using FirstTaskWithCondition;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Walter;

namespace TryAnyTaskWithCondition
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var service = new ServiceCollection()
                                    .AddHttpClient()
                                    .AddLogging(setup =>
                                    {
                                        setup.AddConsole();
                                        setup.SetMinimumLevel(LogLevel.Debug);
                                    }).BuildServiceProvider();

            service.AddLoggingForWalter();//inject logging and enable inverse dependency injection
            var profile = new UserProfile();
            try
            {
                Guard.EnsureNotNullOrEmpty(profile.PublicIpAddress);
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Profile created for user on address {profile.PublicIpAddress}");
                Console.ForegroundColor= color;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
#if !DEBUG
            //if in AoT published profile
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
#endif
            }
        }
    }
}
