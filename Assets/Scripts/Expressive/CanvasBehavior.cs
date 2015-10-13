﻿using UnityEngine;
using System.Collections;

public class CanvasBehavior : MonoBehaviour {

	public GameObject paintCopier;
	public CanvasBehavior pairedCanvas;
	public GameObject player1;
	public GameObject player2;
	public GameObject pairedPlayer;
	private Color canvasColor;
	private float alpha;
	public bool changeColor = true;
	public float maxCheckDistance = 2;
	public float minPaintRadius = 1;
	public float maxPaintRadius = 5;
	[HideInInspector]
	public CapsuleCollider canvasCollider;

	public GameObject pal1;
	public GameObject pal2;
	public GameObject pal3;
	public GameObject pal4;

	private float colorJitter;

	public float r1;
	public float g1;
	public float b1;
	public float a1;

	public float r2;
	public float g2;
	public float b2;
	public float a2;

	public float r3;
	public float g3;
	public float b3;
	public float a3;

	public float r4;
	public float g4;
	public float b4;
	public float a4;

	//[HideInInspector]
	public Vector3 mirrorDistance;

	[HideInInspector]
	public PaintAndNodeCollisionTest nodeCollisionTest;

	//public bool isMirror;
	//public ClusterNodePuzzle puzzleToReveal;


	// Use this for initialization
	
	void Start () {
		alpha = 0.0f;
		canvasColor = new Color(0.0f,0.0f,0.0f,alpha);
		if (changeColor)
		{
			gameObject.GetComponent<Renderer>().material.color = canvasColor;
		}
		if (pairedCanvas != null)
			mirrorDistance = transform.position - pairedCanvas.transform.position;

		nodeCollisionTest = GetComponent<PaintAndNodeCollisionTest>();
		canvasCollider = GetComponent<CapsuleCollider>();
	}
	
	// Update is called once per frame
	void Update () {

		pal1.GetComponent<Palette>().r = r1;
		pal1.GetComponent<Palette>().g = g1;
		pal1.GetComponent<Palette>().b = b1;
		pal1.GetComponent<Palette>().a = a1;

		pal2.GetComponent<Palette>().r = r2;
		pal2.GetComponent<Palette>().g = g2;
		pal2.GetComponent<Palette>().b = b2;
		pal2.GetComponent<Palette>().a = a2;

		pal3.GetComponent<Palette>().r = r3;
		pal3.GetComponent<Palette>().g = g3;
		pal3.GetComponent<Palette>().b = b3;
		pal3.GetComponent<Palette>().a = a3;

		pal4.GetComponent<Palette>().r = r4;
		pal4.GetComponent<Palette>().g = g4;
		pal4.GetComponent<Palette>().b = b4;
		pal4.GetComponent<Palette>().a = a4;



		if (changeColor)
		{
			canvasColor = new Color(0.8f, 0.9f, 0.8f, alpha);
			gameObject.GetComponent<Renderer>().material.color = canvasColor;
			if (alpha < 1.0f)
			{
				alpha += Time.deltaTime * 0.5f;
			}
		}
		if(pairedPlayer != null && paintCopier != null)
		{
			paintCopier.transform.position = pairedPlayer.transform.position + mirrorDistance;
		}
	}

	void OnTriggerEnter (Collider collide)
	{
		if(paintCopier == null)
		{
			if(collide.gameObject.name == "Player 1")
			{
				player1 = collide.gameObject;

				//Assign canvas variables to player
				Paint paintScript = player1.GetComponent<Paint>();
				paintScript.painting = true;
				paintScript.paintCanvas = this;


				pairedCanvas.pairedPlayer = player1;
				//Assign canvas variables to pairedCanvas' copier
				paintScript = pairedCanvas.paintCopier.GetComponent<Paint>();
				paintScript.painting = true;
				paintScript.paintCanvas = pairedCanvas;
			}
			if(collide.gameObject.name == "Player 2")
			{
				player2 = collide.gameObject;

				//Assign canvas variables to paint
				Paint paintScript = player2.GetComponent<Paint>();
				paintScript.painting = true;
				paintScript.paintCanvas = this;

				pairedCanvas.pairedPlayer = player2;
				//Assign canvas variables to pairedCanvas' copier
				paintScript = pairedCanvas.paintCopier.GetComponent<Paint>();
				paintScript.painting = true;
				paintScript.paintCanvas = pairedCanvas;
			}
		}
	}

	void OnTriggerExit (Collider collide)
	{
		if(paintCopier == null)
		{
			if(collide.gameObject.name == "Player 1")
			{
				player1 = collide.gameObject;

				//Assign canvas variables to paint
				Paint paintScript = player1.GetComponent<Paint>();
				paintScript.painting = false;
				paintScript.paintCanvas = null;

				pairedCanvas.pairedPlayer = null;
				//De-reference canvas variables from pairedCanvas' copier
				paintScript = pairedCanvas.paintCopier.GetComponent<Paint>();
				paintScript.painting = false;
				paintScript.paintCanvas = null;
			}
			if(collide.gameObject.name == "Player 2")
			{
				player2 = collide.gameObject;

				//Assign canvas variables to paint
				Paint paintScript = player2.GetComponent<Paint>();
				paintScript.painting = false;
				paintScript.paintCanvas = null;

				pairedCanvas.pairedPlayer = null;
				//De-reference canvas variables from pairedCanvas' copier
				paintScript = pairedCanvas.paintCopier.GetComponent<Paint>();
				paintScript.painting = false;
				paintScript.paintCanvas = null;
			}
		}
	}
}
