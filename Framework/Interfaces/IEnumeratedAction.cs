using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public interface IEnumeratedAction
{
	IEnumerator Execute();
}