﻿using UnityEngine;
using System.Collections;

public class FloatMoving : MonoBehaviour {
	public PartnerLink partnerLink;
	public SimpleMover mover;
	public FluffSpawn fluffSpawn;
	public MovementStats startingStats;
	public MovementStats loneFloatStats;
	public MovementStats perConnectionFloatBonus;
	public int maxConnectionBonuses;
	public LayerMask ignoreLayers;
	private bool wasFloating = false;
	public bool Floating
	{
		get { return wasFloating; }
	}

	void Awake()
	{
		if (mover == null)
		{
			mover = GetComponent<SimpleMover>();
		}
		if (partnerLink == null)
		{
			partnerLink = GetComponent<PartnerLink>();
		}
		if (fluffSpawn == null)
		{
			fluffSpawn = GetComponent<FluffSpawn>();
		}
	}

	void Start()
	{
		startingStats = new MovementStats();
		startingStats.acceleration = mover.acceleration;
		startingStats.handling = mover.handling;
		if (mover.body != null)
		{
			startingStats.bodyDrag = mover.body.drag;
		}
		startingStats.bodylessDampening = mover.bodylessDampening;
		startingStats.sideTrailTime = partnerLink.leftTrail.time;
		startingStats.midTrailTime = partnerLink.midTrail.time;
	}

	void Update()
	{

		RaycastHit hit;
		if(Physics.Raycast(transform.position, Vector3.forward, out hit, Mathf.Infinity, ~ignoreLayers))
		{
			if (wasFloating)
			{
				mover.acceleration = startingStats.acceleration;
				mover.handling = startingStats.handling;
				if (mover.body != null)
				{
					mover.body.drag = startingStats.bodyDrag;
				}
				mover.bodylessDampening = startingStats.bodylessDampening;
				partnerLink.leftTrail.time = partnerLink.rightTrail.time = startingStats.sideTrailTime;
				partnerLink.midTrail.time = partnerLink.rightTrail.time = startingStats.sideTrailTime;
				wasFloating = false;
			}
		}
		else if (!wasFloating)
		{
			wasFloating = true;
			ApplyFloatStats();

			// Ensure that players have fluffs while floating.
			if (fluffSpawn.naturalFluffCount <= 0)
			{
				fluffSpawn.naturalFluffCount = 1;
			}
		}
	}

	private void ApplyFloatStats()
	{
		if (wasFloating)
		{
			mover.acceleration = loneFloatStats.acceleration;
			mover.handling = loneFloatStats.handling;
			if (mover.body != null)
			{
				mover.body.drag = loneFloatStats.bodyDrag;
			}
			mover.bodylessDampening = loneFloatStats.bodylessDampening;
			partnerLink.leftTrail.time = partnerLink.rightTrail.time = loneFloatStats.sideTrailTime;
			partnerLink.midTrail.time = partnerLink.rightTrail.time = loneFloatStats.midTrailTime;

			int connectionBonusCount = Mathf.Min(partnerLink.connectionAttachable.connections.Count, maxConnectionBonuses);
			if (connectionBonusCount > 0)
			{
				mover.acceleration += perConnectionFloatBonus.acceleration * connectionBonusCount;
				mover.handling += perConnectionFloatBonus.handling * connectionBonusCount;
				if (mover.body != null)
				{
					mover.body.drag += perConnectionFloatBonus.bodyDrag * connectionBonusCount;
				}
				mover.bodylessDampening += perConnectionFloatBonus.bodylessDampening * connectionBonusCount;
				partnerLink.leftTrail.time = partnerLink.rightTrail.time += perConnectionFloatBonus.sideTrailTime * connectionBonusCount;
				partnerLink.midTrail.time = partnerLink.rightTrail.time += perConnectionFloatBonus.midTrailTime * connectionBonusCount;
			}
		}
	}

	private void ConnectionMade(ConnectionAttachable connectionPartner)
	{
		ApplyFloatStats();
	}

	private void ConnectionBroken(ConnectionAttachable disconnectedPartner)
	{
		ApplyFloatStats();
	}
}

[System.Serializable]
public class MovementStats
{
	public float acceleration;
	public float handling;
	public float bodyDrag;
	public float bodylessDampening;
	public float sideTrailTime;
	public float midTrailTime;
}
