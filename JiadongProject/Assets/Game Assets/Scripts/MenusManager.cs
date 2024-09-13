using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class MenusManager : Singleton<MenusManager>
{
    public CameraControl cameraControl;
    public KeyCode exitPanelsShortcut = KeyCode.P;

    [HorizontalLine]

    public List<MenuConnector> menus = new List<MenuConnector>();

    [System.Serializable]
    public class MenuConnector
    {
        public GameObject menuObj;
        public List<PanelConfigs> panels = new List<PanelConfigs>();
    }

    [System.Serializable]
    public class PanelConfigs
    {
        public GameObject panelObj;
        public Button activationButton;
    }

    void Start()
    {
        foreach (var menu in menus)
        {
            foreach (var panelConfigs in menu.panels)
            {
                if (panelConfigs.activationButton != null)
                {
                    panelConfigs.activationButton.onClick.RemoveAllListeners();
                    panelConfigs.activationButton.onClick.AddListener(() => ShowAllPanelsFromMenu(menus.IndexOf(menu), false));
                    panelConfigs.activationButton.onClick.AddListener(() => panelConfigs.panelObj.SetActive(true));
                }
            }
        }

        ShowAllCanvas(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(exitPanelsShortcut))
        {
            cameraControl.enabled = true;
            ShowAllCanvas(false);
        }
    }

    public void ShowMenu(int index, bool show)
    {
        menus[index].menuObj.SetActive(show);
    }

    public void ShowAllCanvas(bool show)
    {
        menus.ForEach(x => x.menuObj.SetActive(show));
    }

    void ShowAllPanelsFromMenu(int menuIndex, bool show)
    {
        menus[menuIndex].panels.ForEach(x => x.panelObj.SetActive(show));
        if (show)
        {
            cameraControl.enabled = false;
            ShowMenu(menuIndex, true);
        }
    }

    public void ShowPanelFromMenu(int menuIndex, int panelIndex, bool show)
    {
        ShowAllPanelsFromMenu(menuIndex, false);

        menus[menuIndex].panels[panelIndex].panelObj.SetActive(show);

        if (show)
        {
            cameraControl.enabled = false;
            ShowMenu(menuIndex, true);
        }
    }
}
