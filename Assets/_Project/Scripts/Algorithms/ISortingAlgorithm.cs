using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISortingAlgorithm
{
    string AlgorithmName { get; }
    string Description { get; }
    string Complexity { get; }

    IEnumerator Sort(List<int> array, VisualizationController visualizer, StatisticsTracker stats);
}