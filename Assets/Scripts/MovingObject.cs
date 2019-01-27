using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
	[SerializeField] float duration;
	[SerializeField] Vector3 startPosition;
	[SerializeField] Vector3 endPosition;

	float time;

	private void Update()
	{
		transform.position = Vector3.Lerp(startPosition, endPosition, time);
		time += Time.deltaTime / duration;
		if (time < 1)
			return;

		transform.position = endPosition;
		this.enabled = false;
	}
}
