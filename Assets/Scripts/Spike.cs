using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
	[SerializeField] Transform respawnPoint;
	[SerializeField] SpriteRenderer blackOut;
	[SerializeField] float fadeinTime = 3;
	[SerializeField] float fadeoutTime = 1;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			StartCoroutine(PlayerKilled(collision.gameObject));
			return;
		}

		if (collision.tag == "Builder")
			return;
		
		Destroy(collision);
	}

	IEnumerator PlayerKilled(GameObject player)
	{
		player.GetComponent<CharacterMover>().enabled = false;
		player.GetComponent<CharacterJumper>().enabled = false;

		float time = fadeinTime;
		while (time > 0)
		{
			time -= Time.deltaTime;

			var color = blackOut.color;
			color.a = 1 - (time / fadeinTime);
			blackOut.color = color;

			yield return null;
		}

		player.transform.position = respawnPoint.position;
        Camera.main.GetComponent<OneDirectionCamera>().ResetPosition();
        time = fadeoutTime;
		while (time > 0)
		{
			time -= Time.deltaTime;

			var color = blackOut.color;
			color.a = time / fadeoutTime;
			blackOut.color = color;

			yield return null;
		}

		player.GetComponent<CharacterMover>().enabled = true;
		player.GetComponent<CharacterJumper>().enabled = true;
	}
}
