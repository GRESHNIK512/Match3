using UnityEngine;
using System.Collections;
using System;

public class InputService : MonoBehaviour
{
    public event Action OnMouseUpEvent;
    private WaitForSeconds _waitForSeconds;

    private void Awake()
    {
        _waitForSeconds = new WaitForSeconds(0.1f);
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
            StartCoroutine(MouseUpAfterDelay()); 
    }

    private IEnumerator MouseUpAfterDelay()
    {
        yield return _waitForSeconds;
        OnMouseUpEvent?.Invoke();
    }
}