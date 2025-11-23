using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSort : ISortingAlgorithm
{
    public string AlgorithmName => "Ѕыстра€ сортировка";
    public string Description => "»спользует стратегию 'раздел€й и властвуй', выбира€ опорный элемент и раздел€€ массив на подмассивы.";
    public string Complexity => "O(n log n)";

    private VisualizationController visualizer;
    private StatisticsTracker stats;

    public IEnumerator Sort(List<int> array, VisualizationController visualizer, StatisticsTracker stats)
    {
        this.visualizer = visualizer;
        this.stats = stats;

        yield return QuickSortRecursive(array, 0, array.Count - 1);
        visualizer.MarkAllAsSorted();
    }

    private IEnumerator QuickSortRecursive(List<int> array, int low, int high)
    {
        if (low < high)
        {
            int pivotIndex = 0;
            yield return Partition(array, low, high, (index) => pivotIndex = index);

            yield return QuickSortRecursive(array, low, pivotIndex - 1);
            yield return QuickSortRecursive(array, pivotIndex + 1, high);
        }
    }

    private IEnumerator Partition(List<int> array, int low, int high, System.Action<int> callback)
    {
        int pivot = array[high];
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            yield return visualizer.HighlightBars(j, high);
            stats.IncrementComparisons();

            if (array[j] < pivot)
            {
                i++;

                int temp = array[i];
                array[i] = array[j];
                array[j] = temp;

                yield return visualizer.SwapBars(i, j);
                stats.IncrementSwaps();
            }
        }

        int temp2 = array[i + 1];
        array[i + 1] = array[high];
        array[high] = temp2;

        yield return visualizer.SwapBars(i + 1, high);
        stats.IncrementSwaps();

        callback(i + 1);
    }
}