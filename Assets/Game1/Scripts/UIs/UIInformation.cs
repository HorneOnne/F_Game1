using UnityEngine.UI;

public class UIInformation : CustomCanvas
{
    public Button OkBtn;


    private void Start()
    {
        OkBtn.onClick.AddListener(() =>
        {
            UIGameplayManager.Instance.DisplayUIInformation(false);
            SoundManager.Instance.PlaySound(SoundType.Button, false);
        });
    }


    private void OnDestroy()
    {
        OkBtn.onClick.RemoveAllListeners();
    }
}
