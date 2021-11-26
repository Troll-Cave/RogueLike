using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIScript : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerInput input;
    public InputSystemUIInputModule inputSystem;

    private List<string> activeMenus = new List<string>();

    /// <summary>
    /// Basically when you enter a menu it adds to the list and when
    /// you leave it removes it from the list. Then we can just check
    /// the menu count. Anything with the .menu class will be counted.
    /// </summary>
    public bool inMenu
    {
        get
        {
            return activeMenus.Count > 0;
        }
    }

    private UIDocument mainUI;
    private Button itemsButton;

    // Used when you hit the back button
    private Button lastSelectedButton = null;
    private Label ticker;

    private void OnEnable()
    {
        input.actions["Menu"].performed += OpenMenu;
        input.actions["Reload"].performed += reloadLol;
    }

    private void Update()
    {
        ticker.text = string.Join(" - ", TurnManager.messages);
    }

    private void OnDestroy()
    {
        if (input != null)
        {
            input.actions["Reload"].performed -= reloadLol;
        }
    }

    void reloadLol(InputAction.CallbackContext ctx)
    {
        TurnManager.messages.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    private void OpenMenu(InputAction.CallbackContext obj)
    {
        itemsButton.Focus();
        EventSystem.current.sendNavigationEvents = true;
    }

    private void Awake()
    {
        mainUI = GetComponent<UIDocument>();
        inputSystem = GetComponent<InputSystemUIInputModule>();

        itemsButton = mainUI.rootVisualElement.Query<Button>("itemsButton").First();
        ticker = mainUI.rootVisualElement.Query<Label>("statusText").First();

        lastSelectedButton = itemsButton;

        var menus = mainUI.rootVisualElement.Query<VisualElement>(null, "menu").ToList();

        foreach(var menu in menus)
        {
            menu.RegisterCallback<MouseEnterEvent>(x => activeMenus.Add(menu.name));
            menu.RegisterCallback<MouseLeaveEvent>(x => activeMenus.Remove(menu.name));
            menu.RegisterCallback<NavigationCancelEvent>(navCancel);
        }

        mainUI.rootVisualElement.RegisterCallback<NavigationMoveEvent>(navMove);

        mainUI.rootVisualElement.Query<Button>("exitButton").First().clicked += exitClicked;
    }

    private void navCancel(NavigationCancelEvent evt)
    {
        // On cancel we get rid of menus and kill navigation events
        EventSystem.current.sendNavigationEvents = false;
        var btn = evt.target as Button;

        if (btn != null)
        {
            btn.Blur();
        }

        var focussedButton = mainUI.rootVisualElement.focusController.focusedElement as Button;
        if (lastSelectedButton != null && lastSelectedButton == focussedButton)
        {
            lastSelectedButton.Blur();
            lastSelectedButton = null;
        }
    }

    private void navMove(NavigationMoveEvent evt)
    {
        var button = evt.target as Button;

        if (button != null)
        {
            //button.AddToClassList("selected");
            lastSelectedButton = button;
        }
    }

    private void exitClicked()
    {
        Debug.Log("Exit Clicked");
        Application.Quit();
    }
}
