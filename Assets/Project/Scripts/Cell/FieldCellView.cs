using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class FieldCellView : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] RectTransform _rectTranform;
    public event Action<FieldCellView> OnMouseCellEnterEvent;

    public void ReplacePosition(Vector2 pos)
    {
        _rectTranform.localPosition = pos;
    }

    public void ReplaceSize(int size)
    {
        _rectTranform.sizeDelta = new Vector2(size, size);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseCellEnterEvent?.Invoke(this);
    }
}