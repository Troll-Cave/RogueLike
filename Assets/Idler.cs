using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles simple frame switching for idle animations.
/// Since we don't need any more animation than this it's easier than
/// building a whole animation stack for each creature.
/// </summary>
public class Idler : MonoBehaviour
{
    public Sprite[] frames;
    public float fps = 4;

    // Number of frames
    private int frameCount;

    // Current frame
    private int frameIndex = 0;

    // The next time we want to update the image
    private float nextFrameUpdate = 0;

    // The time it takes for each frame
    private float frameTime = 0;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        frameCount = frames.Length;
        frameTime = 60f / fps / 60f;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextFrameUpdate)
        {
            var nextIndex = frameIndex + 1;
            nextIndex = nextIndex >= frames.Length ? 0 : nextIndex;
            spriteRenderer.sprite = frames[nextIndex];
            nextFrameUpdate = Time.time + frameTime;
            frameIndex = nextIndex;
        }
    }
}
