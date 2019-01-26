using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CrushDetector : MonoBehaviour
{
	[SerializeField] Transform spawnPoint;
	[SerializeField] Transform player;
	[SerializeField] SpriteRenderer blackOut;
	[SerializeField] float detectDepth = 3;
	[SerializeField] float blackOutStartTime = 1;
	[SerializeField] float respawnTime = 3;

	[SerializeField] UnityEvent OnRespawn;

	bool respawning;
	float crushingTime;
	RaycastHit2D[] hits = new RaycastHit2D[15];
	
	void Update()
    {
		if (respawning)
			return;

		Physics2D.RaycastNonAlloc(player.position, Vector2.up, hits, detectDepth);

		int count = 0;
		for (int i = 0; i < hits.Length; i++)
		{
			if (hits[i].collider != null)
				count++;
		}

		var color = blackOut.color;
		color.a = (blackOutStartTime - crushingTime) / (blackOutStartTime - respawnTime);
		blackOut.color = color;

		// If too many stuff pile on top..
		if (count >= 2)
		{
			crushingTime += Time.deltaTime;

			// ..for too long, respawn
			if (crushingTime > respawnTime)
			{
				crushingTime = 0;
				StartCoroutine(Respawn());
			}

			for (int i = 0; i < hits.Length; i++)
			{
				hits[i] = new RaycastHit2D();
			}
		}
		else if (crushingTime > 0)
			crushingTime -= Time.deltaTime;

		if (crushingTime > blackOutStartTime)
		{
			blackOut.transform.position = player.position;
		}

		print(count);
		
	}

	IEnumerator Respawn()
	{
		respawning = true;
		float time = 0.5f;
		var color = blackOut.color;
		player.position = spawnPoint.position;
		OnRespawn.Invoke();

		while (time > 0)
		{
			color = blackOut.color;
			color.a = time * 2;
			blackOut.color = color;
			time -= Time.deltaTime;

			yield return null;
		}

		color = blackOut.color;
		color.a = 0;
		blackOut.color = color;

		respawning = false;
	}
}
