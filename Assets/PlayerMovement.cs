using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Extensions;
using System;

public class PlayerMovement : MonoBehaviour
{
    private InputAction lookAction;

    private Vector3 toMove = Vector3.zero;
    private PlayerController controller;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<PlayerInput>().actions["Click"].performed += ctx => Clicked();
        GetComponent<PlayerInput>().actions["Movement"].performed += ctx => DirectionalMove(ctx.ReadValue<Vector2>());


        lookAction = GetComponent<PlayerInput>().actions["Look"];
        controller = GetComponent<PlayerController>();
    }

    private void DirectionalMove(Vector2 vector2)
    {
        toMove = gameObject.transform.position + (vector2.ToVector3() * 2).Clamp();
    }

    // TODO: save the movement from the click and do it in a FixedUpdate
    void Clicked()
    {
        // Get the current mouse position
        var pos = Camera.main.ScreenToWorldPoint(lookAction.ReadValue<Vector2>());

        // Clamp the difference
        var diff = (gameObject.transform.position - pos.Round().WithZ(0)).Clamp();

        // Apply the difference to the character
        toMove = gameObject.transform.position - diff;
    }

    // All player movement goes here to align with the physics engine
    // Not sure this matters if we aren't a rigidbody but whatevs
    void FixedUpdate()
    {
        if (toMove != Vector3.zero)
        {
            controller.Move(toMove);
            // do the move
            toMove = Vector3.zero;
        }
    }
}
