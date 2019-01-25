using UnityEngine;
using JTUtility.Platformer;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class FallingGroundDetector : ContactGroundDetector
{
	[SerializeField] float fallingDetectStartDistance = 0;
	[SerializeField] float fallingRaycastXOffset = 0;
	[SerializeField] float fallingRaycastYOffset = 0;
	[SerializeField] float fallingRaycastDepth = 0.01f;
	new Rigidbody2D rigidbody;

	float fallingDistance;

	protected override void Awake()
	{
		base.Awake();
		rigidbody = GetComponent<Rigidbody2D>();
	}

	protected override bool IsOnGround()
	{
		if (rigidbody.velocity.y > 1f)
		{
			fallingDistance = 0;
			return false;
		}

		return base.IsOnGround();
	}

	protected override bool IsDetectedGround()
	{
		if (rigidbody.velocity.y < 0f)
		{
			fallingDistance -= rigidbody.velocity.y;
			if (fallingDistance > fallingDetectStartDistance)
			{
				var centre = transform.position;
				centre.y += fallingRaycastYOffset;

				var hit = Physics2D.Raycast(centre, Vector2.down, fallingRaycastDepth * -rigidbody.velocity.y);

				if (hit.collider != null)
					return true;

				centre.x += fallingRaycastXOffset;
				hit = Physics2D.Raycast(centre, Vector2.down, fallingRaycastDepth * -rigidbody.velocity.y);

				if (hit.collider != null)
					return true;

				centre.x -= fallingRaycastXOffset * 2;
				hit = Physics2D.Raycast(centre, Vector2.down, fallingRaycastDepth * -rigidbody.velocity.y);

				if (hit.collider != null)
					return true;
			}
		}
		else
		{
			fallingDistance = 0;
		}

		return false;
	}
}
