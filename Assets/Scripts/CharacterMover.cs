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
	[SerializeField] float fallingSpeedLimit = 10;

	public bool UseRigidbody { get; set; } = true;

	public float MoveSpeed
	{
		get => moveSpeed;
		set => moveSpeed = value;
	}

	public float FallingSpeedLimit
	{
		get => fallingSpeedLimit;
		set => fallingSpeedLimit = value;
	}

	Vector2 airborneVector;
	Vector2 movementVector;

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
		if (airborneVector.y < 0)
			airborneVector.y = 0;
	}

	private void Jumper_OnJump()
	{
		// Concluded from S = Vi * t + 1/2 * a * t^2 and t = (Vf - Vi)/a
		airborneVector.y = Mathf.Sqrt(19.62f * jumpHeight * rigidBody.gravityScale);
	}

	protected override Vector3 GetMovingDirection()
	{
		var input = new Vector3(inputModel.GetAxis("Horizontal"), 0, 0);
		return input;
	}

	protected override void Moving(Vector3 vector)
	{
		var normal = GetGroundNormal();
		var angle = Vector2.SignedAngle(Vector2.up, normal) * Mathf.Deg2Rad;
		if (angle != 0)
		{
			vector = new Vector2(Mathf.Cos(angle) * vector.x - Mathf.Sin(angle) * vector.y, Mathf.Cos(angle) * vector.y + Mathf.Sin(angle) * vector.x);
			Debug.DrawRay(transform.position, vector * 100);
		}
		movementVector = vector;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		var movement = movementVector + airborneVector;

		if (UseRigidbody)
		{
			rigidBody.MovePosition(transform.position + (Vector3)movement * Time.deltaTime);
		}
		else
		{
			rigidBody.velocity = Vector2.zero;
			transform.position += (Vector3)movement * Time.deltaTime;
		}

		airborneVector += Physics2D.gravity * Time.deltaTime * rigidBody.gravityScale;
		if (airborneVector.y < -FallingSpeedLimit)
			airborneVector.y = -FallingSpeedLimit;

		//rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, desiredVelocity, Time.deltaTime * 1);
	}

	private Vector2 GetGroundNormal()
	{
		Physics2D.queriesStartInColliders = false;
		var hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, 1 << LayerMask.NameToLayer("Ground"));
		if (hit.collider == null)
			return Vector2.up;

		Debug.DrawRay(transform.position, Vector2.down * 2);
		Debug.DrawRay(transform.position, hit.normal * 100);
		return hit.normal;
	}

	public void SetInputModel(IInputModel model)
	{
		inputModel = model;
	}
}
