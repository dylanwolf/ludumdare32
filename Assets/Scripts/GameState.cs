using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {

	public static GameState Current;

	#region States
	public static ActionState CurrentActionState;
	public static LightPower CurrentLightPower = LightPower.Freeze;
	public static LightPower OwnedPowers = LightPower.Damage | LightPower.Freeze | LightPower.BreakTerrain;
	#endregion 

	#region Defaults
	public GameObject CounterBarPrefab;
	#endregion

	void Awake()
	{
		Current = this;
	}
}
