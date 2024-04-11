using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu()]
public class LevelSO : ScriptableObject
{
    public List<Transform> Waypoints;
    public List<int> GreenPointIndices;
    public List<int> YellowPointIndices;
    public List<int> RedPointIndices;
    public List<int> PurplePointIndices;
}
