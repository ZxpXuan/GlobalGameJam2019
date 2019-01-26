using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roller : MonoBehaviour
{
	[SerializeField] float maxSpeed;
	[SerializeField] ParticleSystem particle;

	new Rigidbody2D rigidbody;
	new Collider2D collider;

	bool particlePaused;

	ContactPoint2D[] contacts = new ContactPoint2D[3];

	public void SetColorAndSortLayer(Color color, int layerID)
	{
		var renderer = GetComponent<SpriteRenderer>();
		renderer.color = color;
		renderer.sortingLayerID = layerID;
	}

	public void SetSpeed(float force)
	{

	}

	private void Awake()
	{
		collider = GetComponent<Collider2D>();
		rigidbody = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		var emission = particle.emission;
		if (Physics2D.GetContacts(collider, contacts) <= 0)
		{
			emission.enabled = false;
			particlePaused = true;
			particle.transform.position = transform.position;
			return;
		}

		print("Collide");

		foreach (var contact in contacts)
		{
			if (contact.collider == null)
				continue;

			particle.transform.position = contact.point;
			break;
		}

		if (particlePaused)
			emission.enabled = true;
	}

	private void FixedUpdate()
	{
		if (rigidbody.velocity.sqrMagnitude < maxSpeed * maxSpeed)
			return;

		rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
	}
}
