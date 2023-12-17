using System.Linq;

namespace ProjectZ.Base;

public class DoubleAverage(int size)
{
    public double Average;

    private readonly double[] _timeCounts = new double[size];
    private int _currentIndex;

    public void AddValue(double value)
    {
        _timeCounts[_currentIndex] = value;

        _currentIndex++;
        if (_currentIndex >= _timeCounts.Length)
            _currentIndex = 0;

        Average = _timeCounts.Average();
    }
}
