using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RealTimeMovement : MonoBehaviour
{
    public PlayerInput playerInput;

    private void OnEnable()
    {
        playerInput.actions["Click"].performed += Move;
    }

    private void Move(InputAction.CallbackContext obj)
    {
        var position = Camera.main.ScreenToWorldPoint(playerInput.actions["Look"].ReadValue<Vector2>());
        StartCoroutine(MoveToTarget(position.Round().WithZ(0)));
    }

    IEnumerator MoveToTarget(Vector3 target)
    {
        while (transform.position != target)
        {
            yield return new WaitForSeconds(.5f);
            var diff = (transform.position - target).Clamp();

            transform.position -= diff;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
