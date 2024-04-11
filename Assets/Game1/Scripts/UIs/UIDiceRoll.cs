using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
public class UIDiceRoll : CustomCanvas
{
    public Button RollBtn;
    public TextMeshProUGUI TurnText;
    public TextMeshProUGUI DiceNumberText;
    //public int RollNumber;
    private void Start()
    {
        RollBtn.onClick.AddListener(() =>
        {
            if (GameControllers.Instance.CurrentTurnIndex != 0) return;
            SoundManager.Instance.PlaySound(SoundType.Button, false);
            StartCoroutine(RollDice(() =>
            {
                int number = GameControllers.Instance.RollDice();
                DiceNumberText.text = number.ToString();
                StartCoroutine(Utilities.WaitAfter(0.5f, () =>
                {
                    GameControllers.Instance.HandleMove(number);
                    GameControllers.Instance.ChangePlayState(GameControllers.PlayState.Move);
                    UIGameplayManager.Instance.CloseAll();

                }));
            }));     
        });
    }


    private void OnDestroy()
    {
        RollBtn.onClick.RemoveAllListeners();
    }


    public void Autoplay()
    {
        SoundManager.Instance.PlaySound(SoundType.Button, false);
        StartCoroutine(RollDice(() =>
        {
            int number = GameControllers.Instance.RollDice();
            DiceNumberText.text = number.ToString();
            StartCoroutine(Utilities.WaitAfter(0.5f, () =>
            {
                GameControllers.Instance.HandleMove(number);
                GameControllers.Instance.ChangePlayState(GameControllers.PlayState.Move);
                UIGameplayManager.Instance.CloseAll();

            }));
        }));
    }

    public void UpdateTurnText()
    {
        if (GameControllers.Instance.CurrentTurnIndex == 0)
        {
            TurnText.text = "Your Turn";
        }
        else
        {
            TurnText.text = $"Player {GameControllers.Instance.CurrentTurnIndex + 1} Turn";
        }
    }


    private IEnumerator RollDice(System.Action onFinished = null)
    {
        float timeElapsed = 0.0f;
        while(true)
        {
            int number = Random.Range(1, 6);
            DiceNumberText.text = number.ToString();

            yield return new WaitForSeconds(0.02f);
            timeElapsed+= Time.deltaTime;
            if (timeElapsed > 0.5f)
                break;
        }

        onFinished?.Invoke();
    }
}
