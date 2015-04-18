using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CounterBar : MonoBehaviour {

	const float PIXEL_TO_UNITS = 0.32f;
	const float MaxScale = 3;

	static List<CounterBar> Pool = new List<CounterBar>();

	static CounterBar tmpBar;
	static Transform tmpTransform;
	static Vector3 tmpPosWithOffset;
	public static CounterBar InstantiateFromPool(Transform parent, Vector3 offset, Quaternion rotation, Color color, float value, float maxValue)
	{
		tmpBar = null;
		tmpPosWithOffset = parent.position;
		tmpPosWithOffset += offset;
		foreach (CounterBar cb in Pool)
		{
			if (cb.isActiveAndEnabled)
			{
				tmpBar = cb;
				tmpTransform = tmpBar.transform;
				tmpTransform.position = tmpPosWithOffset;
				tmpTransform.rotation = rotation;
				tmpBar.gameObject.SetActive(true);
				break;
			}
		}

		if (tmpBar == null)
		{
			tmpBar = ((GameObject)Instantiate(GameState.Current.CounterBarPrefab, tmpPosWithOffset, rotation)).GetComponent<CounterBar>();
			tmpTransform = tmpBar.transform;
			tmpTransform.rotation = rotation;
			tmpTransform.position = tmpPosWithOffset;
		}

		tmpTransform.parent = parent;
		tmpBar.MaxValue = maxValue;
		tmpBar.Value = value;
		tmpBar.Color = color;
		tmpBar.updated = true;

		return tmpBar;
	}

	public static void ReturnToPool(CounterBar bar)
	{
		bar.transform.parent = null;
		bar.gameObject.SetActive(false);
	}

	public Transform BarTransform;
	public SpriteRenderer BarSprite;
	public float Value;
	public float MaxValue;
	public Color Color;
	bool updated = false;

	public void UpdateValue(float value)
	{
		Value = value;
		updated = true;
	}

	Vector3 tmpPos = Vector3.zero;
	Vector3 tmpScale;
	float pct;
	void Update()
	{
		if (updated)
		{
			pct = 1-(Value / MaxValue);
			tmpPos.x = -(MaxScale * PIXEL_TO_UNITS * pct) / 2;
			tmpScale = BarTransform.localScale;
			tmpScale.x = MaxScale * (1 - pct);
			BarTransform.localPosition = tmpPos;
			BarTransform.localScale = tmpScale;
			BarSprite.color = Color;
			updated = false;
		}
	}
}
