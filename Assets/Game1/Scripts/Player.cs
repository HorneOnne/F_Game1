using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static System.Action<Player> OnMoveFinished;
    private SpriteRenderer _sr;
    [field: SerializeField] public int CurrentLocationIndex { get; set; } = 0;
    public SpriteRenderer SpriteRenderer { get { return _sr; } }
    [field: SerializeField] public Effect Effect { get; set; }

    public Queue<Vector2> _targetPositionQueue = new();
    private Vector2 _targetPosition;
    private Vector2 _startPosition;
    public int AdditionMove;
    private void Awake()
    {
        _sr = GetComponentInChildren<SpriteRenderer>();
        if(_sr == null)
        {
            Debug.LogError("Missing SpriteRenderer references.");
        }

        _targetPosition = transform.position;
        _startPosition = transform.position;
    }

    private void Update()
    {
        if(Vector2.Distance(transform.position, _targetPosition) < 0.1f)
        {
            if(_targetPositionQueue.Count > 0)
            {
                _targetPosition = _targetPositionQueue.Dequeue();

                if(_targetPositionQueue.Count == 0)
                {
                    StartCoroutine(Utilities.WaitAfter(1f, () =>
                    {
                        OnMoveFinished?.Invoke(this);
                    }));
                  
                }
            }
        }

        Vector3 moveDir = _targetPosition - (Vector2)transform.position;
        float moveSpeed = 10;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    public void SetTargetPosition(Vector2 targetPosition)
    {
        //this._targetPosition = targetPosition;
        _targetPositionQueue.Enqueue(targetPosition);
    }

    public void ResetToStart()
    {
        CurrentLocationIndex = 0;
        _targetPosition = _startPosition;
        transform.position = _startPosition;
    }
}
