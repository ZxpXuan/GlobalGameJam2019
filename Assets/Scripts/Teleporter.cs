using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
	[SerializeField] Transform targetPoint;
	[SerializeField] float teleportDelay = 0;

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (teleportDelay > 0)
			StartCoroutine(Teleport(collider));
		else
			collider.transform.position = targetPoint.position;
	}

	IEnumerator Teleport(Collider2D collider)
	{
		yield return new WaitForSeconds(teleportDelay);

		collider.transform.position = targetPoint.position;
	}
}
