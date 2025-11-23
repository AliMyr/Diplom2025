using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSort : ISortingAlgorithm
{
    public string AlgorithmName => "Сортировка выбором";
    public string Description => "Находит минимальный элемент и помещает его в начало неотсортированной части массива.";
    public string Complexity => "O(n²)";

    public IEnumerator Sort(List<int> array, VisualizationController visualizer, StatisticsTracker stats)
    {
        int n = array.Count;

        for (int i = 0; i < n - 1; i++)
        {
            int minIndex = i;

            for (int j = i + 1; j < n; j++)
            {
                yield return visualizer.HighlightBars(minIndex, j);
                stats.IncrementComparisons();

                if (array[j] < array[minIndex])
                {
                    minIndex = j;
                }
            }

            if (minIndex != i)
            {
                int temp = array[i];
                array[i] = array[minIndex];
                array[minIndex] = temp;

                yield return visualizer.SwapBars(i, minIndex);
                stats.IncrementSwaps();
            }

            visualizer.MarkAsSorted(i);
        }

        visualizer.MarkAsSorted(n - 1);
    }
}