using UnityEngine;

[System.Serializable]
public class SortingData
{
    public int value;
    public GameObject barObject;
    public Vector3 targetPosition;

    public SortingData(int value, GameObject barObject)
    {
        this.value = value;
        this.barObject = barObject;
    }
}