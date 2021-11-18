using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Extensions;

public class PlayerMovement : MonoBehaviour
{
    private InputAction lookAction;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<PlayerInput>().actions["Click"].performed += ctx => Clicked();
        lookAction = GetComponent<PlayerInput>().actions["Look"];
    }

    // TODO: save the movement from the click and do it in a FixedUpdate
    void Clicked()
    {
        // Get the current mouse position
        var pos = Camera.main.ScreenToWorldPoint(lookAction.ReadValue<Vector2>());

        // Clamp the difference
        var diff = (gameObject.transform.position - pos.Round().WithZ(0)).Clamp();

        // Apply the difference to the character
        var target = gameObject.transform.position - diff;

        var collider = Physics2D.OverlapCircle(target, .1f);

        var current = gameObject.transform.position;

        if (collider != null)
        {
            Debug.Log(collider);
            var targetX = target.WithY(current.y);
            var targetY = target.WithX(current.x);

            if (Physics2D.OverlapCircle(targetX, .1f) == null)
            {
                target = targetX;
            }
            else if (Physics2D.OverlapCircle(targetY, .1f) == null)
            {
                target = targetY;
            }
            else
            {
                target = gameObject.transform.position;
            }
        }

        gameObject.transform.position = target;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
