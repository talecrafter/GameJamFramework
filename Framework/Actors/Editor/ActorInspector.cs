using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using CraftingLegends.Framework;

[CustomEditor(typeof(Actor))]
public class ActorInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if (Application.isPlaying)
		{
			if (GUILayout.Button("Hit with Damage", GUILayout.Height(40f)))
			{
				DamageCharacter();
			}

			if (GUILayout.Button("Kill", GUILayout.Height(40f)))
			{
				KillCharacter();
			}
		}
	}

	private void KillCharacter()
	{
		Actor actor = target as Actor;
		actor.Kill();
	}

	private void DamageCharacter()
	{
		Actor actor = target as Actor;
		actor.ApplyDamage(2f);
	}
}