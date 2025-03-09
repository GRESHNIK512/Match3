using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Cell : MonoBehaviour, IPointerEnterHandler
{
    public int ColorNumber { get; set; } = -1;
    public int PosX { get; private set; }
    public int PosY { get; private set; }
    public bool HasPuffed { get; set; }

    public Image FigureImage
    {
        get { return _figureImage; }
        set { _figureImage = value; }
    }
    [SerializeField] private Image _figureImage;

    public RectTransform RecTranform => _rectTransform;
    [SerializeField] private RectTransform _rectTransform;

    private CellController _cellController;
    private Vector2 _originalPosition;


    public void Init(int x, int y, Vector2 position, Vector2 size, CellController cellController)
    {
        PosX = x;
        PosY = y;
       
        _rectTransform.localPosition = position;
        _rectTransform.sizeDelta = size;
       
        _originalPosition = _figureImage.rectTransform.anchoredPosition;
        
        _cellController = cellController;
    }

    public void SetColor(int colorNumber, List<Color> availableColors)
    {
        ColorNumber = colorNumber;
       
        _figureImage.color = availableColors[colorNumber];
    }

    public void SetRandomColor(List<Color> availableColors)
    {
        ColorNumber = Random.Range(0, availableColors.Count);
       
        _figureImage.enabled = true;
        _figureImage.color = availableColors[ColorNumber];
    }

    public void SetParentForImage(Transform targetparent)
    {
        _figureImage.transform.SetParent(targetparent);
        _originalPosition = _figureImage.rectTransform.anchoredPosition;
    }

    public void FakeSwapAnimation(Vector2 targetPosition, float duration)
    {
        _figureImage.rectTransform.DOAnchorPos(targetPosition, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => MoveFigureImageToOriginalPosition(duration, false));
    }

    public void MoveFigureImageToOriginalPosition(float duration, bool searchMatch)
    {
        _figureImage.rectTransform.DOAnchorPos(_originalPosition, duration)
           .SetEase(Ease.InOutQuad)
           .OnComplete(() => _cellController?.OnMoveComplete(searchMatch));
    }

    public void Puff()
    {
        if (!HasPuffed)
        {
            _figureImage.enabled = false;
            HasPuffed = true;
        }
    }

    public Vector2 GetOriginalPosition()
    {
        return _originalPosition;
    }

    public Vector2 GetAnchoredImagePosition()
    {
        return _figureImage.rectTransform.anchoredPosition;
    }

    public void SetImageFigurePosition(Vector2 pos)
    {
        _figureImage.rectTransform.anchoredPosition = pos;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _cellController.OnCellClicked(this);
    }
}