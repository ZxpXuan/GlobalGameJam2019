using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingClean : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag != "Player")
			other.gameObject.SetActive(false);
	}
}
