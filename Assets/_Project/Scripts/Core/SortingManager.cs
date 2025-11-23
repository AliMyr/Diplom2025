using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private VisualizationController visualizer;
    [SerializeField] private StatisticsTracker stats;
    [SerializeField] private UIManager uiManager;

    [Header("Settings")]
    [SerializeField] private int arraySize = 30;
    [SerializeField] private float sortSpeed = 0.1f;

    private Dictionary<string, ISortingAlgorithm> algorithms;
    private ISortingAlgorithm currentAlgorithm;
    private Coroutine sortingCoroutine;
    private bool isSorting = false;

    void Start()
    {
        InitializeAlgorithms();
        visualizer.SetAnimationSpeed(sortSpeed);
        GenerateNewArray();

        if (uiManager != null)
        {
            uiManager.Initialize(this, algorithms.Keys);
        }
    }

    private void InitializeAlgorithms()
    {
        algorithms = new Dictionary<string, ISortingAlgorithm>
        {
            { "Bubble", new BubbleSort() },
            { "Quick", new QuickSort() },
            { "Insertion", new InsertionSort() },
            { "Selection", new SelectionSort() }
        };

        currentAlgorithm = algorithms["Bubble"];
    }

    public void GenerateNewArray()
    {
        if (isSorting) StopSorting();

        visualizer.GenerateArray(arraySize);
        stats.ResetStats();
    }

    public void StartSorting(string algorithmName)
    {
        if (isSorting) return;

        if (algorithms.ContainsKey(algorithmName))
        {
            currentAlgorithm = algorithms[algorithmName];
            stats.ResetStats();
            stats.StartTracking();

            List<int> values = visualizer.GetValues();
            sortingCoroutine = StartCoroutine(SortingCoroutine(values));
        }
    }

    private IEnumerator SortingCoroutine(List<int> values)
    {
        isSorting = true;
        yield return currentAlgorithm.Sort(values, visualizer, stats);
        stats.StopTracking();
        isSorting = false;
    }

    public void StopSorting()
    {
        if (sortingCoroutine != null)
        {
            StopCoroutine(sortingCoroutine);
            sortingCoroutine = null;
        }

        isSorting = false;
        stats.StopTracking();
    }

    public void SetArraySize(int size)
    {
        arraySize = Mathf.Clamp(size, 5, 100);
        GenerateNewArray();
    }

    public void SetSortSpeed(float speed)
    {
        sortSpeed = Mathf.Clamp(speed, 0.01f, 1f);
        visualizer.SetAnimationSpeed(sortSpeed);
    }

    public ISortingAlgorithm GetCurrentAlgorithm()
    {
        return currentAlgorithm;
    }

    public bool IsSorting()
    {
        return isSorting;
    }
}