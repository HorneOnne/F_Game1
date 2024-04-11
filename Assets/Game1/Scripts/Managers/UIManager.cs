using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public UIMainmenu UIMainmenu;
    public UISettings UISettings;
    public UIPlayerSelection UIPlayerSelection;
    public UICharacterSelection UICharacterSelection;



    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        CloseAll();
        DisplayMainmenu(true);
    }

    public void CloseAll()
    {
        DisplayMainmenu(false);
        DisplaySettingsMenu(false);
        DisplayPlayerSelection(false);
        DisplayCharacterSelection(false);
    }


    public void DisplayMainmenu(bool isActive)
    {
        UIMainmenu.DisplayCanvas(isActive);
    }

    public void DisplaySettingsMenu(bool isActive)
    {
        UISettings.DisplayCanvas(isActive);
    }

    public void DisplayPlayerSelection(bool isActive)
    {
        UIPlayerSelection.DisplayCanvas(isActive);
    }

    public void DisplayCharacterSelection(bool isActive)
    {
        UICharacterSelection.DisplayCanvas(isActive);
    }
}
