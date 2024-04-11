using UnityEngine.UI;

public class UIWin : CustomCanvas
{
    public Image UnlockCharacterImage;
    public Button collectBtn;


    private void Start()
    {
        UnlockCharacterImage.sprite = GameManager.Instance.NextUnlockCharacter.Sprite;
        UnlockCharacterImage.SetNativeSize();

        collectBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySound(SoundType.Button, false);
            Loader.Load(Loader.Scene.MenuScene);
        });
    }


    private void OnDestroy()
    {
        collectBtn.onClick.RemoveAllListeners();
    }
}
