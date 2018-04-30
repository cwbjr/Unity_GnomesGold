//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

// BEGIN 2d_resettable
using UnityEngine.Events;


public class Resettable : MonoBehaviour {


	public UnityEvent onReset;


	public void Reset() {

		onReset.Invoke();
	}
}
