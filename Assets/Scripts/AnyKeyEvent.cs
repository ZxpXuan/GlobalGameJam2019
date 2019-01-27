using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnyKeyEvent : MonoBehaviour
{
	[SerializeField] bool triggerOnce;
	[SerializeField] UnityEvent OnAnyKeyPressed;

	bool triggered;


	private void Update()
	{
		if (triggered && triggerOnce) return;
		if (Input.anyKeyDown)
		{
			OnAnyKeyPressed.Invoke();
			triggered = true;
		}
	}
}
