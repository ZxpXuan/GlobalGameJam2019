using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
	[SerializeField] float delay;

	private void Start()
	{
		Invoke("Destroy", delay);
	}

	void Destroy()
	{
		GameObject.Destroy(this.gameObject);
	}
}
