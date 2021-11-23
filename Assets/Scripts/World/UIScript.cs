using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UIScript : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerInput input;

    public bool inMenu = false;

    private UIDocument mainUI;
    private Button itemsButton;

    // Used when you hit the back button
    private Button lastSelectedButton = null;

    void Start()
    {
        
    }

    private void OnEnable()
    {
        input.actions["Menu"].performed += OpenMenu;
    }

    private void OpenMenu(InputAction.CallbackContext obj)
    {
        itemsButton.Focus();
    }

    private void Awake()
    {
        mainUI = GetComponent<UIDocument>();

        itemsButton = mainUI.rootVisualElement.Query<Button>("itemsButton").First();

        lastSelectedButton = itemsButton;

        var menus = mainUI.rootVisualElement.Query<VisualElement>(null, "menu").ToList();

        foreach(var menu in menus)
        {
            menu.RegisterCallback<MouseEnterEvent>(x => inMenu = true);
            menu.RegisterCallback<MouseLeaveEvent>(x => inMenu = false);
        }

        mainUI.rootVisualElement.RegisterCallback<NavigationMoveEvent>(navMove);
        mainUI.rootVisualElement.RegisterCallback<NavigationCancelEvent>(navCancel);

        mainUI.rootVisualElement.Query<Button>("exitButton").First().clicked += exitClicked;
        //mainUI.rootVisualElement.Query<Button>("exitButton").First().RegisterCallback<ClickEvent>(exitClicked, TrickleDown.NoTrickleDown);
    }

    private void navCancel(NavigationCancelEvent evt)
    {
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
