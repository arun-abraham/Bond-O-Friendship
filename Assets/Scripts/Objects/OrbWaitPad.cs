﻿using UnityEngine;
using System.Collections;

public class OrbWaitPad : WaitPad {

	public Material activatedSphereColor;
	private int triggersLit = 0;
	public int maxTriggers;
	private bool fullyLit;
	public GameObject[] activationSpheres;
	public ParticleSystem activatedParticle;
	public GameObject optionalGate;

	override protected void Start () {
		red = 0.1f;
		turnTime = 0.3f;
		activationSpheres = new GameObject[transform.parent.childCount];
		for(int i = 0; i < transform.parent.childCount; i++)
		{
			if(transform.parent.GetChild(i).name == "Activation Sphere" && activationSpheres[i] == null)
					activationSpheres[i] = transform.parent.GetChild(i).gameObject;
		}
		if (activatedParticle != null)
		{
			activatedParticle.Stop();
		}
	}

	override protected void Update()
	{
		mycolor = new Color(red,0.3f,0.5f);
		GetComponent<Renderer>().material.color = mycolor;

	if(pOonPad == true && pTonPad == true && fullyLit)
		{
			//renderer.material.color = Color.magenta;
			if(red < 1.0f)
			red += Time.deltaTime*turnTime;
		}
		if(pOonPad == false || pTonPad == false)
		{
			if(red > 0.1f)
			red -= Time.deltaTime;
		}
		if(red >= 1)
		{
			activated = true;
		}
		if(activated)
		{
			//print ("activated");
		}
	}
	void OnTriggerEnter(Collider collide)
	{
		if(collide.name == "Blossom")
		{
			for(int i = 0; i < transform.parent.childCount; i++)
			{
				if(fullyLit == false && activationSpheres[i] != null)
				{
					DepthMaskHandler slotDepthMask = activationSpheres[i].GetComponent<DepthMaskHandler>();
					if (slotDepthMask != null)
					{
						slotDepthMask.CreateDepthMask();
					}

					Destroy(collide.gameObject);
					activationSpheres[i].GetComponent<Renderer>().material = activatedSphereColor;
					ParticleSystem tempParticle = (ParticleSystem)Instantiate(activatedParticle);
					tempParticle.transform.position = activationSpheres[i].transform.position;
					activationSpheres[i] = null;
					//activatedParticle.Play();
					if(optionalGate != null)
						Destroy(optionalGate);
					break;
				}
			}
			triggersLit++;
			if(triggersLit == maxTriggers)
				fullyLit = true;
		}

		if(collide.gameObject.name == "Player 1")
		{
			pOonPad = true;
			//print("1");
		}
		if(collide.gameObject.name == "Player 2")
		{
			pTonPad = true;
			//print ("2");
		}

	}
	void OnTriggerExit(Collider collide)
	{
		if(collide.gameObject.name == "Player 1")
		{
			pOonPad = false;
		}
		if(collide.gameObject.name == "Player 2")
		{
			pTonPad = false;
		}
	}
}