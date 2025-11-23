using UnityEngine;

[CreateAssetMenu(fileName = "AlgorithmInfo", menuName = "Sorting/Algorithm Info")]
public class AlgorithmInfo : ScriptableObject
{
    public string algorithmName;
    [TextArea(3, 6)]
    public string description;
    public string timeComplexity;
    public string spaceComplexity;
    public bool isStable;
}