using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class GameControllers : MonoBehaviour
{
    public static GameControllers Instance { get; private set;}
    [Range(2,4)]
    [SerializeField] private int _numOfPlayers;
    [SerializeField] private List<Player> _players;
    [SerializeField] private int _currentTurnIndex;
    [SerializeField] private Player _currentPlayerTurn;
    private float _movementSpeed = 2f;
    public Map Map;

    // Cached
    private WaitForSeconds _wait = new WaitForSeconds(0.2f);
    
    private void Awake()
    {
        Instance = this;
        _currentTurnIndex = 0;
    }

    private void Start()
    {
        InitializePlayers();
        _currentPlayerTurn = _players[_currentTurnIndex];
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Move");
            RollDice();
        }
    }


    private void InitializePlayers()
    {
        _players = new();
        for(int i = 0; i < _numOfPlayers; i++)
        {
            if(i == 0)
            {
                var mainPlayerPrefab = Resources.Load<MainPlayer>("Player");
                if (mainPlayerPrefab != null)
                {
                    var mainPlayerInstance = Instantiate(mainPlayerPrefab, Map.Waypoints[0].position, Quaternion.identity);
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
                    _players.Add(npcInstance);
               
                }
                else
                {
                    Debug.LogError("Missing NPC prefab.");
                }
            }

            _players[i].CurrentLocationIndex = 0;
        }
    }

    private void NextTurn()
    {
        Debug.Log("Next turn");
        _currentTurnIndex = (_currentTurnIndex + 1) % _players.Count;
        _currentPlayerTurn = _players[_currentTurnIndex];
        SortOrderLayers();
    }

    public void RollDice()
    {
        int moveStep = Dice.Instance.Roll();
        Debug.Log($"Roll: {moveStep}");
        StartCoroutine(Move(_currentPlayerTurn, moveStep, NextTurn));
    }

    private IEnumerator Move(Player player, int step, System.Action OnFinished = null)
    {
        int stepCount = 0;
        while (player.CurrentLocationIndex < Map.Waypoints.Count)
        {
            int nextWayPointIndex = player.CurrentLocationIndex + 1;
            Vector3 targetPosition = Map.Waypoints[nextWayPointIndex].position;
            while (Vector2.Distance(player.transform.position, targetPosition) > 0.1f)
            {
                player.transform.position = Vector3.Slerp(player.transform.position, targetPosition, _movementSpeed * Time.deltaTime);
                yield return null;
            }
            player.CurrentLocationIndex++;

            stepCount++;
            if(stepCount == step)
            {
                break;
            }

           yield return null;
        }

        Effect effect = Map.GetEffect(_currentPlayerTurn.CurrentLocationIndex);
        _currentPlayerTurn.Effect = effect; 
        switch (effect)
        {
            default:
            case Effect.Default:

                break;
            case Effect.OpportunityReroll:

                break;
            case Effect.SkipTurn:

                break;
            case Effect.ReturnStartPoint:

                break;
            case Effect.BonusGame:

                break;
        }


        OnFinished?.Invoke();
    }

    private void SortOrderLayers()
    {
        for(int i = 0; i < _players.Count; i++)
        {
            _players[i].SpriteRenderer.sortingOrder = 0;
        }
        _currentPlayerTurn.SpriteRenderer.sortingOrder = 1;
    }
}
