using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneDirectionCamera : MonoBehaviour
{
	[SerializeField] Transform barrier;
	[SerializeField] float speed;
	[SerializeField] Transform target;

	Camera camera;

	private void Start()
	{
		camera = GetComponent<Camera>();
	}

	private void FixedUpdate()
	{
		Follow();
		MoveBarrier();
	}

	private void Follow()
	{
		var targetPos = target.position;

		if (targetPos.x < camera.transform.position.x)
		{
			targetPos.x = camera.transform.position.x;
		}
		var pos = Vector3.Lerp(camera.transform.position, targetPos, Time.deltaTime * speed);
		pos.z = camera.transform.position.z;
		camera.transform.position = pos;
	}

	private void MoveBarrier()
	{
		var leftMostPos = camera.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));
		barrier.position = leftMostPos;
	}

	public void ResetPosition()
	{
		var pos = target.position;
		pos.z = camera.transform.position.z;
		camera.transform.position = pos;
	}

	public void ChangeOrthographicSize(float newSize, float duration)
	{
		if (duration > 0)
		{
			StartCoroutine(ChangeSize(newSize, duration));
		}
		else
		{
			camera.orthographicSize = newSize;
		}
	}

	IEnumerator ChangeSize(float newSize, float duration)
	{
		float time = duration;
		float origSize = camera.orthographicSize;

		while (time > 0)
		{
			time -= Time.deltaTime;
			camera.orthographicSize = Mathf.Lerp(newSize, origSize, time / duration);
			yield return null;
		}

		camera.orthographicSize = newSize;
	}
}
