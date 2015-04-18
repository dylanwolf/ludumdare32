using UnityEngine;
using System.Collections;

public class RestartButton : MonoBehaviour {

	public static RestartButton Current;

	void Awake()
	{
		Current = this;
		Current.gameObject.SetActive(false);
	}

	public static void Show()
	{
		if (Current != null)
			Current.gameObject.SetActive(true);
	}

	void OnMouseUpAsButton()
	{
		Application.LoadLevel(Application.loadedLevel);
	}
}
