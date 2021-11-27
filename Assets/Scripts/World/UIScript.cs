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
    public Inventory inventory;

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
            menu.RegisterCallback<MouseEnterEvent>(x =>
            {
                Debug.Log("mouse enter " + menu.name);
                activeMenus.Add(menu.name);
            });

            menu.RegisterCallback<MouseLeaveEvent>(x =>
            {
                Debug.Log("mouse exit " + menu.name);
                activeMenus.RemoveAll(n => n == menu.name);
            });

            menu.RegisterCallback<NavigationCancelEvent>(navCancel);
        }

        mainUI.rootVisualElement.RegisterCallback<NavigationMoveEvent>(navMove);

        mainUI.rootVisualElement.Query<Button>("exitButton").First().clicked += exitClicked;
        reloadInventory();

        inventory.inventoryUpdated += reloadInventory;
    }

    /*
     * <ui:VisualElement class="inventory-item">
                        <ui:Label text="City Helmet (1)" display-tooltip-when-elided="true" class="small-box" />
                        <ui:Button text="Equip" display-tooltip-when-elided="true" class="grab-button" />
                        <ui:Button text="Drop" display-tooltip-when-elided="true" class="grab-button" />
                    </ui:VisualElement>
     */
    private void reloadInventory()
    {
        var inventoryContainer = mainUI.rootVisualElement.Query<VisualElement>("inventoryContainer").First();
        inventoryContainer.Clear();
        
        foreach (var item in inventory.Items)
        {
            var ve = new VisualElement();
            ve.AddToClassList("inventory-item");

            inventoryContainer.Add(ve);

            var itemLabel = new Label()
            {
                text = $"{item.name} ({item.quantity})"
            };

            itemLabel.AddToClassList("small-box");

            ve.Add(itemLabel);

            if (item.slot != EquipSlot.none)
            {
                // this is equipable
                var equipButton = new Button()
                {
                    text = "Equip"
                };

                equipButton.AddToClassList("grab-button");
                ve.Add(equipButton);
            }
        }
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
