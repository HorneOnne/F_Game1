using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject Match3Prefab;
    private GameObject _match3Instance;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C) && _match3Instance == null)
        {
            _match3Instance = Instantiate(Match3Prefab);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
             if (_match3Instance != null)
            {
                Destroy(_match3Instance.gameObject);
            }
        }
    }
}
