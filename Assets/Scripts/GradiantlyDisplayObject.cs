using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradiantlyDisplayObject : MonoBehaviour
{
	[SerializeField] float displaySpeed;
	[SerializeField] List<Transform> objects;

	[SerializeField] int nextIndex = 0;
	[SerializeField] float currentRange;
	[SerializeField] float fadeInDuration = 2;

	private void Awake()
	{
		objects.Sort((b1, b2) =>
		{
			var delta1 = b1.transform.position - transform.position;
			var delta2 = b2.transform.position - transform.position;
			return
			delta1.sqrMagnitude > delta2.sqrMagnitude ? 1 :
				(delta1.sqrMagnitude < delta2.sqrMagnitude ? -1 : 0);
		});

		for (int i = nextIndex; i < objects.Count; i++)
		{
			var renderers = objects[i].GetComponentsInChildren<SpriteRenderer>();
			var colors = new Color[renderers.Length];

			for (int j = 0; j < renderers.Length; j++)
			{
				colors[j] = renderers[j].color;
				colors[j].a = 0;
				renderers[j].color = colors[j];
			}
		}

		this.enabled = false;
	}

	private void Update()
	{
		currentRange += Time.deltaTime * displaySpeed;

		for (int i = nextIndex; i < objects.Count; i++)
		{
			Vector2 delta = objects[i].transform.position - transform.position;
			if (delta.sqrMagnitude > currentRange * currentRange)
				return;

			nextIndex = i + 1;
			StartCoroutine(StartDisplay(objects[i], fadeInDuration));
		}
	}

	void StartDisplay()
	{
		currentRange = 0;
		this.enabled = true;
	}

	IEnumerator StartDisplay(Transform renderer, float duration)
	{
		float time = duration;

		var renderers = renderer.GetComponentsInChildren<SpriteRenderer>();
		var colors = new Color[renderers.Length];

		while (time > 0)
		{
			time -= Time.deltaTime;

			var perc = Mathf.Lerp(1, 0, time / duration);
			for (int i = 0; i < renderers.Length; i++)
			{
				colors[i] = renderers[i].color;
				colors[i].a = perc;
				renderers[i].color = colors[i];
			}

			yield return null;
		}

		for (int i = 0; i < renderers.Length; i++)
		{
			colors[i] = renderers[i].color;
			colors[i].a = 1;
			renderers[i].color = colors[i];
		}

	}
}
