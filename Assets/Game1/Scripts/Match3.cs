using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3 : MonoBehaviour
{
    public static Match3 Instance { get; private set; }
    public static event System.Action OnMatched;
    [SerializeField] private List<UISlot> _slotsPrefabs;
    [SerializeField] private UISlot _emtpySlotPrefab;
    public UISlot[] Slots;
    [SerializeField] private Transform _parent;
    [SerializeField] private RectTransform _startPosition;
    private Vector2 _cellSize = new Vector2(150, 150);

    public const int WIDTH = 6;
    public const int HEIGHT = 5;
    private List<int> _matchListndex = new();

    public bool CanPlay = true;


    // check still playable data
    private int[] _checkStillPlayableSlotsID;
    private Queue<GameObject> _destroyGameObjectQueue = new();

    public enum Match3State
    {
        CanPlay,
        Swap,
        WaitToDestroy,
        FallDown,
        Fill,
    }
    public Match3State State;
    public SwapStruct CurrentSwap;


    // auto play
    private float _autoPlayTimer = 0.0f;
    private float _fallDownTimer = 0.0f;
    private float _waitToDestroyTimer = 0.0f;
    private float _showMatchTimer = 0.0f;
    private float _swapCheckTimer = 0.0f;


    // Game condition
    public int MoveCount = 0;
    [SerializeField] private List<CardSO> _allcards;
    public Dictionary<CardSO, int> WinConditions = new();
    public int[] ProgressCounter;

    private void Awake()
    {
        Instance = this;

        AddWinCondition();
        ProgressCounter = new int[WinConditions.Count];
        InitializedBoard();
    }

    private void Start()
    {
        State = Match3State.CanPlay;
    }
    private void Update()
    {
        if (IsFull())
        {
            //if (SweptGrid() && State != Match3State.WaitToDestroy)
            //{
            //    //State = Match3State.FallDown;
            //    State = Match3State.WaitToDestroy;
            //}
            //else
            //{
            //    State = Match3State.CanPlay;
            //}
            if(State != Match3State.Swap)
            {
                if (SweptGrid() && State != Match3State.WaitToDestroy)
                {
             
                    //State = Match3State.FallDown;
                    State = Match3State.WaitToDestroy;       
                }
                else
                {
                    State = Match3State.CanPlay;
                }
            }          
        }



        switch (State)
        {
            case Match3State.Swap:
                _swapCheckTimer += Time.deltaTime;
                if (_swapCheckTimer > 0.2f)
                {
                    if (SweptGrid())
                    {
                        //UpdateProgress(_matchListndex[0]);
                        //bool canWin = IsWin();
                        //if (canWin)
                        //{
                        //    Debug.Log("Win");
                        //}

                        State = Match3State.WaitToDestroy;
                    }
                    else
                    {
                        Swap(CurrentSwap.x1, CurrentSwap.y1, CurrentSwap.x2, CurrentSwap.y2);
                        State = Match3State.CanPlay;
                    }
                    _swapCheckTimer = 0.0f;
                }
                break;
            case Match3State.WaitToDestroy:
                _waitToDestroyTimer += Time.deltaTime;
                _showMatchTimer += Time.deltaTime;
              
                if (_showMatchTimer > 0.05f)
                {
                    _showMatchTimer = 0.0f;
                    for (int i = 0; i < _matchListndex.Count; i++)
                    {
                        Slots[_matchListndex[i]].Match();
                    }
                }
                if (_waitToDestroyTimer > 0.15f)
                {
                    SoundManager.Instance.PlaySound(SoundType.HitBlock, false);
                    UpdateProgress(Slots[_matchListndex[0]].ID);
                    OnMatched?.Invoke();
                    bool canWin = IsWin();
                    if (canWin)
                    {
                        //Debug.Log("Win");
                        GameControllers.Instance.CurrentPlayerTurn.AdditionMove+=2;
                        GameControllers.Instance.ChangePlayState(GameControllers.PlayState.None);
                        GameplayManager.Instance.RemoveMatch3();
                    }

                    for (int i = 0; i < _matchListndex.Count; i++)
                    {
                        _destroyGameObjectQueue.Enqueue(Slots[_matchListndex[i]].gameObject);
                        int cellX = _matchListndex[i] % WIDTH;
                        int cellY = _matchListndex[i] / WIDTH;
                        CreateNewEmtpySlot(cellX, cellY);
                    }
                    if (_destroyGameObjectQueue.Count > 0)
                    {
                        while (_destroyGameObjectQueue.Count > 0)
                        {
                            Destroy(_destroyGameObjectQueue.Dequeue());
                        }
                    }

                    _showMatchTimer = 0.0f;
                    _waitToDestroyTimer = 0;
                    State = Match3State.FallDown;
                }
                break;
            case Match3State.FallDown:
                _fallDownTimer += Time.deltaTime;
                if (_fallDownTimer > 0.1f)
                {
                    _fallDownTimer = 0.0f;
                    bool isFalling = FallDown();
                    if (isFalling == false)
                    {
                        State = Match3State.Fill;
                    }
                }
                break;
            case Match3State.Fill:
                Debug.Log("Fill");
                if (IsFull())
                {
                    State = Match3State.CanPlay;
                }
                else
                {
                    CreateNewLinesOnTop();
                    State = Match3State.FallDown;
                }
                break;
        }




        _autoPlayTimer += Time.deltaTime;
        if (_autoPlayTimer > 0.5f && State == Match3State.CanPlay)
        {
            _autoPlayTimer = 0.0f;
            if (StillCanPlay(out int x1, out int y1, out int x2, out int y2))
            {
                //Swap(x1, y1, x2, y2);
            }
            else
            {
                Debug.Log("shuffle");
                Shuffle();
            }
        }

    }



    private void InitializedBoard()
    {
        Slots = new UISlot[WIDTH * HEIGHT];
        _checkStillPlayableSlotsID = new int[Slots.Length];
        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                CreateNewSlot(x, y);
            }
        }
    }

    public Vector2 GetCellPosition(int x, int y)
    {
        return new Vector2(_startPosition.anchoredPosition.x + x * _cellSize.x, _startPosition.anchoredPosition.y + y * _cellSize.y);
    }

    private void CreateNewSlot(int x, int y)
    {
        Vector2 cellPosition = GetCellPosition(x, y);
        var slot = Instantiate(_slotsPrefabs[Random.Range(0, _slotsPrefabs.Count)], _parent);
        slot.RectTrans.anchoredPosition = cellPosition;
        slot.SetSlot(x, y);
        Slots[x + y * WIDTH] = slot;
    }
    private void CreateNewEmtpySlot(int x, int y)
    {
        Vector2 cellPosition = GetCellPosition(x, y);
        var slot = Instantiate(_emtpySlotPrefab, _parent);
        slot.RectTrans.anchoredPosition = cellPosition;
        slot.SetSlot(x, y);
        Slots[x + y * WIDTH] = slot;
    }


    public bool Swap(int startX, int startY, int endX, int endY)
    {
        if (startX < 0 || startX >= WIDTH || startY < 0 || startY >= HEIGHT) return false;
        if (endX < 0 || endX >= WIDTH || endY < 0 || endY >= HEIGHT) return false;

        //Debug.Log("Swap");
        // swap in slots data
        var slotA = GetSlot(startX, startY);
        Slots[startX + startY * WIDTH] = Slots[endX + endY * WIDTH];
        Slots[endX + endY * WIDTH] = slotA;

        // update local uislot data
        Slots[startX + startY * WIDTH].SetSlot(startX, startY);
        Slots[endX + endY * WIDTH].SetSlot(endX, endY);

        // update position
        //Slots[startX + startY * WIDTH].RectTrans.anchoredPosition = GetCellPosition(startX, startY);
        //Slots[endX + endY * WIDTH].RectTrans.anchoredPosition = GetCellPosition(endX, endY);
        return true;
    }

    private UISlot GetSlot(int x, int y)
    {
        if (x < 0 || x >= WIDTH || y < 0 || y >= HEIGHT) return null;
        return Slots[x + y * WIDTH];
    }

    public bool IsInsideGrid(int x, int y)
    {
        return !(x < 0 || x >= WIDTH || y < 0 || y >= HEIGHT);
    }


    public bool SweptGrid()
    {
        _matchListndex.Clear();
        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if (CheckMatch3(x, y, ref _matchListndex))
                {                                          
                    return true;

                }

            }
        }
        return false;
    }



    private bool CheckMatch3(int x, int y, ref List<int> matchListIndex)
    {
        int id = GetSlotID(x, y);
        if (id == -1) return false;
        // Check horizontally
        if (x + 2 < WIDTH && Slots[x + 1 + y * WIDTH].ID == id && Slots[x + 2 + y * WIDTH].ID == id)
        {
            matchListIndex.Add(x + y * WIDTH);
            matchListIndex.Add((x + 1) + y * WIDTH);
            matchListIndex.Add((x + 2) + y * WIDTH);

            return true;
        }

        // Check vertically
        if (y + 2 < HEIGHT && Slots[x + (y + 1) * WIDTH].ID == id && Slots[x + (y + 2) * WIDTH].ID == id)
        {
            matchListIndex.Add(x + y * WIDTH);
            matchListIndex.Add(x + (y + 1) * WIDTH);
            matchListIndex.Add(x + (y + 2) * WIDTH);
            return true; ;
        }

        return false;
    }



    private int GetSlotID(int x, int y)
    {
        return Slots[x + y * WIDTH].ID;
    }

    private bool FallDown()
    {
        bool isFalling = false;
        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if (IsInsideGrid(x, y - 1))
                {
                    if (Slots[x + y * WIDTH].ID != -1 && Slots[x + (y - 1) * WIDTH].ID == -1)
                    {
                        Swap(x, y, x, y - 1);
                        isFalling = true;
                    }
                }

            }
        }
        return isFalling;
    }

    private void CreateNewLinesOnTop()
    {
        for (int x = 0; x < WIDTH; x++)
        {
            if (GetSlotID(x, HEIGHT - 1) == -1)
            {
                Destroy(Slots[x + (HEIGHT - 1) * WIDTH].gameObject);
                CreateNewSlot(x, HEIGHT - 1);
            }
        }
    }

    private bool IsFull()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i].ID == -1)
                return false;
        }
        return true;
    }





    #region Check match3 can still playable
    public bool TrySweptGrid()
    {
        _matchListndex.Clear();
        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if (TryCheckMatch3(x, y))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool TryCheckMatch3(int x, int y)
    {
        int id = _checkStillPlayableSlotsID[x + y * WIDTH];
        if (id == -1) return false;
        // Check horizontally
        if (x + 2 < WIDTH && _checkStillPlayableSlotsID[x + 1 + y * WIDTH] == id && _checkStillPlayableSlotsID[x + 2 + y * WIDTH] == id)
        {
            return true;
        }

        // Check vertically
        if (y + 2 < HEIGHT && _checkStillPlayableSlotsID[x + (y + 1) * WIDTH] == id && _checkStillPlayableSlotsID[x + (y + 2) * WIDTH] == id)
        {
            return true;
        }

        return false;
    }


    public void TrySwap(int startX, int startY, int endX, int endY)
    {
        var temp = _checkStillPlayableSlotsID[startX + startY * WIDTH];
        _checkStillPlayableSlotsID[startX + startY * WIDTH] = _checkStillPlayableSlotsID[endX + endY * WIDTH];
        _checkStillPlayableSlotsID[endX + endY * WIDTH] = temp;
    }

    private bool StillCanPlay(out int x1, out int y1, out int x2, out int y2)
    {
        x1 = -1;
        x2 = -1;
        y1 = -1;
        y2 = -1;
        // swap in horizontal
        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH - 1; x++)
            {
                for (int i = 0; i < Slots.Length; i++)
                {
                    _checkStillPlayableSlotsID[i] = Slots[i].ID;
                }
                TrySwap(x, y, x + 1, y);

                if (TrySweptGrid() == true)
                {
                    x1 = x; y1 = y; x2 = x + 1; y2 = y;
                    //Debug.Log($"horizontal {x} {y}     {x + 1} {y}");
                    return true;
                }
            }
        }

        // swap in vertical
        for (int y = 0; y < HEIGHT - 1; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                for (int i = 0; i < Slots.Length; i++)
                {
                    _checkStillPlayableSlotsID[i] = Slots[i].ID;
                }
                TrySwap(x, y, x, y + 1);

                if (TrySweptGrid() == true)
                {
                    x1 = x; y1 = y; x2 = x; y2 = y + 1;
                    //Debug.Log($"vertical {x} {y}     {x} {y + 1}");
                    return true;
                }
            }
        }

        return false;
    }


    private void Shuffle()
    {
        System.Random rng = new System.Random();

        int n = Slots.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);

            int x1 = k % WIDTH;
            int y1 = k / WIDTH;
            int x2 = n % WIDTH;
            int y2 = n / WIDTH;

            Swap(x1, y1, x2, y2);
        }
    }
    #endregion


    #region Win condition
    private void AddWinCondition()
    {
        int attempts = 0;
        while(true)
        {
            CardSO card = _allcards[Random.Range(0, _allcards.Count)];
            int quantity = Random.Range(1, 2);
            //int quantity = Random.Range(3, 8);

            if(WinConditions.ContainsKey(card) == false)
            {
                WinConditions.Add(card, quantity);
            }


            if(WinConditions.Count == 3)
            {
                break;
            }

            attempts++;
            if (attempts > 1000)
                break;
        }         
    }

    private void UpdateProgress(int id)
    {
        int index = 0;
        foreach(var e in WinConditions)
        {
            if(id == e.Key.ID)
            {
                if (ProgressCounter[index] < e.Value)
                    ProgressCounter[index]++;
            }

            index++;
        }
    }

    private bool IsWin()
    {
        bool canWin = true;
        int index = 0;
        foreach (var e in WinConditions)
        {
            if (ProgressCounter[index] < e.Value)
            {
                canWin = false;
                break;
            }

            index++;
        }

        return canWin;
    }
    #endregion
}


public struct SwapStruct
{
    public int x1, y1, x2, y2;

    public SwapStruct(int x1, int y1, int x2, int y2)
    {
        this.x1 = x1; this.y1 = y1;
        this.x2 = x2; this.y2 = y2;
    }
}
