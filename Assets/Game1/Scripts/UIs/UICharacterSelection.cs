using UnityEngine.UI;

public class UICharacterSelection : CustomCanvas
{
    public Button LeftBtn;
    public Button RightBtn;
    public Button ChooseBtn;
    public Image LockIconImage;
    public Image CharacterImage;

    private void Start()
    {
        UpdateCharacter();
        LeftBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.TurnLeft();
            UpdateCharacter();
            SoundManager.Instance.PlaySound(SoundType.Button, false);
        });

        RightBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.TurnRight();
            UpdateCharacter();
            SoundManager.Instance.PlaySound(SoundType.Button, false);
        });

        ChooseBtn.onClick.AddListener(() =>
        {
            if (GameManager.Instance.CurrentCharacter.Unlock)
            {
                Loader.Load(Loader.Scene.GameplayScene);
                SoundManager.Instance.PlaySound(SoundType.Button, false);
            }
            else
            {
                SoundManager.Instance.PlaySound(SoundType.HitBlock, false);
            }
        });
    }


    private void UpdateCharacter()
    {
        if(GameManager.Instance.CurrentCharacter.Unlock)
        {
            LockIconImage.enabled = false;
            CharacterImage.sprite = GameManager.Instance.CurrentCharacter.Sprite;
            CharacterImage.material.SetColor("_Color", UnityEngine.Color.white);
            CharacterImage.SetNativeSize();
        }
        else
        {
            LockIconImage.enabled = true;
            CharacterImage.sprite = GameManager.Instance.CurrentCharacter.Sprite;
            CharacterImage.material.SetColor("_Color", UnityEngine.Color.black);
            CharacterImage.SetNativeSize();
        }
    }


    private void OnDestroy()
    {
        LeftBtn.onClick.RemoveAllListeners();
        RightBtn.onClick.RemoveAllListeners();
        ChooseBtn.onClick.RemoveAllListeners();
    }

}