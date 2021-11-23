using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UIElements;

public class UIScript : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerInput input;
    public InputSystemUIInputModule inputSystem;

    public bool inMenu = false;

    private UIDocument mainUI;
    private Button itemsButton;

    // Used when you hit the back button
    private Button lastSelectedButton = null;

    private void OnEnable()
    {
        input.actions["Menu"].performed += OpenMenu;
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

        inputSystem.move = InputActionReference.Create(input.actions["Movement"]);

        itemsButton = mainUI.rootVisualElement.Query<Button>("itemsButton").First();

        lastSelectedButton = itemsButton;

        var menus = mainUI.rootVisualElement.Query<VisualElement>(null, "menu").ToList();

        foreach(var menu in menus)
        {
            menu.RegisterCallback<MouseEnterEvent>(x => inMenu = true);
            menu.RegisterCallback<MouseLeaveEvent>(x => inMenu = false);
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
