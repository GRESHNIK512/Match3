using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour//, IPointerEnterHandler
{
    public int ColorNumber { get; set; } = -1;
    public int PosX { get; private set; }
    public int PosY { get; private set; }

    public bool HasPuffed { get; set; }

    [SerializeField] private RectTransform _rectTransform;
    public RectTransform RecTranform => _rectTransform;

    [SerializeField] private Image _figureImage;
    [SerializeField] private Image _cellIBackgroundImage;
    [SerializeField] private BoxCollider2D _boxCollider2D;

    public Image FigureImage
    {
        get { return _figureImage; }
        set { _figureImage = value; }
    }

    private CellController _cellController;
    private Vector2 _originalPosition;

    private void OnEnable()
    {
        _cellController = FindObjectOfType<CellController>();
    }

    public void SetNewName(string name)
    {
        gameObject.name = name;
    }

    public void SetCellCoordinate(int x, int y)
    {
        PosX = x;
        PosY = y;
    }

    public void SetStartPositonAndSize(Vector2 pos, Vector2 size)
    {
        _rectTransform.localPosition = pos;
        _rectTransform.sizeDelta = size;
        _boxCollider2D.size = size;
    }

    public void SetColor(int colorNumber)
    {
        ColorNumber = colorNumber;
        FigureImage.color = _cellController._availableColors[colorNumber];
    }

    public void SetRandomColor()
    {
        ColorNumber = Random.Range(0, _cellController._availableColors.Count);
        FigureImage.enabled = true;
        FigureImage.color = _cellController._availableColors[ColorNumber];
    }

    void OnMouseEnter()
    {
        _cellController.OnCellClicked(this);
    }

    public void FakeSwapAnimation(Vector2 targetPosition, float duration)
    {
        FigureImage.rectTransform.DOAnchorPos(targetPosition, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                MoveFigureImageToOriginalPosition(duration, false);
            });
    }

    public Vector2 GetOriginalPosition()
    {
        return _originalPosition;
    }

    public Vector2 GetAnchoredImagePosition()
    {
        return FigureImage.rectTransform.anchoredPosition;
    }

    public void SetImageFigurePosition(Vector2 pos)
    {
        FigureImage.rectTransform.anchoredPosition = pos;
    }

    public void MoveFigureImageToOriginalPosition(float duration, bool searchMatch)
    {
        FigureImage.rectTransform.DOAnchorPos(_originalPosition, duration)
           .SetEase(Ease.InOutQuad)
           .OnComplete(() =>
           {
               --_cellController.BusySwapCount;
               if (searchMatch) _cellController.SearchCellToMatch();
           });
    }

    public void Puff()
    {
        if (!HasPuffed)
        {
            FigureImage.enabled = false;
            HasPuffed = true;
        }
    }

    public void SetparentForImage(Transform targetparent)
    {
        _figureImage.transform.SetParent(targetparent);
        _originalPosition = FigureImage.rectTransform.anchoredPosition;
    }
}