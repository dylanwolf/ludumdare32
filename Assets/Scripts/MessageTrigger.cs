using UnityEngine;
using System.Collections;

public class MessageTrigger : MonoBehaviour {

	bool hasActivated = false;
	public string Message;

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (!hasActivated)
			MessageWindow.Current.DisplayText(Message);
		hasActivated = true;
	}
}
