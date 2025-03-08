using DG.Tweening;
using UnityEngine;

public class Root : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
        DOTween.Init();
    }
} 