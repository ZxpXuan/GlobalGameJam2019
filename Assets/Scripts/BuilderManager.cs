using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderManager : MonoBehaviour
{
	[SerializeField] List<Builder> builders;

	[SerializeField] List<Vector2> positions;

	[SerializeField] int maxWorkingBuilder;

	[SerializeField] bool build;
	[SerializeField] int workingBuilder;

	bool building;


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
			builders[i].transform.position += new Vector3(Random.Range(-5, 5), Random.Range(0, 2), 0);
			builders[i].OnFinished += BuilderManager_OnFinished;
		}
	}

	private void BuilderManager_OnFinished()
	{
		print("Finished");
		print(workingBuilder);
		workingBuilder--;
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
			if (builders[i].Working || builders[i].Finished) continue;

			workingBuilder++;
			builders[i].BeginBuilding(positions[i]);

			if (workingBuilder >= maxWorkingBuilder)
				break;
		}
	}
}
