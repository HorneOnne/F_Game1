using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer _sr;
    [field: SerializeField] public int CurrentLocationIndex { get; set; } = 0;
    public SpriteRenderer SpriteRenderer { get { return _sr; } }
    public Effect Effect { get; set; }
    private void Awake()
    {
        _sr = GetComponentInChildren<SpriteRenderer>();
        if(_sr == null)
        {
            Debug.LogError("Missing SpriteRenderer references.");
        }
    }
}
