using UnityEngine;
using System.Collections;

public class PowerIconContainer : MonoBehaviour {

	public float Margin = 0.1f;
	public Transform Cursor;

	public static PowerIconContainer Current;

	PowerIcon[] icons;

	void Awake()
	{
		Current = this;
	}

	public void UpdateInventory()
	{
		foreach (PowerIcon icon in icons)
		{
			icon.gameObject.SetActive((GameState.OwnedPowers & icon.Power) != 0);
		}
	}

	void Start()
	{
		Vector3 tmpPos = transform.localPosition;
		tmpPos.x = -(Camera.main.aspect * Camera.main.orthographicSize) + Margin;
		tmpPos.y = Camera.main.orthographicSize - Margin;
		transform.localPosition = tmpPos;

		icons = GetComponentsInChildren<PowerIcon>();
		UpdateInventory();
	}
}
