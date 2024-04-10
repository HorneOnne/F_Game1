using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public static Dice Instance {get; private set;}
    private const int MIN = 1;
    private const int MAX = 6;

    private void Awake()
    {
        Instance = this;
    }


    public int Roll()
    {
        return Random.Range(MIN, MAX + 1);  
    }
}
