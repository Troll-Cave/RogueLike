using UnityEngine;
using UnityEngine.InputSystem;

using Extensions;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public InputDispatcher dispatcher;
    public UIScript uIScript;

    private Vector3 toMove = Vector3.zero;
    private bool move;

    private PlayerController controller;

    private Combat combat;

    private void OnEnable()
    {
        dispatcher.RegisterMovementHandler(DirectionalMove);
        dispatcher.RegisterClickHandler(Clicked);
    }

    void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    private void OnDestroy()
    {
    }

    private void OnDisable()
    {
    }

    private void DirectionalMove(Vector2 vector2)
    {
        // remove ghost callback
        if (this == null)
        {
            Debug.LogWarning("Dangling callback");
        }

        // don't move if you're in the 
        if (uIScript.inMenu || EventSystem.current.sendNavigationEvents == true)
        {
            return;
        }

        toMove = gameObject.transform.position + (vector2.ToVector3() * 2).Clamp();
        move = true;
    }

    // TODO: save the movement from the click and do it in a FixedUpdate
    void Clicked(Vector2 pos)
    {
        if (uIScript.inMenu || EventSystem.current.sendNavigationEvents == true)
        {
            // don't try to move in menu
            return;
        }

        // Clamp the difference
        var diff = (gameObject.transform.position - pos.ToVector3().Round().WithZ(0)).Clamp();

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
