using UnityEngine;
using UnityEngine.UI;

public class UIPlayerSelection : CustomCanvas
{
    public Button Player1Btn;
    public Button Player2Btn;
    public Button Player3Btn;
    public Button Player4Btn;
    public Button CloseBtn;

    private void Start()
    {
        Player1Btn.onClick.AddListener(() =>
        {
            GameManager.Instance.NumOfPlayers = 1;
            UIManager.Instance.CloseAll();
            UIManager.Instance.DisplayCharacterSelection(true);
            SoundManager.Instance.PlaySound(SoundType.Button, false);
        });

        Player2Btn.onClick.AddListener(() =>
        {
            GameManager.Instance.NumOfPlayers = 2;
            UIManager.Instance.CloseAll();
            UIManager.Instance.DisplayCharacterSelection(true);
            SoundManager.Instance.PlaySound(SoundType.Button, false);
        });

        Player3Btn.onClick.AddListener(() =>
        {
            GameManager.Instance.NumOfPlayers = 3;
            UIManager.Instance.CloseAll();
            UIManager.Instance.DisplayCharacterSelection(true);
            SoundManager.Instance.PlaySound(SoundType.Button, false);
        });

        Player4Btn.onClick.AddListener(() =>
        {
            GameManager.Instance.NumOfPlayers = 4;
            UIManager.Instance.CloseAll();
            UIManager.Instance.DisplayCharacterSelection(true);
            SoundManager.Instance.PlaySound(SoundType.Button, false);
        });

        CloseBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.CloseAll();
            UIManager.Instance.DisplayMainmenu(true);

            SoundManager.Instance.PlaySound(SoundType.Button, false);
        });
    }

    private void OnDestroy()
    {
        Player1Btn.onClick.RemoveAllListeners();
        Player2Btn.onClick.RemoveAllListeners();
        Player3Btn.onClick.RemoveAllListeners();
        Player4Btn.onClick.RemoveAllListeners();

        CloseBtn.onClick.RemoveAllListeners();
    }
}
