using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
	[SerializeField] Rigidbody2D player;
	Vector3 lastPosition;

	private void FixedUpdate()
	{
		if (player == null)
		{
			lastPosition = transform.position;
			return;
		}

		Vector2 deltaPosition = transform.position - lastPosition;
		lastPosition = transform.position;
		player.transform.position += (Vector3)deltaPosition;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.collider.tag == "Player")
		{
			player = collision.collider.attachedRigidbody;
			player.GetComponent<CharacterMover>().UseRigidbody = false;
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.collider.tag == "Player")
		{
			player.GetComponent<CharacterMover>().UseRigidbody = true;
			player = null;
		}
	}
}
