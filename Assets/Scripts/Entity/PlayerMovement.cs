using UnityEngine;
using UnityEngine.InputSystem;

using Extensions;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public PlayerInput playerInput;
    public UIScript uIScript;

    private Vector3 toMove = Vector3.zero;
    private bool move;

    private PlayerController controller;

    private Combat combat;

    private void OnEnable()
    {
        playerInput.actions["Click"].performed -= Clicked;
        playerInput.actions["Movement"].performed -= DirectionalMove;

        playerInput.actions["Click"].performed += Clicked;
        playerInput.actions["Movement"].performed += DirectionalMove;
    }

    void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    private void OnDestroy()
    {
        if (playerInput != null)
        {
            playerInput.actions["Click"].performed -= Clicked;
            playerInput.actions["Movement"].performed -= DirectionalMove;
        }
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Click"].performed -= Clicked;
            playerInput.actions["Movement"].performed -= DirectionalMove;
        }
    }

    private void DirectionalMove(InputAction.CallbackContext ctx)
    {
        // remove ghost callback
        if (this == null)
        {
            Debug.LogWarning("Dangling callback");
            var obj = FindObjectOfType<PlayerInput>();
            obj.actions["Movement"].performed -= DirectionalMove;
            return;
        }

        // don't move if you're in the 
        if (uIScript.inMenu || EventSystem.current.sendNavigationEvents == true)
        {
            return;
        }

        Vector2 vector2 = ctx.ReadValue<Vector2>();
        toMove = gameObject.transform.position + (vector2.ToVector3() * 2).Clamp();
        move = true;
    }

    // TODO: save the movement from the click and do it in a FixedUpdate
    void Clicked(InputAction.CallbackContext ctx)
    {
        // remove ghost callback
        if (this == null)
        {
            Debug.LogWarning("Dangling callback");
            var obj = FindObjectOfType<PlayerInput>();
            obj.actions["Click"].performed -= Clicked;
            return;
        }

        if (uIScript.inMenu || EventSystem.current.sendNavigationEvents == true)
        {
            // don't try to move in menu
            return;
        }

        // Get the current mouse position
        var pos = Camera.main.ScreenToWorldPoint(playerInput.actions["Look"].ReadValue<Vector2>());

        // Clamp the difference
        var diff = (gameObject.transform.position - pos.Round().WithZ(0)).Clamp();

        // Apply the difference to the character
        toMove = gameObject.transform.position - diff;
        move = true;
    }

    // All player movement goes here to align with the physics engine
    // Not sure this matters if we aren't a rigidbody but whatevs
    void FixedUpdate()
    {
        if (move)
        {
            controller.Move(toMove);
            // do the move
            toMove = Vector3.zero;
            move = false;
        }
    }
}
