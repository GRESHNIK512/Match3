using UnityEngine;
using System.Collections;

public class InputService : MonoBehaviour
{
    [SerializeField] private CellController _cellController; 
    private WaitForSeconds _waitForSeconds;  

    private void Awake()
    { 
        _waitForSeconds = new WaitForSeconds(0.1f);
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            StartCoroutine(CallMethodWithDelay());
        }
    }

    private IEnumerator CallMethodWithDelay()
    {
        yield return _waitForSeconds; 
        _cellController.ResetSelection();
    }
}