using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility.Platformer;

[RequireComponent(typeof(BaseGroundDetector))]
[RequireComponent(typeof(BaseCharacterJumper))]
public class CharacterMover : PhysicalMover, IInputModelPlugable
{
	[SerializeField] int id;
	[SerializeField] float jumpHeight;

	Vector2 desiredVelocity;

	IInputModel inputModel;

	protected override void Awake()
	{
		base.Awake();

		var detector = GetComponent<BaseGroundDetector>();
		detector.OnStayGround += Detector_OnStayGround;

		var jumper = GetComponent<CharacterJumper>();
		jumper.OnJump += Jumper_OnJump;

		InputManager.Instance.RegisterPluggable(id, this);
	}

	private void Detector_OnStayGround()
	{
		if (desiredVelocity.y < 0)
			desiredVelocity.y = 0;
	}

	private void Jumper_OnJump()
	{
		// Concluded from S = Vi * t + 1/2 * a * t^2 and t = (Vf - Vi)/a
		desiredVelocity.y = Mathf.Sqrt(19.62f * jumpHeight * rigidBody.gravityScale);
	}

	protected override Vector3 GetMovingDirection()
	{
		var input = new Vector3(inputModel.GetAxis("Horizontal"), 0, 0);
		return input;
	}

	protected override void Moving(Vector3 vector)
	{
		desiredVelocity.x = vector.x;
		desiredVelocity.y += vector.y;

		if (vector.x > 0)
		{
			this.transform.localScale = Vector3.one;
		}
		else if (vector.x < 0)
		{
			this.transform.localScale = new Vector3(-1, 1, 1);
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		rigidBody.MovePosition(transform.position + (Vector3)desiredVelocity * Time.deltaTime);
		desiredVelocity += Physics2D.gravity * Time.deltaTime * rigidBody.gravityScale;
		rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, desiredVelocity, Time.deltaTime * 8);
	}

	public void SetInputModel(IInputModel model)
	{
		inputModel = model;
	}
}
