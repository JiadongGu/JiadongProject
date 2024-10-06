using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using System;

[RequireComponent(typeof(Outline), typeof(BoxCollider))]
public class CityClicker : MonoBehaviour
{
    [HideInInspector] public Outline outline;
    public Action<CityClicker> OnClickAction;

    void Start()
    {
        outline = GetComponent<Outline>();
        ShowOutline(false);
    }

    public void ShowOutline(bool show)
    {
        outline.enabled = show;
    }

    public void OnClick()
    {
        OnClickAction?.Invoke(this);
    }
}
