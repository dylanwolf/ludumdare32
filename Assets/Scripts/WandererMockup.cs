using UnityEngine;
using System.Collections;

public class WandererMockup : MonoBehaviour {
	Transform _t;
	Rigidbody2D _r;
	Vector3 tmpPos;
	float direction = 1;
	float speed = 1;

	bool isFrozen;

	void Awake () {
		_t = transform;
		_r = GetComponent<Rigidbody2D>();
		_r.velocity = new Vector2(direction * speed, 0);
	}
	
	void Update () {
		if (isFrozen && _r.velocity.x != 0)
		{
			_r.velocity = Vector2.zero;
		}
		else if (!isFrozen && _r.velocity.x == 0)
		{
			_r.velocity = new Vector2(direction * speed, 0);
		}
		else if (direction > 0 && _t.position.x >= 8)
		{
			direction = -1;
			_r.velocity = new Vector2(direction * speed, 0);
		}
		else if (direction < 0 && _t.position.x <= -8)
		{
			direction = 1;
			_r.velocity = new Vector2(direction * speed, 0);
		}

		isFrozen = false;
	}

	void DoHit(LightPower power)
	{
		if (power == LightPower.Damage)
			Debug.Log("HIT!");
		else if (power == LightPower.Freeze)
			isFrozen = true;
	}
}
