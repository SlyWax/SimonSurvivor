using UnityEngine;
using System.Collections;

public class CameraVRCorrection : MonoBehaviour {
    [SerializeField]
	private float moveScale = 1.0f;

	public Vector3 previousPosition;

	// Use this for initialization
	void Start () {
        updatePreviousPosition();
    }

    // Update is called once per frame
    void LateUpdate () {
		Vector3 move2D = new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z) - previousPosition;
		//move.Normalize();
		transform.Translate(-move2D * moveScale);
        updatePreviousPosition();
    }

    void updatePreviousPosition()
    {
        previousPosition = new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z);
    }
}
