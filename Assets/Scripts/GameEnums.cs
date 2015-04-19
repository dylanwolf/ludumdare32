using UnityEngine;
using System.Collections;
using System;

public enum ActionState
{
	Playing,
	GameOver,
	FinishedLevel
}

public enum PlatformingState
{
	Grounded,
	Jumping,
	Falling
}

[Flags]
public enum LightPower
{
	Damage = 1,
	Freeze = 2,
	BreakTerrain = 4
}
