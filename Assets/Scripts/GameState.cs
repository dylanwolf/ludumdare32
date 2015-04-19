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

	public LightPower[] StartingPowers;

	public void SetStatus(ActionState state)
	{
		CurrentActionState = state;
		if (state == ActionState.GameOver)
			RestartButton.Show();
	}

	void Awake()
	{
		Current = this;

		OwnedPowers = LightPower.Damage;
		foreach (LightPower power in StartingPowers)
			OwnedPowers |= power;
	}
}
