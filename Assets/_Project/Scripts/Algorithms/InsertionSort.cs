using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsertionSort : ISortingAlgorithm
{
    public string AlgorithmName => "Сортировка вставками";
    public string Description => "Строит отсортированный массив по одному элементу, вставляя каждый новый элемент в правильную позицию.";
    public string Complexity => "O(n²)";

    public IEnumerator Sort(List<int> array, VisualizationController visualizer, StatisticsTracker stats)
    {
        int n = array.Count;

        for (int i = 1; i < n; i++)
        {
            int key = array[i];
            int j = i - 1;

            while (j >= 0)
            {
                yield return visualizer.HighlightBars(j, j + 1);
                stats.IncrementComparisons();

                if (array[j] > key)
                {
                    array[j + 1] = array[j];
                    yield return visualizer.SwapBars(j, j + 1);
                    stats.IncrementSwaps();
                    j--;
                }
                else
                {
                    break;
                }
            }

            array[j + 1] = key;
        }

        visualizer.MarkAllAsSorted();
    }
}