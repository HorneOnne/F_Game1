using UnityEngine.UI;

public class UIMiniGame : CustomCanvas
{
    public Button StartBtn;

    private void Start()
    {
        StartBtn.onClick.AddListener(() =>
        {
            UIGameplayManager.Instance.DisplayUIMiniGame(false);
            GameplayManager.Instance.CreateMatch3();
        });
    }


    private void OnDestroy()
    {
        StartBtn.onClick.RemoveAllListeners();
    }
}
