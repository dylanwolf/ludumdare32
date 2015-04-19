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
		tmpBar.lastFlipped = 0;
		tmpBar.ParentTransform = parent;
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

	Transform _t;
	public Transform BarTransform;
	public SpriteRenderer BarSprite;
	public float Value;
	public float MaxValue;
	public Color Color;
	bool updated = false;
	public Transform ParentTransform;

	public void UpdateValue(float value)
	{
		Value = value;
		updated = true;
	}

	Vector3 tmpPos = Vector3.zero;
	Vector3 tmpScale;
	float pct;

	void Awake()
	{
		_t = transform;
	}

	int lastFlipped = 0;
	int thisFlipped = 0;
	void Update()
	{
		thisFlipped = (int)Mathf.Sign(ParentTransform.lossyScale.x);
		if (lastFlipped != thisFlipped)
		{
			lastFlipped = thisFlipped;
			tmpScale = _t.localScale;
			tmpScale.x = Mathf.Abs(tmpScale.x) * thisFlipped;
			_t.localScale = tmpScale;
		}

		if (updated)
		{
			pct = 1-(Value / MaxValue);
			tmpPos.x = -(MaxScale * PIXEL_TO_UNITS * pct) / 2;
			tmpScale = BarTransform.localScale;
			tmpScale.x = MaxScale * (1 - pct) * Mathf.Sign(ParentTransform.lossyScale.x);
			BarTransform.localPosition = tmpPos;
			BarTransform.localScale = tmpScale;
			BarSprite.color = Color;
			updated = false;
		}
	}
}
