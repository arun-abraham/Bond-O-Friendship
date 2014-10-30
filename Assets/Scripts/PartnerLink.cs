﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PartnerLink : MonoBehaviour {
	public bool isPlayer = false;
	public Renderer headRenderer;
	public Renderer fillRenderer;
	public TrailRenderer trail;
	public PulseShot pulseShot;
	public float partnerLineSize = 0.25f;
	[HideInInspector]
	public SimpleMover mover;
	[HideInInspector]
	public Tracer tracer;
	public GameObject connectionPrefab;
	[SerializeField]
	public List<SimpleConnection> connections;
	[HideInInspector]
	public float fillScale = 1;
	public bool empty;
	public float minScale;
	public float maxScale;
	public float normalScale;
	public float preChargeScale;
	public float scaleRestoreRate;
	public float endChargeRestoreRate;
	public bool chargingPulse = false;
	public int volleysToConnect = 2;
	[SerializeField]
	public List<ContactPoint> contacts;
	public bool skipScaleUp = false;
	public float baseScaleTestRadius= 0.6f;
	public SphereCollider scaleTestCollider;

	void Awake()
	{
		if (mover == null)
		{
			mover = GetComponent<SimpleMover>();
		}
		if (tracer == null)
		{
			tracer = GetComponent<Tracer>();
		}
		if (pulseShot == null)
		{
			pulseShot = GetComponent<PulseShot>();
		}

		fillRenderer.material.color = headRenderer.material.color;

		contacts = new List<ContactPoint>();
	}
	
	void Update()
	{
		// Fill based on the amount drained by connection
		/*if (connections == null || connections.Count < 1)
		{
			fillScale = 0;
		}*/
		fillScale = 1;
		fillRenderer.transform.localScale = new Vector3(fillScale, fillScale, fillScale);

		// Record scale before starting charge.
		if (!chargingPulse && preChargeScale < transform.localScale.x)
		{
			preChargeScale = transform.localScale.x;
		}

		if (contacts.Count < 1 && !skipScaleUp)
		{
			// Restore scale up to normal, if below it and not charging.
			if (transform.localScale.x < normalScale && !chargingPulse)
			{
				// If scale is less than the scale before starting charge, scale up to that first.
				if (transform.localScale.x < preChargeScale)
				{
					float actualRestoreRate = endChargeRestoreRate * transform.localScale.x;
					transform.localScale = new Vector3(Mathf.Min(transform.localScale.x + actualRestoreRate * Time.deltaTime, preChargeScale), Mathf.Min(transform.localScale.y + actualRestoreRate * Time.deltaTime, normalScale), Mathf.Min(transform.localScale.z + actualRestoreRate * Time.deltaTime, normalScale));
				}
				else
				{
					float actualRestoreRate = scaleRestoreRate * transform.localScale.x;
					transform.localScale = new Vector3(Mathf.Min(transform.localScale.x + actualRestoreRate * Time.deltaTime, normalScale), Mathf.Min(transform.localScale.y + actualRestoreRate * Time.deltaTime, normalScale), Mathf.Min(transform.localScale.z + actualRestoreRate * Time.deltaTime, normalScale));
				}
			}

			// Stay within scale bounds.
			if (transform.localScale.x < minScale)
			{
				transform.localScale = new Vector3(minScale, minScale, minScale);
			}
			else if (transform.localScale.x > maxScale)
			{
				transform.localScale = new Vector3(maxScale, maxScale, maxScale);
			}

			scaleTestCollider.radius = 0.5f + ((baseScaleTestRadius - 0.5f) / transform.localScale.x);
		}
		skipScaleUp = false;

		trail.startWidth = transform.localScale.x;
	}

	private void OnTriggerEnter(Collider other)
	{
		// If colliding with a pulse, accept it.
		if (other.gameObject.tag == "Pulse")
		{
			MovePulse pulse = other.GetComponent<MovePulse>();
			if (pulse != null && (pulse.creator == null || pulse.creator != pulseShot))
			{
				if (contacts.Count < 1)
				{
					transform.localScale += new Vector3(pulse.capacity, pulse.capacity, pulse.capacity);
				}
				pulseShot.volleys = 1;
				if (pulse.volleyPartner != null && pulse.volleyPartner == pulseShot)
				{
					pulseShot.volleys = pulse.volleys;
				}
				if (pulseShot.volleys >= volleysToConnect)
				{
					bool connectionAlreadyMade = false;
					for (int i = 0; i < connections.Count && !connectionAlreadyMade; i++)
					{
						if ((connections[i].attachment1.partner == this && connections[i].attachment2.partner == pulse.creator.partnerLink) || (connections[i].attachment2.partner == this && connections[i].attachment1.partner == pulse.creator.partnerLink))
						{
							connectionAlreadyMade = true;
						}
					}
					if (!connectionAlreadyMade)
					{
						SimpleConnection newConnection = ((GameObject)Instantiate(connectionPrefab, Vector3.zero, Quaternion.identity)).GetComponent<SimpleConnection>();
						connections.Add(newConnection);
						pulse.creator.partnerLink.connections.Add(newConnection);
						newConnection.AttachPartners(pulse.creator.partnerLink, this);
					}
				}
				pulseShot.lastPulseAccepted = pulse.creator;
				Destroy(pulse.gameObject);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		bool collidingConnection = false;
		/*for (int i = 0; i < connections.Count; i++)
		{
			if (connections[i].bondCollider.gameObject == collision.collider.gameObject || connections[i].Shield.gameObject != collision.collider.gameObject)
			{
				collidingConnection = true;
			}
		}
		Debug.Log(collision.collider.gameObject.name + " " + collision.contacts.Length);*/
		if (collision.collider.gameObject != gameObject && !collidingConnection)
		{
			for (int i = 0; i < collision.contacts.Length; i++)
			{
				contacts.Add(collision.contacts[i]);
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{

		if (collision.collider.gameObject != gameObject)
		{
			
			for (int i = 0; i < collision.contacts.Length; i++)
			{
				for (int j = 0; j < contacts.Count; j++)
				{
					if (contacts[j].otherCollider == collision.contacts[i].otherCollider)
					{
						contacts.RemoveAt(j);
					}
				}
			}
		}
	}
}
