﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StreamFillReaction : StreamReaction {

	[SerializeField]
	public List<Renderer> fillTargetRenderers;
	public Color unfilledTint = Color.grey;
	public Color filledTint = Color.white;
	[HideInInspector]
	public List<Color> baseColors;

	void Awake()
	{
		baseColors = new List<Color>();
		for (int i = 0; i < fillTargetRenderers.Count; i++)
		{
			baseColors.Add(fillTargetRenderers[i].material.color);
		}
	}

	void Start()
	{
		ApplyTint();
	}

	public override bool React(float actionRate)
	{
		bool reacted = base.React(actionRate);
		if (reacted)
		{
			ApplyTint();
		}
		return reacted;
	}

	private void ApplyTint()
	{
		Color tint = (unfilledTint * (1 - reactionProgress)) + (filledTint * reactionProgress);
		for (int i = 0; i < fillTargetRenderers.Count && i < baseColors.Count; i++)
		{
			fillTargetRenderers[i].material.color = baseColors[i] * tint;
		}
	}
}
