using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public bool isFalling = false;
    public float fallingSpeed = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //if (isFalling)
        //{
        transform.position += Vector3.down;
        //}
    }

    public void Fall() {
        isFalling = true;
    }
}
