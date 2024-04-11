using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
public class UIMatch3Progress : MonoBehaviour
{
    public List<Image> Icons;
    public Image Icon { get; private set; }
    public TextMeshProUGUI ProgressText;

    private void Awake()
    {
        for (int i = 0; i < Icons.Count; i++)
        {
            Icons[i].gameObject.SetActive(false);
        }
    }

    public void SetIcon(int id)
    {
        Debug.Log($"set icon: {id}");
        Icon = Icons[id];
        Icon.gameObject.SetActive(true);
       
    }

    public void UpdateTexts(string progress)
    {
        ProgressText.text = progress;
    }
}