using Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform toFollow = null;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (toFollow != null)
        {
            gameObject.transform.position = toFollow.position.WithZ(-1.5f);
        }
    }
}
