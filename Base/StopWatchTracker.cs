using System.Collections.Generic;
using System.Diagnostics;

namespace ProjectZ.Base;

public class StopWatchTracker(int averageSize)
{
    private readonly Dictionary<string, TickCounter> _timespans = [];
    private readonly Stopwatch _stopWatch = new();

    private string _timespanName;
    private readonly int _averageSize = averageSize;

    public void Start(string timespanName)
    {
        if (_timespanName != null)
            Stop();

        _timespanName = timespanName;

        _stopWatch.Reset();
        _stopWatch.Start();
    }

    public void Stop()
    {
        if (_timespanName == null)
            return;
        
        _stopWatch.Stop();

        // add a new entry if the counter does not already exists
        if (!_timespans.ContainsKey(_timespanName))
            _timespans.Add(_timespanName, new TickCounter(_averageSize));

        _timespans[_timespanName].AddTick(_stopWatch.ElapsedTicks);

        _timespanName = null;
    }

    public string GetString()
    {
        var strCounter = "";

        foreach (var tickCounter in _timespans)
            strCounter += (strCounter == "" ? "" : "\n") + tickCounter.Key + ":\t" + tickCounter.Value.AverageTime;

        return strCounter;
    }
}
