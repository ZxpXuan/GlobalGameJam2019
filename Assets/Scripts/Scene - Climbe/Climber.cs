using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public class Climber : MonoBehaviour
{
	[SerializeField] protected Transform target;
	[SerializeField, Range(0f, 1f)] protected float pushPlayerChance;
	[SerializeField, MinMaxSlider(0, 5)] protected Vector2 climbingForce;
	[SerializeField, MinMaxSlider(0, 5)] protected Vector2 chargeDelay;

	new protected Rigidbody2D rigidbody;

	protected float delay = 0;
	protected bool charged;

	protected virtual void Awake()
	{
		charged = Random.value > 0.5f;
		delay = chargeDelay.RandomBetween();
		rigidbody = GetComponent<Rigidbody2D>();
	}

	protected virtual void Update()
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

	protected virtual void Charge()
	{
		var toTarget = (target.position + Vector3.up * 3) - transform.position;
		toTarget.Normalize();

		rigidbody.AddForce(climbingForce.RandomBetween() * 1000 * toTarget, ForceMode2D.Impulse);
	}

	protected virtual void StepBack()
	{
		var toTarget = target.position - transform.position;
		toTarget.Normalize();

		rigidbody.AddForce(climbingForce.RandomBetween() * 800 * -toTarget, ForceMode2D.Impulse);
	}

	protected virtual void OnCollisionEnter2D(Collision2D collision)
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

		if (Vector3.Dot(toPlayer, toTarget) > 0.7f) return;

		rigidbody.AddForce(climbingForce.RandomBetween() * 700 * toPlayer, ForceMode2D.Impulse);
	}
}
