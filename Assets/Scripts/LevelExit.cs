using UnityEngine;
using System.Collections;

public class LevelExit : MonoBehaviour {

	public string Message;
	public string NextLevel;

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.layer == LayerManager.PlayerLayer)
		{
			Soundboard.PlayExitLevel();
			MessageWindow.Current.DisplayFinalText(Message, NextLevel);
		}
	}
}
