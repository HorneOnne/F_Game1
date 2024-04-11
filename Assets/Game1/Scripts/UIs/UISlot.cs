using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int ID;
    public RectTransform RectTrans;
    private Image IconImage;
    private Image BorderImage;
    private Image BlushImage;

    private Vector3 _startDragPosition;
    private Vector3 _endDragPosition;

    [field: SerializeField] public int X { get;  set; }
    [field: SerializeField] public int Y { get;  set; }


    private void Awake()
    {
        RectTrans = GetComponent<RectTransform>();
        IconImage = transform.Find("Icon").GetComponent<Image>();
        BorderImage = transform.Find("Border").GetComponent<Image>();
        BlushImage = transform.Find("Blush").GetComponent<Image>();

        BorderImage.enabled = false;
        BlushImage.enabled = false;
    }

    private void Update()
    {
        Vector2 targetPosition = Match3.Instance.GetCellPosition(X, Y);
        Vector2 moveDir = targetPosition - RectTrans.anchoredPosition;
        float moveSpeed = 20f;
        RectTrans.anchoredPosition += moveDir * moveSpeed * Time.deltaTime;
    }

    public void SetSlot(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public void Match()
    {
        BorderImage.enabled = true;
        BlushImage.enabled = true;
    }


    private void OnDestroy()
    {
     
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _startDragPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log($"Drag: {eventData.position}");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Match3.Instance.State != Match3.Match3State.CanPlay) return;

        _endDragPosition = eventData.position;
        Vector2 direction = (_endDragPosition - _startDragPosition).normalized;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
              
                int neighborX = X + 1;
                int neighborY = Y;
                if (Match3.Instance.IsInsideGrid(neighborX, neighborY))
                {
                    //Debug.Log("Moving right");
                    Match3.Instance.CurrentSwap = new SwapStruct(X, Y, neighborX, neighborY);
                    Match3.Instance.Swap(X, Y, neighborX, neighborY);
                    Match3.Instance.State = Match3.Match3State.Swap;
                    Match3.Instance.MoveCount++;

                  
                }
            }
            else
            {
              
                int neighborX = X - 1;
                int neighborY = Y;

                if (Match3.Instance.IsInsideGrid(neighborX, neighborY))
                {
                    //Debug.Log("Moving left");
                    Match3.Instance.CurrentSwap = new SwapStruct(X, Y, neighborX, neighborY);
                    Match3.Instance.Swap(X, Y, neighborX, neighborY);
                    Match3.Instance.State = Match3.Match3State.Swap;
                    Match3.Instance.MoveCount++;

                }
            }
        }
        else
        {
            if (direction.y > 0)
            {
          
                int neighborX = X;
                int neighborY = Y + 1;

                if (Match3.Instance.IsInsideGrid(neighborX, neighborY))
                {
                    //Debug.Log("Moving up");
                    Match3.Instance.CurrentSwap = new SwapStruct(X, Y, neighborX, neighborY);
                    Match3.Instance.Swap(X, Y, neighborX, neighborY);
                    Match3.Instance.State = Match3.Match3State.Swap;
                    Match3.Instance.MoveCount++;

                }
            }
            else
            {
               
                int neighborX = X;
                int neighborY = Y - 1;

                if (Match3.Instance.IsInsideGrid(neighborX, neighborY))
                {
                    //Debug.Log("Moving down");
                    Match3.Instance.CurrentSwap = new SwapStruct(X, Y, neighborX, neighborY);
                    Match3.Instance.Swap(X, Y, neighborX, neighborY);
                    Match3.Instance.State = Match3.Match3State.Swap;
                    Match3.Instance.MoveCount++;

                }
            }
        }
    }



   

}
