using System.Collections;
using System.Collections.Generic;
using cakeslice;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerClicker : Singleton<PlayerClicker>
{
    public Camera cam;
    public LayerMask layerMask;

    [ReadOnly] public CityClicker hoveringObj;

    void Update()
    {
        DetectObjectClick();
    }

    void DetectObjectClick()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            hoveringObj = null;
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.TryGetComponent<CityClicker>(out CityClicker cityClicker))
            {
                hoveringObj = cityClicker;
                if (Input.GetMouseButtonDown(0))
                {
                    cityClicker.OnClick();
                }
            }
            else
            {
                hoveringObj = null;
            }
        }
        else
        {
            hoveringObj = null;
        }
    }
}
