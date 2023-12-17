using System.Linq;

namespace ProjectZ.Base;

public class TickCounter(int average)
{
    public int AverageTime;

    private readonly int[] _timeCounts = new int[average];
    private int _currentIndex;

    public void AddTick(long tick)
    {
        _timeCounts[_currentIndex] = (int)tick;

        _currentIndex++;
        if (_currentIndex >= _timeCounts.Length)
            _currentIndex = 0;

        AverageTime = (int)_timeCounts.Average();
    }
}
