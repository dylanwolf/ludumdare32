using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {

	BoxCollider2D _collider;
	Camera _c;
	public float MinCameraLeftX;
	public float MaxCameraRightX;

	public float MaxCameraTopY;
	public float MinCameraBottomY;

	float screenWidth;

	float minCameraX;
	float maxCameraX;

	float minLocalX;
	float maxLocalX;

	float minCameraY;
	float maxCameraY;

	float minLocalY;
	float maxLocalY;

	Vector3 tmpPos;

	Transform _t;
	Transform _ct;

	void Start()
	{
		_c = GetComponentInParent<Camera>();
		_ct = _c.transform;
		_collider = GetComponent<BoxCollider2D>();
		_t = transform;

		screenWidth = _c.aspect * _c.orthographicSize * 2;
		float w = _collider.size.x * _t.localScale.x;
		float h = _collider.size.y * _t.localScale.y;

		minCameraX = MinCameraLeftX + (screenWidth / 2);
		maxCameraX = MaxCameraRightX - (screenWidth / 2);

		minCameraY = MinCameraBottomY + _c.orthographicSize;
		maxCameraY = MaxCameraTopY - _c.orthographicSize;

		minLocalY = (h / 2) - _c.orthographicSize;
		maxLocalY = _c.orthographicSize - (h / 2);

		minLocalX = (w - screenWidth) / 2;
		maxLocalX = (screenWidth - w) / 2;
	}

	float cameraX;
	float cameraY;
	float posX;
	float posY;
	float pct;
	void Update () {

		cameraX = _ct.position.x;
		if (cameraX < minCameraX) cameraX = minCameraX;
		else if (cameraX > maxCameraX) cameraX = maxCameraX;

		cameraY = _ct.position.y;
		if (cameraY < minCameraY) cameraY = minCameraY;
		else if (cameraY > maxCameraY) cameraY = maxCameraY;

		pct = (cameraX - minCameraX) / (maxCameraX - minCameraX);
		posX = minLocalX + (pct * (maxLocalX - minLocalX));

		pct = (cameraY - minCameraY) / (maxCameraY - minCameraY);
		posY = minLocalY + (pct * (maxLocalY - minLocalY));

		tmpPos = _t.localPosition;
		tmpPos.x = posX;
		tmpPos.y = posY;
		_t.localPosition = tmpPos;
	}
}
