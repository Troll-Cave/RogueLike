using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This is just to handle cross object event handler weirdness
/// </summary>
public class InputDispatcher : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerInput playerInput;

    public event Action<Vector2> onClick;
    public event Action<Vector2> onMove;
    public event Action onReload;

    public UIScript uiScript;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Movement"].performed += HandleMovement;
        playerInput.actions["Click"].performed += HandleClicks;
        playerInput.actions["Reload"].performed += handleReload;
    }

    private void OnDestroy()
    {
        // this one *is* needed
        playerInput.actions["Movement"].performed -= HandleMovement;
        playerInput.actions["Click"].performed -= HandleClicks;
        playerInput.actions["Reload"].performed -= handleReload;
    }

    private void handleReload(InputAction.CallbackContext obj)
    {
        if (onReload != null && !uiScript.inMenu)
        {
            onReload();
        }
    }

    private void HandleMovement(InputAction.CallbackContext obj)
    {
        if (onMove != null && !uiScript.inMenu)
        {
            onMove(obj.ReadValue<Vector2>());
        }
    }

    private void HandleClicks(InputAction.CallbackContext obj)
    {
        if (onClick != null && !uiScript.inMenu)
        {
            onClick(GetLookPosition());
        }
    }

    public Vector2 GetLookPosition()
    {
        return Camera.main.ScreenToWorldPoint(playerInput.actions["Look"].ReadValue<Vector2>());
    }
}
