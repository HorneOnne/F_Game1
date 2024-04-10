using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3 : MonoBehaviour
{
    public static Match3 Instance { get; private set; }
    [SerializeField] private List<UISlot> _slotsPrefabs;
    public UISlot[] Slots;
    [SerializeField] private Transform _parent;
    [SerializeField] private RectTransform _startPosition;
    private Vector2 _cellSize = new Vector2(150, 150);

    public const int WIDTH = 6;
    public const int HEIGHT = 5;
    private List<int> _matchListndex = new();


    private void Awake()
    {
        Instance = this;
        InitializedBoard();
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            Swap(0, 0, 1, 0);
        }
    }

    private void FixedUpdate()
    {
        SweptGrid();
    }

    private void InitializedBoard()
    {
        Slots = new UISlot[WIDTH * HEIGHT];
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


    public bool Swap(int startX, int startY, int endX, int endY)
    {
        if (startX < 0 || startX >= WIDTH || startY < 0 || startY >= HEIGHT) return false;
        if (endX < 0 || endX >= WIDTH || endY < 0 || endY >= HEIGHT) return false;

        Debug.Log("Swap");
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


    public void SweptGrid()
    {
        _matchListndex.Clear();
        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if(CheckMatch3(x, y, ref _matchListndex))
                {
                    Debug.Log("found");
                    for(int i = 0; i < _matchListndex.Count; i++)
                    {
                        Destroy(Slots[_matchListndex[i]].gameObject);

                        int cellX = _matchListndex[i] % WIDTH;
                        int cellY = _matchListndex[i] / WIDTH;
                        CreateNewSlot(cellX, cellY);
                    }
                }
        
            }
        }
    }

    private bool CheckMatch3(int x, int y, ref List<int> matchListIndex)
    {
        int id = GetSlotID(x, y);

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
}
