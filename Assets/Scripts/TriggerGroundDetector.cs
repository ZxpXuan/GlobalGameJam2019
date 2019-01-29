using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using JTUtility.Platformer;

public class TriggerGroundDetector : BaseGroundDetector
{
	[SerializeField] protected new Collider2D collider;

	[SerializeField] protected List<Collider2D> touchedObjects;

	[SerializeField] protected LayerMask groundLayer;

	protected virtual void Awake()
	{
		if (collider == null)
			collider = GetComponent<Collider2D>();

		touchedObjects = new List<Collider2D>();
		Physics2D.queriesHitTriggers = false;
	}

	protected override bool IsDetectedGround()
	{
		return touchedObjects.Count > 0;
	}

	protected override bool IsOnGround()
	{
		return touchedObjects.Count > 0;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if ((1 << collision.gameObject.layer & groundLayer) == 0) return;
		touchedObjects.Add(collision);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if ((1 << collision.gameObject.layer & groundLayer) == 0) return;
		touchedObjects.Remove(collision);
	}
}
