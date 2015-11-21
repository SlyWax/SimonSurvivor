using UnityEngine;
using System.Collections;

public class CameraVRCorrection : MonoBehaviour {
	public float moveScale = 1.0f;

	private Vector3 previousPosition;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (previousPosition == Vector3.zero) {
			previousPosition = transform.position;
		} else {
			Vector3 move = transform.position - previousPosition;
			move.Normalize();
			transform.Translate(move * moveScale);
		}
	}
}
