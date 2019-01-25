using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility.Platformer;

public class CharacterJumper : PhysicalJumper, IInputModelPlugable
{
	[SerializeField] int id;

	IInputModel input;

	protected override void Awake()
	{
		InputManager.Instance.RegisterPluggable(id, this);
		base.Awake();
	}

	public void SetInputModel(IInputModel model)
	{
		input = model;
	}

	protected override Vector3 GetJumpingCommand()
	{
		if (input.GetButtonDown("Jump"))
		{
			return Vector3.up;
		}

		return base.GetJumpingCommand();
	}
}
