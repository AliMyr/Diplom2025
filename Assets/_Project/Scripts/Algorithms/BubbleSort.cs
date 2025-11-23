using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSort : ISortingAlgorithm
{
    public string AlgorithmName => "Пузырьковая сортировка";
    public string Description => "Последовательно сравнивает соседние элементы и меняет их местами, если они расположены в неправильном порядке.";
    public string Complexity => "O(n²)";

    public IEnumerator Sort(List<int> array, VisualizationController visualizer, StatisticsTracker stats)
    {
        int n = array.Count;

        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                // Подсветка сравниваемых элементов
                yield return visualizer.HighlightBars(j, j + 1);

                stats.IncrementComparisons();

                if (array[j] > array[j + 1])
                {
                    // Swap
                    int temp = array[j];
                    array[j] = array[j + 1];
                    array[j + 1] = temp;

                    yield return visualizer.SwapBars(j, j + 1);
                    stats.IncrementSwaps();
                }
            }

            visualizer.MarkAsSorted(n - i - 1);
        }

        visualizer.MarkAsSorted(0);
    }
}