using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public static class StringExtensions
{
    public static string Color(this string s, Color c)
	{		
		string cHex = ColorUtility.ToHtmlStringRGBA(c);
		return "<color=#" + cHex + ">" + s + "</color>";
	}

	public static string Bold(this string s)
	{
		return "<b>" + s + "</b>";
	}

	public static string Italic(this string s)
	{
		return "<i>" + s + "</i>";
	}
}