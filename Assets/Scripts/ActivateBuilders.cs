using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBuilders : MonoBehaviour
{
	Collider2D collider;

	private void Awake()
	{
		print("Initialize");
		collider = GetComponent<Collider2D>();
	}

	private void Update()
	{
		collider.gameObject.SetActive(true);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		var builder = collision.GetComponent<Builder>();
		if (builder)
			builder.Ready = true;
	}

	private void OnDestroy()
	{
		print("Destroy");
	}
}
