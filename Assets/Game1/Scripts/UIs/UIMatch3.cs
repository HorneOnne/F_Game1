using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
public class UIMatch3 : CustomCanvas
{
    public TextMeshProUGUI MoveText;
    public Transform ProgressPanelParent;
    public List<UIMatch3Progress> ProgressList;



    private void Start()
    {
        ProgressList = new();
        int index = 0;
        foreach (var e in Match3.Instance.WinConditions)
        {
            var uiprogressPrefab = Resources.Load<UIMatch3Progress>("UIProgress");

            if(uiprogressPrefab != null)
            {
                var uiprogressInstance = Instantiate(uiprogressPrefab, ProgressPanelParent);
                uiprogressInstance.SetIcon(e.Key.ID);
                uiprogressInstance.UpdateTexts($"{Match3.Instance.ProgressCounter[index]}/{e.Value}");


                ProgressList.Add(uiprogressInstance);

            }
            else
            {
                Debug.LogError("Missing UIProgress prefab.");
            }

            index++;
        }


        Match3.OnMatched += UpdateProgressTexts;
    }

 
    private void OnDestroy()
    {
        Match3.OnMatched -= UpdateProgressTexts;
    }
    private void FixedUpdate()
    {
        MoveText.text = $"MOVE:\n{Match3.Instance.MoveCount}";
    }

    private void UpdateProgressTexts()
    {
        int index = 0;
        foreach (var e in Match3.Instance.WinConditions)
        {
            ProgressList[index].UpdateTexts($"{Match3.Instance.ProgressCounter[index]}/{e.Value}");
            index++;
        }

    }
}
