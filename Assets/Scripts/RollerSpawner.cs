using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public class RollerSpawner : MonoBehaviour
{
	[SerializeField, MinMaxSlider(0, 3)] Vector2 spawnDelay;
	[SerializeField, MinMaxSlider(0, 1)] Vector2 colorRange;
	[SerializeField, MinMaxSlider(0, 2)] Vector2 sizeRange;
	[SerializeField] GameObject rollerPrefab;
	[SerializeField] int frontLayerID;
	[SerializeField] int backLayerID;

	float time;
	List<GameObject> rollers;

	private void Awake()
	{
		rollers = new List<GameObject>();

	}
	
	void Update()
    {
		time -= Time.deltaTime;
		if (time > 0)
			return;

		Spawn();
		time = spawnDelay.RandomBetween();
	}

	void Spawn()
	{
		var instance = Instantiate(rollerPrefab);
		instance.transform.position = transform.position;

		var roller = instance.GetComponentInChildren<Roller>();
		var color = Color.white * colorRange.RandomBetween();
		color.a = 1;

		if (color.r > 0.5f)
			roller.SetColorAndSortLayer(color, frontLayerID);
		else
			roller.SetColorAndSortLayer(color, backLayerID);

		roller.transform.localScale = Vector3.one * sizeRange.RandomBetween();
	}
}
