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
    private List<Action<Vector2>> movementHandlers = new List<Action<Vector2>>();
    private List<Action<Vector2>> clickHandlers = new List<Action<Vector2>>();

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Movement"].performed += HandleMovement;
        playerInput.actions["Click"].performed += HandleClicks;
    }

    private void HandleMovement(InputAction.CallbackContext obj)
    {
        movementHandlers.ForEach(handler =>
        {
            handler(obj.ReadValue<Vector2>());
        });
    }

    private void HandleClicks(InputAction.CallbackContext obj)
    {
        clickHandlers.ForEach(handler =>
        {
            handler(GetLookPosition());
        });
    }

    public void RegisterMovementHandler(Action<Vector2> handler)
    {
        if (!movementHandlers.Contains(handler))
        {
            movementHandlers.Add(handler);
        }
    }

    public void RegisterClickHandler(Action<Vector2> handler)
    {
        if (!clickHandlers.Contains(handler))
        {
            clickHandlers.Add(handler);
        }
    }

    public Vector2 GetLookPosition()
    {
        return Camera.main.ScreenToWorldPoint(playerInput.actions["Look"].ReadValue<Vector2>());
    }

    private void OnDestroy()
    {
        // probably don't need this but why not
        movementHandlers.Clear();

        // this one *is* needed
        playerInput.actions["Movement"].performed -= HandleMovement;
        playerInput.actions["Click"].performed -= HandleClicks;
    }
}
