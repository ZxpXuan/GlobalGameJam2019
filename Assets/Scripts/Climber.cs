using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public class Climber : MonoBehaviour
{
	[SerializeField] Transform target;
	[SerializeField, Range(0f, 1f)] float pushPlayerChance;
	[SerializeField, MinMaxSlider(0, 5)] Vector2 climbingForce;
	[SerializeField, MinMaxSlider(0, 5)] Vector2 chargeDelay;

	new Rigidbody2D rigidbody;

	float delay = 0;
	bool charged;

	private void Awake()
	{
		charged = Random.value > 0.5f;
		delay = chargeDelay.RandomBetween();
		rigidbody = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		delay -= Time.deltaTime;
		if (delay > 0) return;

		if (charged)
		{
			StepBack();
		}
		else
		{
			Charge();
		}

		charged = !charged;
		delay = chargeDelay.RandomBetween();
	}

	void Charge()
	{
		var toTarget = target.position - transform.position;
		toTarget.Normalize();

		rigidbody.AddForce(climbingForce.RandomBetween() * 1000 * toTarget, ForceMode2D.Impulse);
	}

	void StepBack()
	{
		var toTarget = target.position - transform.position;
		toTarget.Normalize();

		rigidbody.AddForce(climbingForce.RandomBetween() * 1000 * -toTarget, ForceMode2D.Impulse);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.collider.tag != "Player")
			return;

		if (Random.value > pushPlayerChance)
		{
			return;
		}

		var toPlayer = collision.collider.transform.position - transform.position;
		toPlayer.Normalize();
		var toTarget = target.position - transform.position;
		toTarget.Normalize();

		if (Vector3.Dot(toPlayer, toTarget) > 0.5f) return;

		rigidbody.AddForce(climbingForce.RandomBetween() * 500 * toPlayer, ForceMode2D.Impulse);
	}
}
