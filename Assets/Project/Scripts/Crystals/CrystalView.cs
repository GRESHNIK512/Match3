using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CrystalView : MonoBehaviour
{
    public event Action<bool> OnEndAimationEvent;
    [SerializeField] private RectTransform _crystalRectTr;
    [SerializeField] private Image _crystalImg;

    private GameSettings _gameSettings;

    public void Init(GameSettings gameSettings, Vector2 pos, int size)
    {
        _gameSettings = gameSettings;
        _crystalRectTr.sizeDelta = new Vector2(size, size);
        _crystalRectTr.localPosition = pos;
    }

    public void ReplaceColor(int colorNumber)
    {
        _crystalImg.color = _gameSettings.Colors[colorNumber];
    }

    public void MoveToPositionWithAnimation(Vector2 newPos)
    {
        _crystalRectTr.DOAnchorPos(newPos, 0.6f)//0.25
               .SetEase(Ease.InOutQuad)
               .OnComplete(() => OnEndAimationEvent?.Invoke(true));
    }

    public void PlayFakeAnimation(Vector2 startPos, Vector2 fakePos)
    {
        _crystalRectTr.DOAnchorPos(fakePos, 0.2f)
              .SetEase(Ease.InOutQuad)
              .OnComplete(() =>
              {
                  _crystalRectTr.DOAnchorPos(startPos, 0.2f)
                  .SetEase(Ease.InOutQuad)
                  .OnComplete(() => OnEndAimationEvent?.Invoke(false));
              });
    }

    public void ChangeCrystalVisible(bool visible)
    {
        //_crystalImg.color = new Color(_crystalImg.color.r, _crystalImg.color.g, _crystalImg.color.b, 0.5f);
        _crystalImg.enabled = visible;
    }

    public void TeleportToPosition(Vector2 pos)
    {
        _crystalRectTr.anchoredPosition = pos;
    }
}