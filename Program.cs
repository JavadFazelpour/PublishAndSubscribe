namespace PublishAndSubscribe;

// a class to hold the information about the event
// in this case it will hold only information
// available in the clock class, but could hold
// additional state information
public class TimeInfoEventArgs : EventArgs
{
    public int hour;
    public int minute;
    public int second;
    public TimeInfoEventArgs(int hour, int minute, int second)
    {
        this.hour = hour;
        this.minute = minute;
        this.second = second;
    }
}

// The publisher: the class that other classes
// will observe. This class publishes one delegate:
// SecondChangeHandler.
public class Clock
{
    private int hour;
    private int minute;
    private int second;
    // the delegate the subscribers must implement
    public delegate void SecondChangeHandler(object clock, TimeInfoEventArgs timeInformation);

    //an instance of the delegate
    public SecondChangeHandler? SecondChanged;
    // set the clock running
    // it will raise an event for each new second
    public void Run()
    {
        for (; ; )
        {
            //sleep 100 milisecond
            Thread.Sleep(100);
            //get the current time
            System.DateTime dt = System.DateTime.Now;
            //if the second has changed notify the subscribers
            if (dt.Second != second)
            {
                // create the TimeInfoEventArgs object
                // to pass to the subscriber
                TimeInfoEventArgs timeInformation = new TimeInfoEventArgs(dt.Hour, dt.Minute, dt.Second);
                // if anyone has subscribed, notify them
                if (SecondChanged != null)
                    SecondChanged(this, timeInformation);
            }
            // update the state
            this.hour = dt.Hour;
            this.minute = dt.Minute;
            this.second = dt.Second;
        }
    }
}
// A subscriber: DisplayClock subscribes to the
// clock's events. The job of DisplayClock is
// to display the current time
public class DisplayClock
{
    // given a clock, subscribe to
    // its SecondChangeHandler event
    public void Subscribe(Clock theClock)
    {
        theClock.SecondChanged += new Clock.SecondChangeHandler(TimeHasChanged);
    }
    // the method that implements the
    // delegated functionality
    private void TimeHasChanged(object theClock, TimeInfoEventArgs ti)
    {
        Console.WriteLine($"Current time: {ti.hour}:{ti.minute}:{ti.second}");
    }
}
// a second subscriber whose job is to write to a file
public class LogCurrentTime
{
    public void Subscribe(Clock theClock)
    {
        theClock.SecondChanged += new Clock.SecondChangeHandler(WriteLogEntry);
    }
    // this method should write to a file
    // we write to the console to see the effect
    // this object keeps no state
    private void WriteLogEntry(object clock, TimeInfoEventArgs ti)
    {
        Console.WriteLine($"Logging to file: {ti.hour}:{ti.minute}:{ti.second}");
    }
}
public class Tester
{
    public void Run()
    {
        //create a new clock
        Clock clock = new Clock();

        //  create a DisplayClock
        //and tell it to subscribe to the clock just created
        DisplayClock dc = new DisplayClock();
        dc.Subscribe(clock);

        // create a Log object and tell it
        // to subscribe to the clock
        LogCurrentTime lc = new LogCurrentTime();
        lc.Subscribe(clock);
        //get the clock run
        clock.Run();
    }
}
class Program
{
    static void Main(string[] args)
    {
        Tester t = new Tester();
        t.Run();
    }
}
