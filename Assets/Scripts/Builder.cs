using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public class Builder : Climber
{
    [SerializeField] bool ready;
    [SerializeField] bool building;
	[SerializeField] bool floatThrough;
	[SerializeField] Vector3 buildTargetPosition;
	[SerializeField] Color buildColor;

	Quaternion origRotation;

    public event Action OnReady;
    public event Action OnFinished;

	public bool Ready
    {
        get => ready;
        set
        {
            if (ready == false && value == true)
            {
                OnReady.Invoke();
            }
            ready = value;
        }
    }
	public bool Working { get; set; }
	public bool Finished { get; set; }

	public void BeginBuilding(Vector3 position)
	{
		this.gameObject.layer = LayerMask.NameToLayer("Roller");
		buildTargetPosition = position;
		building = true;
		Working = true;
	}

	public void Initialize()
	{
		origRotation = transform.rotation;
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Update()
	{
		if (Ready)
		{
			GetComponentInChildren<SpriteRenderer>().color = buildColor;
        }

		if (building)
		{
			Build();
		}
		else
		{
			base.Update();
		}
	}

	void Build()
	{
		var toTarget = buildTargetPosition - transform.position;

		if (Finished) return;

		if (floatThrough)
		{
			if (toTarget.sqrMagnitude < Time.deltaTime * Time.deltaTime * 100)
			{
				transform.position = buildTargetPosition;
				this.gameObject.layer = LayerMask.NameToLayer("Roller");
				rigidbody.simulated = true;
				rigidbody.isKinematic = true;
				rigidbody.velocity = Vector3.zero;
				Finished = true;
				OnFinished.Invoke();
			}
			else
			{
				transform.position = Vector3.MoveTowards(transform.position, buildTargetPosition, Time.deltaTime * 5);
				transform.rotation = origRotation;
			}
		}
		else
		{
			Debug.DrawRay(transform.position, toTarget);
			toTarget.Normalize();
			rigidbody.AddForce(toTarget * 3000);

			var dotProduct = Vector3.Dot(Vector3.up, toTarget);
			if (dotProduct < 0.1f || dotProduct > 0.98f)
			{
				floatThrough = true;
				rigidbody.simulated = false;
			}
		}
	}
}
