using UnityEngine;
using System.Collections;

public class PowerIconContainer : MonoBehaviour {

	public float Margin = 0.1f;
	public Transform Cursor;

	public static PowerIconContainer Current;

	void Awake()
	{
		Current = this;
	}

	void Start()
	{
		Vector3 tmpPos = transform.localPosition;
		tmpPos.x = -(Camera.main.aspect * Camera.main.orthographicSize) + Margin;
		tmpPos.y = Camera.main.orthographicSize - Margin;
		transform.localPosition = tmpPos;
	}
}
