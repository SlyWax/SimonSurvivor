using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public bool isFalling = false;
	public bool impaled = false;
	public bool dead = false;
    public float fallingSpeed = 1.0f;
    private Vector3 originPosition;

	private AudioSource sonFall;
	private AudioSource sonDeath;

    // Use this for initialization
    void Start()
    {
        originPosition = transform.position;
		sonFall = (GameObject.Find("Audio Fall")).GetComponent<AudioSource>();
		sonDeath = (GameObject.Find("Audio Death")).GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFalling)
        {
            if (transform.position.y > -13.5f)
            {
                transform.position += Vector3.down * fallingSpeed * Time.deltaTime;
            }
            else if (transform.position.y > -14.75f)
            {
                transform.position += Vector3.down * fallingSpeed * Time.deltaTime * 0.025f;
				if (!impaled) {
					sonDeath.Play();
				}
				impaled = true;
            }
            else
            {
				dead = true;
            }
        }
    }

    public void Fall()
    {
        isFalling = true;
		sonFall.Play();
    }

    public void Reset()
    {
        transform.position = originPosition;
        isFalling = false;
		dead = false;
		impaled = false;
    }
}
