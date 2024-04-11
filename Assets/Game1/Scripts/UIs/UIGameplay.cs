using UnityEngine.UI;

public class UIGameplay : CustomCanvas
{
    public Button BackBtn;
    public Button InformationBtn;


    private void Start()
    {
        BackBtn.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MenuScene);
            SoundManager.Instance.PlaySound(SoundType.Button, false);
        });

        InformationBtn.onClick.AddListener(() =>
        {
            UIGameplayManager.Instance.DisplayUIInformation(true);
            SoundManager.Instance.PlaySound(SoundType.Button, false);
        });
    }

    private void OnDestroy()
    {
        BackBtn.onClick.RemoveAllListeners();
        InformationBtn.onClick.RemoveAllListeners();
    }
}
