using UnityEngine.UI;

public class UILevelSelection : CustomCanvas
{
    public Button Level1Btn;
    public Button Level2Btn;
    public Button Level3Btn;
    public Button Level4Btn;

    private void Start()
    {
        Level1Btn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySound(SoundType.Button, false);
            GameManager.Instance.LoadLevel(1);
            GameControllers.Instance.Map = GameManager.Instance.CurrentMap;
            GameControllers.Instance.InitializePlayers();
            GameplayManager.Instance.ChangeGameState(GameplayManager.GameState.PLAYING);
            UIGameplayManager.Instance.CloseAll();

            UIGameplayManager.Instance.UIBackground.Background_01.enabled = true;
            UIGameplayManager.Instance.UIBackground.Background_01.enabled = true;
        });

        Level2Btn.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadLevel(2);
            SoundManager.Instance.PlaySound(SoundType.Button, false);
            GameControllers.Instance.Map = GameManager.Instance.CurrentMap;
            GameControllers.Instance.InitializePlayers();
            GameplayManager.Instance.ChangeGameState(GameplayManager.GameState.PLAYING);
            UIGameplayManager.Instance.CloseAll();
        });

        Level3Btn.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadLevel(3);
            SoundManager.Instance.PlaySound(SoundType.Button, false);
            GameControllers.Instance.Map = GameManager.Instance.CurrentMap;
            GameControllers.Instance.InitializePlayers();
            GameplayManager.Instance.ChangeGameState(GameplayManager.GameState.PLAYING);
            UIGameplayManager.Instance.CloseAll();
        });

        Level4Btn.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadLevel(4);
            SoundManager.Instance.PlaySound(SoundType.Button, false);
            GameControllers.Instance.Map = GameManager.Instance.CurrentMap;
            GameControllers.Instance.InitializePlayers();
            GameplayManager.Instance.ChangeGameState(GameplayManager.GameState.PLAYING);
            UIGameplayManager.Instance.CloseAll();
        });

    }

    private void OnDestroy()
    {
        Level1Btn.onClick.RemoveAllListeners();
        Level2Btn.onClick.RemoveAllListeners();
        Level3Btn.onClick.RemoveAllListeners();
        Level4Btn.onClick.RemoveAllListeners();
    }
}
