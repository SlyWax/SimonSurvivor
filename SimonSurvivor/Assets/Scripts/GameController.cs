using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using AssemblyCSharp;

public class GameController : MonoBehaviour {
    private PlayerController playerController;
    private ZoneController blueController;
    private ZoneController greenController;
    private ZoneController redController;
    private ZoneController yellowController;
	private GameObject blueZone;
	private GameObject greenZone;
	private GameObject redZone;
	private GameObject yellowZone;
	private Camera mainCamera;

    private BallSequenceGenerator generator = new BallSequenceGenerator();

    private float currentTime;
    private float timeLimit = 1f;

    private bool zoneOpened = false;

	// Use this for initialization
	void Start () {
		blueZone = GameObject.Find("Blue Zone");
		greenZone = GameObject.Find("Green Zone");
		redZone = GameObject.Find("Red Zone");
		yellowZone = GameObject.Find("Yellow Zone");
		blueController = blueZone.GetComponent<ZoneController>();
        greenController = greenZone.GetComponent<ZoneController>();
		redController = redZone.GetComponent<ZoneController>();
		yellowController = yellowZone.GetComponent<ZoneController>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
		mainCamera = Camera.main;
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
            OpenAllZonesOtherThan(color);
            currentTime = 0;
			if (PlayerIsInSafeZone(color)) {
				playerController.Fall();
			}
        }
        else
        {
            CloseZone();
            Reset();
        }
	}

	private void OpenAllZonesOtherThan(BallColor color)
    {
		var availableColors = Enum.GetValues(typeof(BallColor)).Cast<BallColor>();
		var hazardousZones = availableColors.Where(c => c != color)
											.Select(c => ZoneFor(c));
		foreach (GameObject zone in hazardousZones) {
			zone.GetComponent<ZoneController>().changeState();
		}
    }

    private void CloseZone()
    {
		var availableColors = Enum.GetValues(typeof(BallColor)).Cast<BallColor>();
		var allZones = availableColors.Select(c => ZoneFor(c));
		foreach (GameObject zone in allZones) {
			zone.GetComponent<ZoneController>().isOpening = false;
		}
    }

    private void Reset()
    {
        currentTime = 0;
        generator.reset();
        zoneOpened = false;
    }

	private bool PlayerIsInSafeZone(BallColor color) {
		Ray ray = new Ray(mainCamera.transform.position, Vector3.down);
		RaycastHit hit;
		bool isInSafeZone = false;

		if (Physics.Raycast(ray, out hit, 10)) {
			GameObject zone = ZoneFor(color);
			if (zone != null) {
				isInSafeZone = hit.transform.IsChildOf(zone.transform);
			}
		}
		return isInSafeZone;
	}

	private GameObject ZoneFor(BallColor color) {
		switch (color) {
		case BallColor.Blue:
			return blueZone;
		case BallColor.Red:
			return redZone;
		case BallColor.Yellow:
			return yellowZone;
		case BallColor.Green:
			return greenZone;
		default:
			return null;
		} 
	}
}
