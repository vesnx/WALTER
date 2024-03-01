using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Walter;
namespace FirstTaskWithCondition
{
    internal class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <remarks>
        /// Get the first response from api endpoints that tell me what my public IP address is.
        /// </remarks>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {

            using var service = new ServiceCollection()
                                    .AddHttpClient()
                                    .AddLogging(setup =>
                                    {
                                        setup.AddConsole();
                                        setup.SetMinimumLevel(LogLevel.Debug);
                                    }).BuildServiceProvider();
            var color = Console.ForegroundColor;
            service.AddLoggingForWalter();//inject logging and enable inverse dependency injection
            var profile = new UserProfile();
            try
            {
                Guard.EnsureNotNullOrEmpty(profile.PublicIpAddress);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Profile created for user on {profile.PublicIpAddress}");
                Console.ForegroundColor = color;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ForegroundColor = color;
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
