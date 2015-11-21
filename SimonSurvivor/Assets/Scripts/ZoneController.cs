using UnityEngine;
using System.Collections;

public class ZoneController : MonoBehaviour {
	public bool isOpening = false;
	public float openingSpeed = 5.0f;
	public float closingSpeed = 10.0f;
	public Vector3 transformOrigin;
	public Vector3 transformDestination;

	// Use this for initialization
	void Start () {
		transformOrigin = transform.position;
		transformDestination = new Vector3 (transform.position.x * 2, transform.position.y, transform.position.z * 2);
	}
	
	// Update is called once per frame
	void Update () {
		float step;
		if (isOpening) {
			step = openingSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, transformDestination, step);
		} else {
			step = closingSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, transformOrigin, step);
		}
	}

	public ZoneController changeState() {
		isOpening = !isOpening;
		return this;
	}
}
