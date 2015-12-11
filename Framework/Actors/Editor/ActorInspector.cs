using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using CraftingLegends.Framework;

[CustomEditor(typeof(Actor2D))]
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
		Actor2D actor = target as Actor2D;
		actor.Kill();
	}

	private void DamageCharacter()
	{
		Actor2D actor = target as Actor2D;
		actor.ApplyDamage(2f);
	}
}