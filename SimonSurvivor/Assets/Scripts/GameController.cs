using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class GameController : MonoBehaviour {
	public Text sequenceHelper;
	public UILabel scoreKeeper;
	public UILabel highScoreKeeper;
    public UILabel tutorialText;
	public float intervalBetweenSequences = 1f;
	public float pipeLightingTime = 1f;
	public float pipeIntervalTime = 1f;
	public float trapDoorClosingTime = 1f;
	public float trapDoorOpeningTime = 1f;
	public float trapDoorWaitingTime = 2f;
	public float sequenceCountDown = 5f;
	public float speedFactor = 0.95f;
	public float currentSpeedFactor = 1f;
    public float minimumSpeedFactor = 0.4f;

    private PlayerController playerController;
	private GameObject blueZone;
	private GameObject greenZone;
	private GameObject redZone;
	private GameObject yellowZone;
	private GameObject pipeA;
	private GameObject pipeB;
	private GameObject pipeC;
	private GameObject pipeD;
	private GameObject sphereA;
	private GameObject sphereB;
	private GameObject sphereC;
	private GameObject sphereD;
	private Camera mainCamera;

    private BallSequenceGenerator ballGenerator;
	private PipeSequenceGenerator pipeGenerator;
	private int score;
	private int highScore = 0;
	private IList<Pipe> remainingPipes;
	private IList<BallColor> remainingSequence;

	// Use this for initialization
	void Start () {
		blueZone = GameObject.Find("Blue Zone");
		greenZone = GameObject.Find("Green Zone");
		redZone = GameObject.Find("Red Zone");
		yellowZone = GameObject.Find("Yellow Zone");
		pipeA = GameObject.Find("Pipe A");
		pipeB = GameObject.Find("Pipe B");
		pipeC = GameObject.Find("Pipe C");
		pipeD = GameObject.Find("Pipe D");
		sphereA = pipeA.transform.Find("Boule").gameObject;
		sphereB = pipeB.transform.Find("Boule").gameObject;
		sphereC = pipeC.transform.Find("Boule").gameObject;
		sphereD = pipeD.transform.Find("Boule").gameObject;

        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
		mainCamera = Camera.main;
		InputTracking.Recenter ();
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Backspace))
        {
            ResetSequence();
		} 
		if (Input.GetKeyDown (KeyCode.Space))
        {
            tutorialText.enabled = false;
            ResetSequence();
            Start();
		}
		if (Input.GetKeyDown (KeyCode.Delete)) {
			InputTracking.Recenter ();
		}
		if (playerController.dead) {
			playerController.Reset();
            CancelInvoke();
            CloseZones();
            TurnOffPipes();
            tutorialText.enabled = true;
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
		} else if (!remainingSequence.Any()) {
			UpdateScore();
		}
    }

	private void UpdateScore() {
		score++;
		scoreKeeper.text = score.ToString ();	
		if (score > highScore)
		{
			highScore = score;
			highScoreKeeper.text = score.ToString ();
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

	private Color ColorFor(BallColor color) {
		switch (color) {
		case BallColor.Blue:
			return Color.blue;
		case BallColor.Red:
			return Color.red;
		case BallColor.Yellow:
			return Color.yellow;
		case BallColor.Green:
			return Color.green;
		default:
			return Color.white;
		} 
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

	private GameObject PipeFor(Pipe pipe) {
		switch (pipe) {
		case Pipe.A:
			return pipeA;
		case Pipe.B:
			return pipeB;
		case Pipe.C:
			return pipeC;
		case Pipe.D:
			return pipeD;
		default:
			return null;
		} 
	}

	private GameObject SphereFor(Pipe pipe) {
		switch (pipe) {
		case Pipe.A:
			return sphereA;
		case Pipe.B:
			return sphereB;
		case Pipe.C:
			return sphereC;
		case Pipe.D:
			return sphereD;
		default:
			return null;
		} 
	}

	private void ResetSequence() {
		CancelInvoke();
		CloseZones();
		TurnOffPipes ();
		score = 0;
		scoreKeeper.text = score.ToString ();
		pipeGenerator = new PipeSequenceGenerator().addNewPipe();
		ballGenerator = new BallSequenceGenerator().addNewBall();
		PlaySequenceToRemember();
	}
	
	private void IncrementSequence() {
		pipeGenerator = pipeGenerator.addNewPipe();
		ballGenerator = ballGenerator.addNewBall();
		PlaySequenceToRemember();
	}

	private void PlaySequenceToRemember() {
		remainingPipes = pipeGenerator.sequence.ToList<Pipe>();
		remainingSequence = ballGenerator.sequence.ToList<BallColor>();
		currentSpeedFactor = Mathf.Pow (speedFactor, remainingSequence.Count - 1);
        if (currentSpeedFactor < minimumSpeedFactor)
        {
            currentSpeedFactor = minimumSpeedFactor;
        }
		InvokeRepeating("UpdatePipes", intervalBetweenSequences * currentSpeedFactor,
		                (pipeLightingTime + pipeIntervalTime) * currentSpeedFactor);
	}

	private void UpdatePipes() {
		if (remainingPipes.Any()) {
			LightPipeWithColor (remainingPipes.First<Pipe> (), remainingSequence.First<BallColor> ());
			remainingPipes = remainingPipes.Skip (1).ToList<Pipe> ();
			remainingSequence = remainingSequence.Skip (1).ToList<BallColor> ();
			if (remainingPipes.Count () >= 1) {
				Invoke ("TurnOffPipes", pipeLightingTime * currentSpeedFactor);
			} else {
				CancelInvoke();
				Invoke ("TurnOffPipes", pipeLightingTime);			
				Invoke ("UpdatePipes", pipeLightingTime + pipeIntervalTime);
			}
		} else {
			CancelInvoke();
			PrepareTrapDoorSequence();
		}
	}

	private void LightPipeWithColor(Pipe pipe, BallColor ballColor) {
		Animator anim = PipeFor(pipe).GetComponent<Animator>();
		anim.SetInteger("couleur", (int)ballColor);
	}

	private void TurnOffPipes() {
		var pipeTypes = Enum.GetValues(typeof(Pipe)).Cast<Pipe>();
		foreach (Pipe pipe in pipeTypes) {
			Animator anim = PipeFor(pipe).GetComponent<Animator>();
			anim.SetInteger("couleur", 99);
		}
	}

	private void UpdateSequenceHelper() {
		sequenceHelper.text = "" + ballGenerator.sequence.Select(c => c.ToString("F"))
			                                             .Aggregate((c, n) => c + "-" + n);
	}

	private void PrepareTrapDoorSequence() {
		UpdateSequenceHelper ();
		remainingSequence = ballGenerator.sequence.ToList<BallColor> ();
		InvokeRepeating("UpdateTrapDoors", sequenceCountDown * currentSpeedFactor,
		                (trapDoorOpeningTime + trapDoorClosingTime + trapDoorWaitingTime) * currentSpeedFactor);
	}

	private void UpdateTrapDoors() {
		if (remainingSequence.Any ()) {
			var ballColor = remainingSequence.First<BallColor> ();
			remainingSequence = remainingSequence.Skip (1).ToList<BallColor> ();
			OpenAllZonesOtherThan (ballColor);
			Invoke ("CloseZones", trapDoorClosingTime * currentSpeedFactor);
		} else {
			CancelInvoke();
			IncrementSequence();
		}
	}
}
