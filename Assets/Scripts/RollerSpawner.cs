using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public class RollerSpawner : MonoBehaviour
{
	[SerializeField, MinMaxSlider(0, 3)] Vector2 spawnDelay;
	[SerializeField, MinMaxSlider(0, 1)] Vector2 colorRange;
	[SerializeField, MinMaxSlider(0, 20)] Vector2 speedRange;
	[SerializeField] GameObject rollerPrefab;
	[SerializeField] string frontLayerName;
	[SerializeField] string backLayerName;

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
		rollers.Add(instance);
		instance.transform.position = transform.position;

		var roller = instance.GetComponentInChildren<Roller>();
		var color = Color.white * colorRange.RandomBetween();
		color.a = 1;

		if (color.r > (colorRange.x + colorRange.y)/2)
			roller.SetColorAndSortLayer(color, frontLayerName);
		else
			roller.SetColorAndSortLayer(color, backLayerName);

		roller.SetSpeed(speedRange.RandomBetween());
	}

	public void DestroyAllRoller()
	{
		for (int i = 0; i < rollers.Count; i++)
		{
			if (rollers[i] != null)
				Destroy(rollers[i]);
		}
	}

	public void DestroyAllRollerBefore(float x)
	{
		for (int i = 0; i < rollers.Count; i++)
		{
			if (rollers[i] != null && rollers[i].GetComponentInChildren<Roller>().transform.position.x < x)
				Destroy(rollers[i]);
		}
	}
}
