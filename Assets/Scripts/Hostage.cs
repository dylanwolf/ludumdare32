using UnityEngine;
using System.Collections;

public class Hostage : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D collider)
	{
		// Player rescue
		if (collider.gameObject.layer == LayerManager.PlayerLayer)
		{
			MessageWindow.Current.DisplayText("You rescued an innocent bystander!");
			DestroyObject(gameObject);
		}
		else if (collider.gameObject.layer == LayerManager.EnemyCollisionLayer)
		{
			MessageWindow.Current.DisplayText("A bad guy attacked an innocent bystander!");
			GameState.Current.SetStatus(ActionState.GameOver);
			DestroyObject(gameObject);
		}
	}

	void DoHit(LightPower power)
	{
		if (power == LightPower.Damage)
		{
			GameState.Current.SetStatus(ActionState.GameOver);
			MessageWindow.Current.DisplayText("You killed an innocent bystander!");
			DestroyObject(gameObject);
		}
	}
}
