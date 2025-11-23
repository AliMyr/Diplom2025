using UnityEngine;
using System;

public class StatisticsTracker : MonoBehaviour
{
    public int comparisons { get; private set; }
    public int swaps { get; private set; }
    public float elapsedTime { get; private set; }

    private float startTime;
    private bool isTracking;

    public event Action<int, int, float> OnStatsUpdated;

    public void ResetStats()
    {
        comparisons = 0;
        swaps = 0;
        elapsedTime = 0f;
        isTracking = false;
    }

    public void StartTracking()
    {
        startTime = Time.time;
        isTracking = true;
    }

    public void StopTracking()
    {
        isTracking = false;
        elapsedTime = Time.time - startTime;
    }

    public void IncrementComparisons()
    {
        comparisons++;
        OnStatsUpdated?.Invoke(comparisons, swaps, elapsedTime);
    }

    public void IncrementSwaps()
    {
        swaps++;
        OnStatsUpdated?.Invoke(comparisons, swaps, elapsedTime);
    }

    void Update()
    {
        if (isTracking)
        {
            elapsedTime = Time.time - startTime;
        }
    }
}