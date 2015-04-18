using UnityEngine;
using System.Collections;

public class PowerIconContainer : MonoBehaviour {

	public float Margin = 0.1f;
	public Transform Cursor;
	Camera _c;

	public static PowerIconContainer Current;

	PowerIcon[] icons;

	void Awake()
	{
		Current = this;
		_c = GetComponentInParent<Camera>();
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
		tmpPos.x = -(_c.aspect * _c.orthographicSize) + Margin;
		tmpPos.y = _c.orthographicSize - Margin;
		transform.localPosition = tmpPos;

		icons = GetComponentsInChildren<PowerIcon>();
		UpdateInventory();
	}
}
