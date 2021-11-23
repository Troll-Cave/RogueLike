using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Extensions;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    private InputAction lookAction;

    private Vector3 toMove = Vector3.zero;
    private PlayerController controller;

    private Combat combat;

    // Start is called before the first frame update
    void Start()
    {
        combat = GetComponent<Combat>();
        combat.SetStats(10, 10, 10, 7, 6, 1);

        GetComponent<PlayerInput>().actions["Click"].performed += Clicked;
        GetComponent<PlayerInput>().actions["Movement"].performed += DirectionalMove;
        GetComponent<PlayerInput>().actions["Scroll"].performed += Scroll;

        GetComponent<PlayerInput>().actions["Reload"].performed += reloadLol;

        lookAction = GetComponent<PlayerInput>().actions["Look"];
        controller = GetComponent<PlayerController>();
    }

    private void OnDestroy()
    {
        GetComponent<PlayerInput>().actions["Click"].performed -= Clicked;
        GetComponent<PlayerInput>().actions["Movement"].performed -= DirectionalMove;
        GetComponent<PlayerInput>().actions["Scroll"].performed -= Scroll;

        GetComponent<PlayerInput>().actions["Reload"].performed -= reloadLol;
    }

    void reloadLol(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    private void Scroll(InputAction.CallbackContext ctx)
    {
        float v = ctx.ReadValue<float>();
        Camera.main.orthographicSize -= Mathf.Clamp(v, -2, 2);
        if (Camera.main.orthographicSize < 0)
        {
            Camera.main.orthographicSize = 0;
        }
    }

    private void DirectionalMove(InputAction.CallbackContext ctx)
    {
        Vector2 vector2 = ctx.ReadValue<Vector2>();
        toMove = gameObject.transform.position + (vector2.ToVector3() * 2).Clamp();
    }

    // TODO: save the movement from the click and do it in a FixedUpdate
    void Clicked(InputAction.CallbackContext ctx)
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
