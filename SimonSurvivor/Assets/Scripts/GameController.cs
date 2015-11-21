﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class GameController : MonoBehaviour {
	public Text sequenceHelper;
	public TextMesh scoreKeeper;
	public float trapDoorClosingTime = 1f;
	public float trapDoorOpeningTime = 1f;
	public float trapDoorWaitingTime = 2f;
	public float sequenceCountDown = 5f;

    private PlayerController playerController;
	private GameObject blueZone;
	private GameObject greenZone;
	private GameObject redZone;
	private GameObject yellowZone;
	private Camera mainCamera;

    private BallSequenceGenerator generator;
	private int score;
	private IList<BallColor> remainingSequence;

	// Use this for initialization
	void Start () {
		blueZone = GameObject.Find("Blue Zone");
		greenZone = GameObject.Find("Green Zone");
		redZone = GameObject.Find("Red Zone");
		yellowZone = GameObject.Find("Yellow Zone");
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
		mainCamera = Camera.main;
		ResetSequence();
		InputTracking.Recenter ();
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Backspace)) {
			ResetSequence();
		} 
		if (Input.GetKeyDown (KeyCode.Space)) {
			IncrementSequence();
		}
		if (Input.GetKeyDown (KeyCode.Delete)) {
			InputTracking.Recenter ();
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
		if (!PlayerIsInSafeZone (color)) {
			playerController.Fall ();
		} 
    }

    private void CloseZones()
    {
		var availableColors = Enum.GetValues(typeof(BallColor)).Cast<BallColor>();
		var allZones = availableColors.Select(c => ZoneFor(c));
		foreach (GameObject zone in allZones) {
			zone.GetComponent<ZoneController>().isOpening = false;
		}
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

	private void ResetSequence() {
		score = 0;
		generator = new BallSequenceGenerator().addNewBall();
		PrepareTrapDoorSequence();
	}
	
	private void IncrementSequence() {
		score++;
		generator = generator.addNewBall();
		PrepareTrapDoorSequence();
	}

	private void UpdateSequenceHelper() {
		sequenceHelper.text = "" + generator.sequence.Select(c => c.ToString("F"))
			                                         .Aggregate((c, n) => c + "-" + n);
	}

	
	private void UpdateScoreKeeper() {
		scoreKeeper.text = score.ToString ();
	}

	private void PrepareTrapDoorSequence() {
		UpdateSequenceHelper ();
		UpdateScoreKeeper ();
		remainingSequence = generator.sequence.ToList<BallColor> ();
		InvokeRepeating("UpdateTrapDoors", sequenceCountDown, trapDoorOpeningTime + trapDoorClosingTime + trapDoorWaitingTime);
	}

	private void UpdateTrapDoors() {
		if (remainingSequence.Any ()) {
			OpenAllZonesOtherThan (remainingSequence.First<BallColor> ());
			remainingSequence = remainingSequence.Skip (1).ToList<BallColor> ();
			Invoke ("CloseZones", trapDoorClosingTime);
		} else {
			CancelInvoke();
			IncrementSequence();
		}
	}
}
