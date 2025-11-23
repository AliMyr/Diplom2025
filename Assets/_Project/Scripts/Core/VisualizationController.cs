using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizationController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject barPrefab;

    [Header("Visualization Settings")]
    [SerializeField] private float spacing = 0.5f;
    [SerializeField] private float maxHeight = 5f;
    [SerializeField] private float animationSpeed = 0.1f;

    [Header("Materials")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material comparingMaterial;
    [SerializeField] private Material sortedMaterial;

    private List<SortingData> dataList = new List<SortingData>();
    private int maxValue;

    public void GenerateArray(int size, int minValue = 10, int maxValue = 100)
    {
        ClearArray();
        this.maxValue = maxValue;

        for (int i = 0; i < size; i++)
        {
            int value = Random.Range(minValue, maxValue + 1);
            GameObject bar = Instantiate(barPrefab, transform);

            // Установка позиции
            float xPos = (i - size / 2f) * spacing;
            bar.transform.localPosition = new Vector3(xPos, 0, 0);

            // Установка высоты
            float height = (value / (float)maxValue) * maxHeight;
            bar.transform.localScale = new Vector3(0.4f, height, 0.4f);

            // Создание данных
            SortingData data = new SortingData(value, bar);
            data.targetPosition = bar.transform.localPosition;
            dataList.Add(data);

            // Установка материала
            bar.GetComponent<Renderer>().material = defaultMaterial;
        }
    }

    public void ClearArray()
    {
        foreach (var data in dataList)
        {
            if (data.barObject != null)
                Destroy(data.barObject);
        }
        dataList.Clear();
    }

    public List<int> GetValues()
    {
        List<int> values = new List<int>();
        foreach (var data in dataList)
        {
            values.Add(data.value);
        }
        return values;
    }

    public IEnumerator HighlightBars(int index1, int index2)
    {
        if (index1 >= 0 && index1 < dataList.Count)
            dataList[index1].barObject.GetComponent<Renderer>().material = comparingMaterial;

        if (index2 >= 0 && index2 < dataList.Count)
            dataList[index2].barObject.GetComponent<Renderer>().material = comparingMaterial;

        yield return new WaitForSeconds(animationSpeed);

        if (index1 >= 0 && index1 < dataList.Count)
            dataList[index1].barObject.GetComponent<Renderer>().material = defaultMaterial;

        if (index2 >= 0 && index2 < dataList.Count)
            dataList[index2].barObject.GetComponent<Renderer>().material = defaultMaterial;
    }

    public IEnumerator SwapBars(int index1, int index2)
    {
        // Визуальный swap позиций
        Vector3 tempPos = dataList[index1].targetPosition;
        dataList[index1].targetPosition = dataList[index2].targetPosition;
        dataList[index2].targetPosition = tempPos;

        // Анимация перемещения
        float elapsed = 0f;
        Vector3 startPos1 = dataList[index1].barObject.transform.localPosition;
        Vector3 startPos2 = dataList[index2].barObject.transform.localPosition;

        while (elapsed < animationSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationSpeed;

            dataList[index1].barObject.transform.localPosition =
                Vector3.Lerp(startPos1, dataList[index1].targetPosition, t);
            dataList[index2].barObject.transform.localPosition =
                Vector3.Lerp(startPos2, dataList[index2].targetPosition, t);

            yield return null;
        }

        // Swap данных
        SortingData temp = dataList[index1];
        dataList[index1] = dataList[index2];
        dataList[index2] = temp;
    }

    public void MarkAsSorted(int index)
    {
        if (index >= 0 && index < dataList.Count)
        {
            dataList[index].barObject.GetComponent<Renderer>().material = sortedMaterial;
        }
    }

    public void MarkAllAsSorted()
    {
        foreach (var data in dataList)
        {
            data.barObject.GetComponent<Renderer>().material = sortedMaterial;
        }
    }

    public void SetAnimationSpeed(float speed)
    {
        animationSpeed = Mathf.Clamp(speed, 0.01f, 1f);
    }
}