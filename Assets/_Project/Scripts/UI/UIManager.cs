using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Dropdown algorithmDropdown;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Slider sizeSlider;
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button resetButton;

    [Header("Statistics")]
    [SerializeField] private TextMeshProUGUI comparisonsText;
    [SerializeField] private TextMeshProUGUI swapsText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI arraySizeText;

    [Header("Algorithm Info")]
    [SerializeField] private TextMeshProUGUI algorithmNameText;
    [SerializeField] private TextMeshProUGUI algorithmDescriptionText;
    [SerializeField] private TextMeshProUGUI complexityText;

    private SortingManager sortingManager;

    public void Initialize(SortingManager manager, IEnumerable<string> algorithmNames)
    {
        sortingManager = manager;

        // Инициализация dropdown
        algorithmDropdown.ClearOptions();
        algorithmDropdown.AddOptions(new List<string>(algorithmNames));
        algorithmDropdown.onValueChanged.AddListener(OnAlgorithmChanged);

        // Инициализация кнопок
        startButton.onClick.AddListener(OnStartClicked);
        stopButton.onClick.AddListener(OnStopClicked);
        resetButton.onClick.AddListener(OnResetClicked);

        // Инициализация слайдеров
        speedSlider.onValueChanged.AddListener(OnSpeedChanged);
        sizeSlider.onValueChanged.AddListener(OnSizeChanged);

        // Подписка на события статистики
        var stats = FindObjectOfType<StatisticsTracker>();
        if (stats != null)
        {
            stats.OnStatsUpdated += UpdateStatistics;
        }

        UpdateAlgorithmInfo();
    }

    private void OnAlgorithmChanged(int index)
    {
        UpdateAlgorithmInfo();
    }

    private void OnStartClicked()
    {
        string selectedAlgorithm = algorithmDropdown.options[algorithmDropdown.value].text;
        sortingManager.StartSorting(selectedAlgorithm);
    }

    private void OnStopClicked()
    {
        sortingManager.StopSorting();
    }

    private void OnResetClicked()
    {
        sortingManager.GenerateNewArray();
    }

    private void OnSpeedChanged(float value)
    {
        sortingManager.SetSortSpeed(1f - value); // Инвертируем для интуитивности
    }

    private void OnSizeChanged(float value)
    {
        sortingManager.SetArraySize((int)value);
        arraySizeText.text = $"Размер: {(int)value}";
    }

    private void UpdateStatistics(int comparisons, int swaps, float time)
    {
        comparisonsText.text = $"Сравнений: {comparisons}";
        swapsText.text = $"Перестановок: {swaps}";
        timeText.text = $"Время: {time:F2}с";
    }

    private void UpdateAlgorithmInfo()
    {
        var algorithm = sortingManager.GetCurrentAlgorithm();
        if (algorithm != null)
        {
            algorithmNameText.text = algorithm.AlgorithmName;
            algorithmDescriptionText.text = algorithm.Description;
            complexityText.text = $"Сложность: {algorithm.Complexity}";
        }
    }
}