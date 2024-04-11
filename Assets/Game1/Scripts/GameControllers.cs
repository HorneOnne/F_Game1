using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class GameControllers : MonoBehaviour
{
    public static GameControllers Instance { get; private set; }
    public static event System.Action OnRoll;
    public static event System.Action OnSpecialGreen;
    public static event System.Action OnSpecialYellow;
    public static event System.Action OnSpecialRed;
    public static event System.Action OnSpecialPurple;


    [Range(2, 4)]
    [SerializeField] private int _numOfPlayers;
    [SerializeField] private List<Player> _players;
    [SerializeField] private int _currentTurnIndex;
    [SerializeField] private Player _currentPlayerTurn;
    private float _movementSpeed = 2f;
    public Map Map;



    public enum PlayState
    {
        None,
        Roll,
        Special_Green,
        Special_Yellow,
        Special_Red,
        Special_Purple,
        Minigame,
        Move,
    }
    public PlayState CurrentPlayState;



    public int CurrentTurnIndex { get => _currentTurnIndex; }
    public Player CurrentPlayerTurn { get => _currentPlayerTurn; }

    private void Awake()
    {
        Instance = this;
        _currentTurnIndex = 0;
        CurrentPlayState = PlayState.Roll;
    }

    private void Start()
    {
       
   

        Player.OnMoveFinished += OnPlayerFinishMove;
    }

    private void OnDestroy()
    {
        Player.OnMoveFinished -= OnPlayerFinishMove;
    }

    private void Update()
    {
        if (GameplayManager.Instance.CurrentState != GameplayManager.GameState.PLAYING) return;

        switch(CurrentPlayState)
        {
            case PlayState.None:
                if (_currentPlayerTurn.AdditionMove > 0)
                {
                    _currentPlayerTurn.AdditionMove--;

                    ChangePlayState(PlayState.Roll);
                }
                else
                {
                    NextTurn();
                    if (_currentPlayerTurn.Effect == Effect.SkipTurn)
                    {
                        _currentPlayerTurn.Effect = Effect.Default;
                        ChangePlayState(PlayState.None);
                        break;
                    }

                    ChangePlayState(PlayState.Roll);

                }      
                break;
            case PlayState.Roll:
              
                break;
            case PlayState.Special_Green:
                if(_currentTurnIndex == 0) // player
                {
                    if (Random.Range(0f, 1f) < 0.7f)
                    {
                        ChangePlayState(PlayState.Roll);
                    }
                }
                else
                {
                    if(Random.Range(0f,1f) < 0.7f)
                    {
                        ChangePlayState(PlayState.Roll);
                    }
                }
                break;
            case PlayState.Special_Yellow:
                _currentPlayerTurn.Effect = Effect.SkipTurn;
                ChangePlayState(PlayState.None);
                break;
            case PlayState.Special_Red:
                _currentPlayerTurn.ResetToStart();
                ChangePlayState(PlayState.None);
                break;
            case PlayState.Special_Purple:
                if (_currentTurnIndex == 0) // player
                {
                    UIGameplayManager.Instance.DisplayUIMiniGame(true);
                    ChangePlayState(PlayState.Minigame);
                }
                else
                {                
                    _currentPlayerTurn.AdditionMove += 2;
                    ChangePlayState(PlayState.Roll);
                }        
                break;
            case PlayState.Minigame:

                break;
            case PlayState.Move:

                break;
        }
    }

    private void OnPlayerFinishMove(Player player)
    {
        if(IsWin(player))
        {
            if(player == _players[0])
            {
                Debug.Log("Win");
                GameplayManager.Instance.ChangeGameState(GameplayManager.GameState.WIN);
            }
            else
            {
                Debug.Log("Lose");
                GameplayManager.Instance.ChangeGameState(GameplayManager.GameState.GAMEOVER);
            }
        }

        Effect effect = Map.GetEffect(player.CurrentLocationIndex);
        switch (effect)
        {
            default:
            case Effect.Default:
                ChangePlayState(PlayState.None);
                break;
            case Effect.OpportunityReroll:
                ChangePlayState(PlayState.Special_Green);
                break;
            case Effect.SkipTurn:
                ChangePlayState(PlayState.Special_Yellow);
                break;
            case Effect.ReturnStartPoint:
                ChangePlayState(PlayState.Special_Red);
                break;
            case Effect.BonusGame:
                ChangePlayState(PlayState.Special_Purple);
                break;
        }

    }

    public void ChangePlayState(PlayState state)
    {
        //if (CurrentPlayState == state) return;
        CurrentPlayState = state;

        switch (CurrentPlayState)
        {
            default: break;
            case PlayState.Roll:
                UIGameplayManager.Instance.DisplayUIDiceRoll(true);
                if (_currentTurnIndex != 0)
                {
                    UIGameplayManager.Instance.UIDiceRoll.Autoplay();
                }
               
                OnRoll?.Invoke();
                break;
            case PlayState.Special_Green:
                if(Random.Range(0f,1f) < 0.7f) // 70% can reroll
                {
                    UIGameplayManager.Instance.DisplayUIDiceRoll(true);
                    ChangePlayState(PlayState.Roll);
                }
                else
                {
                    ChangePlayState(PlayState.None);
                }
                OnSpecialGreen?.Invoke();
                break;
            case PlayState.Special_Yellow:
                OnSpecialYellow?.Invoke();
                break;
            case PlayState.Special_Red:
                OnSpecialRed?.Invoke();
                break;
            case PlayState.Special_Purple:
                OnSpecialPurple?.Invoke();
                break;

        }
    }

    public void InitializePlayers()
    {      
        _players = new();
        _numOfPlayers = GameManager.Instance.NumOfPlayers;
        for (int i = 0; i < _numOfPlayers; i++)
        {
            if (i == 0)
            {
                var mainPlayerPrefab = Resources.Load<MainPlayer>("Player");
                if (mainPlayerPrefab != null)
                {
                    var mainPlayerInstance = Instantiate(mainPlayerPrefab, Map.Waypoints[0].position, Quaternion.identity);
                    mainPlayerInstance.SpriteRenderer.sprite = GameManager.Instance.CurrentCharacter.Sprite;
                    _players.Add(mainPlayerInstance);
                }
                else
                {
                    Debug.LogError("Missing Player prefab.");
                }
            }
            else
            {
                var npcPrefab = Resources.Load<NPC>("NPC");
                if (npcPrefab != null)
                {
                    var npcInstance = Instantiate(npcPrefab, Map.Waypoints[0].position, Quaternion.identity);
                    npcInstance.SpriteRenderer.sprite = GameManager.Instance.Characters[Random.Range(0, GameManager.Instance.Characters.Count)].Sprite;
                    _players.Add(npcInstance);

                }
                else
                {
                    Debug.LogError("Missing NPC prefab.");
                }
            }

            _players[i].CurrentLocationIndex = 0;
        }
        _currentPlayerTurn = _players[_currentTurnIndex];
    }

    private void NextTurn()
    {
        _currentTurnIndex = (_currentTurnIndex + 1) % _players.Count;
        _currentPlayerTurn = _players[_currentTurnIndex];
        SortOrderLayers();
    }

    public int RollDice()
    {
        int moveStep = Dice.Instance.Roll();
        return moveStep;
    }

    public void HandleMove(int moveStep)
    {
        for (int i = 0; i < moveStep; i++)
        {
            _currentPlayerTurn.CurrentLocationIndex++;
            if (_currentPlayerTurn.CurrentLocationIndex >= Map.Waypoints.Count - 1)
            {
                _currentPlayerTurn.CurrentLocationIndex = Map.Waypoints.Count - 1;
            }
            Vector3 targetPosition = Map.Waypoints[_currentPlayerTurn.CurrentLocationIndex].position;
            _currentPlayerTurn.SetTargetPosition(targetPosition);
        }
    }

   

    public bool IsWin(Player player)
    {
        return player.CurrentLocationIndex == Map.Waypoints.Count - 1;
    }

    private void SortOrderLayers()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            _players[i].SpriteRenderer.sortingOrder = 0;
        }
        _currentPlayerTurn.SpriteRenderer.sortingOrder = 1;
    }
}
