using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenusManager : MonoBehaviour
{
    public List<MenuConnector> menuConnectors = new List<MenuConnector>();

    [System.Serializable]
    public class MenuConnector
    {
        public GameObject menuPanel;
        public Button activationButton;
    }

    void Start()
    {
        foreach (var connectors in menuConnectors)
        {
            connectors.activationButton.onClick.RemoveAllListeners();
            connectors.activationButton.onClick.AddListener(() => ShowMenuPanels(false));
            connectors.activationButton.onClick.AddListener(() => connectors.menuPanel.SetActive(true));
        }
    }

    public void ShowMenuPanel(int index, bool show)
    {
        ShowMenuPanels(false);
        menuConnectors[index].menuPanel.SetActive(show);
    }

    void ShowMenuPanels(bool show)
    {
        GetAllMenuPanels().ForEach(x=>x.SetActive(show));
    }

    List<GameObject> GetAllMenuPanels()
    {
        return menuConnectors.Select(x => x.menuPanel).ToList();
    }


}
