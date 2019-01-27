using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility.Platformer;

public class FallingDeath : MonoBehaviour
{
	[SerializeField] BaseGroundDetector detectoer;
	[SerializeField] SpriteRenderer blackOut;
	[SerializeField] Transform respawnPoint;
	[SerializeField] float fatalFallingDistance = 35;
	[SerializeField] float fadeinTime = 0;
	[SerializeField] float delayTime = 1;
	[SerializeField] float fadeoutTime = 2;

	float origYPos;

	private void Start()
	{
		detectoer.OnLanding += Detectoer_OnLanding;
		detectoer.OnTakingoff += Detectoer_OnTakingoff;
	}

	private void Detectoer_OnTakingoff()
	{
		origYPos = detectoer.transform.position.y;
	}

	private void Detectoer_OnLanding()
	{
		var distance = origYPos - detectoer.transform.position.y;
		if (distance >= fatalFallingDistance && this.enabled)
		{
			StartCoroutine(PlayerKilled(detectoer.gameObject));
		}
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

		var c = blackOut.color;
		c.a = 1;
		blackOut.color = c;

		player.transform.position = respawnPoint.position;
		Camera.main.GetComponent<OneDirectionCamera>().ResetPosition();

		time = delayTime;
		while (time > 0)
		{
			time -= Time.deltaTime;

			yield return null;
		}

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
