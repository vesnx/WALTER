using System.Diagnostics;
using System.Runtime.InteropServices;

using Walter;
namespace ReplayClock
{
    internal partial class Program
    {

        static async Task Main(string[] args)
        {

            IClock.Instance = new NistTime();
            var now = DateTime.Now;
            while ((IClock.Instance.Now- now).TotalSeconds<10) 
            { 
                Console.SetCursorPosition(0, 0);
                var recorder = new Recorder() 
                { 
                    X=IClock.Instance.Now.Microsecond ,
                    Y=IClock.Instance.Now.Millisecond ,
                };
                Recorders.Add(recorder);
                await Task.Delay(1000);
                Console.Write(recorder.ToString());
                
            }
            Console.WriteLine();
            Console.WriteLine("Will now replay");

            //create a rest clock and set the time
            var clock=new TestClock(Recorders[0].CreatedUtc);

            IClock.Instance = clock;
            foreach (var actual in Recorders)
            { 
                //transfer the time from the recorded
                clock.Set(actual.CreatedUtc);

                var mocked= new Recorder() 
                { 
                    X=IClock.Instance.Now.Microsecond ,
                    Y=IClock.Instance.Now.Millisecond ,
                };
                //using the test clock will generate the same values as
                //the test clock is providing managed time to the 
                //application rather than the system clock
                Guard.EnsureAreEqual(mocked.ToString(), actual.ToString());
            }

            
        }

        static List<Recorder> Recorders { get; } = [];
    }
}
