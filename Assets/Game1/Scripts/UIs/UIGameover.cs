using UnityEngine.UI;

public class UIGameover : CustomCanvas
{
    public Button TryAgainBtn;

    private void Start()
    {
        TryAgainBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySound(SoundType.Button, false);
            Loader.Load(Loader.Scene.GameplayScene);
        });
    }


    private void OnDestroy()
    {
        TryAgainBtn.onClick.RemoveAllListeners();
    }
}
