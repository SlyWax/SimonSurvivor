using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public bool isFalling = false;
    public float fallingSpeed = 1.0f;
    private Vector3 originPosition;

    // Use this for initialization
    void Start()
    {
        originPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFalling)
        {
            if (transform.position.y > -14)
            {
                transform.position += Vector3.down * fallingSpeed * Time.deltaTime;
            }
            else if (transform.position.y > -15)
            {
                transform.position += Vector3.down * fallingSpeed * Time.deltaTime * 0.1f;
            }
            else
            {
                Reset();
            }
        }
    }

    public void Fall()
    {
        isFalling = true;
    }

    public void Reset()
    {
        transform.position = originPosition;
        isFalling = false;
    }
}
