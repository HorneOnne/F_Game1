using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3 : MonoBehaviour
{
    public static Match3 Instance { get; private set; }
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


    enum Match3State
    {
        Swept,
        FallDown,
    }


    private void Awake()
    {
        Instance = this;
        InitializedBoard();
    }


    private void Update()
    {
      
        //if(Input.GetKeyDown(KeyCode.W))
        //{
        //    SweptGrid();
        //}


        //if (Input.GetKeyDown(KeyCode.F))
        //{  
        //    bool falling = FallDown();
        //    if(falling)
        //    {
        //        CreateNewLinesOnTop();
        //    }
           

        //    Debug.Log($"Fall: {falling}");
        //}

        //SweptGrid();
        //FallDown();
        //bool isFilled = CreateNewLinesOnTop();
        //if(isFilled == true)
        //{
        //    Debug.Log("filled");
        //}

        if(CanPlay == true)
        {
            if(SweptGrid())
            {
                CanPlay = false;
                StartCoroutine(LogicHandlerCoroutine());
            }
            else
            {
                if (StillCanPlay(out int x1, out int y1, out int x2, out int y2))
                {
                    Swap(x1, y1, x2, y2);
                }
                else
                {
                    Debug.Log("cannot play");
                    Shuffle();
                }
            }
        }

    }

    public IEnumerator LogicHandlerCoroutine()
    {
        while(true)
        {
            FallDown();
            bool isFull = CreateNewLinesOnTop();
            if (isFull == true)
            {
                break;
            }
            yield return new WaitForSeconds(0.2f);
        }
        CanPlay = true;
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

    private Vector2 GetCellPosition(int x, int y)
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
        Slots[startX + startY * WIDTH].RectTrans.anchoredPosition = GetCellPosition(startX, startY);
        Slots[endX + endY * WIDTH].RectTrans.anchoredPosition = GetCellPosition(endX, endY);
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
                if(CheckMatch3(x, y, ref _matchListndex))
                {
                    for(int i = 0; i < _matchListndex.Count; i++)
                    {
                        Destroy(Slots[_matchListndex[i]].gameObject);

                        int cellX = _matchListndex[i] % WIDTH;
                        int cellY = _matchListndex[i] / WIDTH;
                        CreateNewEmtpySlot(cellX, cellY);
                    }
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
        bool falling = false;
        for(int y = 0; y < HEIGHT; y++)
        {
            for(int x = 0; x < WIDTH; x++)
            {
                if(IsInsideGrid(x, y-1))
                {
                    if (Slots[x + (y - 1) * WIDTH].ID == -1)
                    {
                        Swap(x, y, x, y - 1);
                        falling = true;
                    }
                }

            }
        }
        return falling;
    }

    private bool CreateNewLinesOnTop()
    {
        bool isFull = true;
        for(int x = 0; x < WIDTH; x++)
        {
            if(GetSlotID(x,HEIGHT-1) == -1)
            {
                Destroy(Slots[x + (HEIGHT - 1) * WIDTH].gameObject);
                CreateNewSlot(x, HEIGHT - 1);
                isFull = false;
            }
        }
        return isFull;
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
                    x1 = x; y1 = y; x2 = x + 1;y2 = y;
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
}
