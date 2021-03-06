﻿using UnityEngine;
using System.Collections;

public class PowerIcon : MonoBehaviour {

	Transform _t;
	public LightPower Power;
	public bool IsDefault = false;

	public void SwitchPower()
	{
		GameState.CurrentLightPower = Power;
		PowerIconContainer.Current.Cursor.position = _t.position;
	}

	void Awake()
	{
		_t = transform;
	}

	void Start()
	{
		if (IsDefault)
			SwitchPower();
	}

	void OnMouseUpAsButton()
	{
		if (GameState.CurrentActionState != ActionState.Playing)
			return;

		SwitchPower();
	}
}
