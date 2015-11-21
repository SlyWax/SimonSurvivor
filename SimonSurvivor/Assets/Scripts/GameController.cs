using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class GameController : MonoBehaviour {
    private PlayerController playerController;
    private ZoneController blueController;
    private ZoneController greenController;
    private ZoneController redController;
    private ZoneController yellowController;
    
    private BallSequenceGenerator generator = new BallSequenceGenerator();

    private float currentTime;
    private float timeLimit = 1f;

    private bool zoneOpened = false;

	// Use this for initialization
	void Start () {
        blueController = GameObject.Find("Blue Zone").GetComponent<ZoneController>();
        greenController = GameObject.Find("Green Zone").GetComponent<ZoneController>();
        redController = GameObject.Find("Red Zone").GetComponent<ZoneController>();
        yellowController = GameObject.Find("Yellow Zone").GetComponent<ZoneController>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        Reset();
    }
	
	// Update is called once per frame
	void Update () {
        currentTime += Time.deltaTime;
        if (currentTime < timeLimit)
        {
            return;
        }
        if (zoneOpened == false)
        {
            zoneOpened = true;
            BallColor color = generator.addNewBall().getLastColor();
            OpenZone(color);
            currentTime = 0;
            playerController.Fall();
        }
        else
        {
            CloseZone();
            Reset();
        }
	}

    private void OpenZone(BallColor color)
    {
        if (color == BallColor.Blue)
        {
            greenController.changeState();
            redController.changeState();
            yellowController.changeState();
        }
        else if (color == BallColor.Green)
        {
            blueController.changeState();
            redController.changeState();
            yellowController.changeState();
        }
        else if (color == BallColor.Red)
        {
            blueController.changeState();
            greenController.changeState();
            yellowController.changeState();
        }
        else if (color == BallColor.Yellow)
        {
            blueController.changeState();
            greenController.changeState();
            redController.changeState();
        }
    }

    private void CloseZone()
    {
        blueController.isOpening = false;
        greenController.isOpening = false;
        redController.isOpening = false;
        yellowController.isOpening = false;
    }

    private void Reset()
    {
        currentTime = 0;
        generator.reset();
        zoneOpened = false;
    }
}
