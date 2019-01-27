using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBuilders : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		var builder = collision.GetComponent<Builder>();
		if (builder)
			builder.Ready = true;
	}
}
