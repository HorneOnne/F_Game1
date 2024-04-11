using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoading : CustomCanvas
{
    public Image LoadingIcon;
    private float _rotationSpeed = -360f; 
    void Start()
    {
        StartCoroutine(RotateCoroutine(()=>
        {
            Loader.Load(Loader.Scene.MenuScene);
        }));
    }


    private IEnumerator RotateCoroutine(System.Action onFinished = null)
    {
        float elapsedTime = 0f;
        while (elapsedTime < 2f) // Rotate for 2 seconds
        {
            float rotationAmount = _rotationSpeed * Time.deltaTime;
            LoadingIcon.rectTransform.Rotate(0f, 0f, rotationAmount);
            elapsedTime += Time.deltaTime;
            yield return null;

            if(elapsedTime > 1.5f)
            {
                break;
            }
        }

        onFinished?.Invoke();
    }
}
