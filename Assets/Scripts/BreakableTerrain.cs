using UnityEngine;
using System.Collections;

public class BreakableTerrain : MonoBehaviour {

	public Transform ParticleSource;
	Transform tmpPrefab;
	public Transform[] ParticlePrefabs;
	public float GravitySpeed;
	public Transform[] GroundCircles;

	void DoHit(LightPower power)
	{
		if (GameState.CurrentActionState != ActionState.Playing)
			return;

		if (power != LightPower.BreakTerrain)
			return;

		tmpPrefab = ParticlePrefabs[Random.Range(0, ParticlePrefabs.Length)];
		Instantiate(tmpPrefab, ParticleSource.position, ParticleSource.rotation * tmpPrefab.rotation);
		DestroyObject(gameObject);
	}

	bool isGrounded = false;
	Collider2D[] childColliders;
	Collider2D[] raycastHits = new Collider2D[255];
	int hitCount = 0;
	const float CASTING_RADIUS = 0.1f;

	bool foundMatch;
	bool TestRaycastHit(Transform t, int layerMask)
	{
		hitCount = 0;
		hitCount = Physics2D.OverlapCircleNonAlloc(t.position, CASTING_RADIUS, raycastHits, layerMask);

		for (int i = 0; i < hitCount; i++)
		{
			if (raycastHits[i] == null)
				break;

			foundMatch = false;
			foreach (Collider2D c in childColliders)
			{
				if (c == raycastHits[i])
				{
					foundMatch = true;
					break;
				}
			}
			raycastHits[i] = null;

			if (!foundMatch)
			{
				return true;
			}
		}
		return false;
	}

	bool? wasGrounded = null;
	Vector3 tmpVelocity;
	protected virtual void FixedUpdate()
	{
		if (GameState.CurrentActionState != ActionState.Playing)
		{
			if (_r.velocity.x != 0 || _r.velocity.y != 0)
				_r.velocity = Vector3.zero;
			return;
		}

		// Test for current position
		isGrounded = false;
		foreach (Transform t in GroundCircles)
		{
			if (TestRaycastHit(t, LayerManager.EnemyAndTerrainLayers))
			{
				isGrounded = true;
				break;
			}
		}

		if (isGrounded != wasGrounded)
		{
			tmpVelocity = _r.velocity;
			tmpVelocity.y = isGrounded ? 0 : -GravitySpeed;
			_r.velocity = tmpVelocity;
		}
		wasGrounded = isGrounded;
	}

	Rigidbody2D _r;
	void Awake()
	{
		_r = GetComponent<Rigidbody2D>();
	}

	void Start()
	{
		childColliders = GetComponentsInChildren<Collider2D>();
	}
}
