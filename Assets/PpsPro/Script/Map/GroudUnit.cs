using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class GroudUnit : MonoBehaviour, IPointerClickHandler 
{

    public Action ClickHandle;
    private bool isStart;
    private float clickLen = .1f;
    public void OnPointerClick(PointerEventData eventData)
    {
        ClickHandle?.Invoke();
        Debug.Log(" click");
    }
    public void Update()
    {

    }
}
