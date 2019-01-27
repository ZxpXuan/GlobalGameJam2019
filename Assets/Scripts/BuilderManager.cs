using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuilderManager : MonoBehaviour
{
	[SerializeField] List<Builder> builders;

	[SerializeField] List<Vector2> positions;

	[SerializeField] int maxWorkingBuilder;

	[SerializeField] bool build;
	[SerializeField] int workingBuilder;

	[SerializeField] UnityEvent OnFinished;

	bool building;

	int builderCount;


	private void Awake()
	{
		positions = new List<Vector2>();

		// Sorted by lowest Y
		builders.Sort((b1, b2) => 
		{
			return 
			b1.transform.position.y > b2.transform.position.y ? 1 : 
				(b1.transform.position.y < b2.transform.position.y ? -1 : 0);
		});

		for (int i = 0; i < builders.Count; i++)
		{
			positions.Add(builders[i].transform.position);
			builders[i].Initialize();
			builders[i].transform.rotation = Quaternion.identity;
			builders[i].transform.position += new Vector3(Random.Range(-10, 10), Random.Range(0, 5), 0);
			builders[i].OnFinished += BuilderManager_OnFinished;
		}

		builderCount = builders.Count;
	}

	public void StartBuilding()
	{
		build = true;
	}

	private void BuilderManager_OnFinished()
	{
		print("Finished");
		print(workingBuilder);
		workingBuilder--;
		builderCount--;

		if (builderCount <= 1)
			OnFinished.Invoke();
	}

	private void Update()
	{
		if (!build)
		{
			return;
		}

		ManageBuilders();
	}

	private void ManageBuilders()
	{
		if (workingBuilder >= maxWorkingBuilder) return;

		for (int i = 0; i < builders.Count; i ++)
		{
			if (!builders[i].Ready) return;
			if (builders[i].Working || builders[i].Finished) continue;

			workingBuilder++;
			builders[i].BeginBuilding(positions[i]);

			if (workingBuilder >= maxWorkingBuilder)
				break;
		}
	}
}
