﻿using UnityEngine;
using System.Collections;

public class RingCollision : MonoBehaviour {

    public GameObject creator;
	public ParticleSystem ringCollisionParticle;
	private ParticleSystem collidedParticle;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter (Collider col) {
		if (col.gameObject != creator)
		{
			collidedParticle = (ParticleSystem)Instantiate(ringCollisionParticle);
			collidedParticle.transform.position = col.transform.position;
			if (col.name == "Player 1")
				collidedParticle.startColor = col.GetComponent<PartnerLink>().headRenderer.material.color;
			else if (col.name == "Player 2")
				collidedParticle.startColor = col.GetComponent<PartnerLink>().headRenderer.material.color;
			else
				collidedParticle.startColor = col.renderer.material.color;
			Destroy(collidedParticle.gameObject, 1.0f);
		}
	}
}
