using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using JTUtility;

public class ControllerInputModel : IInputModel
{
	JoyStickInput input;
	Dictionary<string, PS4ControllerInputAxis> axisMap;
	Dictionary<string, PS4ControllerInputButton> buttonMap;
	Dictionary<string, PS4ControllerInputAxis> axisAlternativeMap;
	Dictionary<string, PS4ControllerInputButton> buttonAlternativeMap;

	public ControllerInputModel(JoyStickInput input, string[] commandList, string[] controllerKeys)
	{
		if (input == null || input.JoyStickNumber <= 0)
			throw new System.ArgumentException("Input is null or haven't intialized");

		if (commandList.Length != controllerKeys.Length)
			throw new System.ArgumentException("Length of the arraies don't match");

		this.input = input;
		axisMap = new Dictionary<string, PS4ControllerInputAxis>();
		buttonMap = new Dictionary<string, PS4ControllerInputButton>();
		axisAlternativeMap = new Dictionary<string, PS4ControllerInputAxis>();
		buttonAlternativeMap = new Dictionary<string, PS4ControllerInputButton>();

		for (int i = 0; i < commandList.Length; i++)
		{
			
			if (System.Enum.IsDefined(typeof(PS4ControllerInputButton), controllerKeys[i]))
			{
				var button = (PS4ControllerInputButton)System.Enum.Parse(typeof(PS4ControllerInputButton), controllerKeys[i]);
				if (!buttonMap.ContainsKey(commandList[i]))
					buttonMap.Add(commandList[i], button);
				else
					buttonAlternativeMap.Add(commandList[i], button);
				continue;
			}

			if (System.Enum.IsDefined(typeof(PS4ControllerInputAxis), controllerKeys[i]))
			{
				var axis = (PS4ControllerInputAxis)System.Enum.Parse(typeof(PS4ControllerInputAxis), controllerKeys[i]);
				if (!axisMap.ContainsKey(commandList[i]))
					axisMap.Add(commandList[i], axis);
				else
					axisAlternativeMap.Add(commandList[i], axis);
				continue;
			}

			Debug.Log("Cannot parse the key " + controllerKeys[i]);
		}
	}

	public float GetAxis(string key)
	{
		var standard = input.GetAxis(axisMap[key]);
		var alternative = 0.0f;

		if (axisAlternativeMap.ContainsKey(key))
			alternative = input.GetAxis(axisAlternativeMap[key]);

		return Mathf.Abs(standard) > Mathf.Abs(alternative) ? standard : alternative;
	}

	public bool GetButtonDown(string key)
	{
		var standard = input.GetButtonDown(buttonMap[key]);
		var alternative = false;

		if (axisAlternativeMap.ContainsKey(key))
			alternative = input.GetButtonDown(buttonAlternativeMap[key]);

		return standard? standard : alternative;
	}

	public bool GetButton(string key)
	{
		var standard = input.GetButton(buttonMap[key]);
		var alternative = false;

		if (axisAlternativeMap.ContainsKey(key))
			alternative = input.GetButton(buttonAlternativeMap[key]);

		return standard ? standard : alternative;
	}

	public bool GetButtonUp(string key)
	{
		var standard = input.GetButtonUp(buttonMap[key]);
		var alternative = false;

		if (axisAlternativeMap.ContainsKey(key))
			alternative = input.GetButtonUp(buttonAlternativeMap[key]);

		return standard ? standard : alternative;
	}
}
