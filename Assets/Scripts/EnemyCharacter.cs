using UnityEngine;
using System.Collections;

public abstract class EnemyCharacter : MonoBehaviour {

	[System.NonSerialized]
	public PlatformingState Platforming = PlatformingState.Falling;
	public Transform[] GroundCircles;
	public Transform[] HeadCircles;
	public float JumpTimerMax = 0.5f;
	public float JumpForce = 1.0f;
	public float GravityForce = 1.0f;
	public float Speed = 1.0f;

	public Transform[] ParticlePrefabs;
	public float Health = 1.0f;
	public float LightDamage = 4.0f;
	float currentHealth;
	public Vector3 HealthBarOffset;

	bool isGrounded = false;
	bool hitHead = false;
	Collider2D[] childColliders;
	Collider2D[] raycastHits = new Collider2D[255];
	int hitCount = 0;
	const float CASTING_RADIUS = 0.1f;

	protected int aiDirection = 0;
	protected bool aiJump = false;
	protected abstract bool DecideAction();
	protected abstract void InitializeEnemy();

	protected bool isFrozen = false;
	Rigidbody2D _r;
	protected Transform _t;

	protected virtual void Awake()
	{
		_r = GetComponent<Rigidbody2D>();
		_t = transform;
	}

	protected virtual void Start()
	{
		childColliders = GetComponentsInChildren<Collider2D>();
		InitializeEnemy();
		currentHealth = Health;
	}

	IEnumerator JumpCoroutine()
	{
		Platforming = PlatformingState.Jumping;
		for (float timer = JumpTimerMax; timer >= 0; timer -= Time.deltaTime)
		{
			if (GameState.CurrentActionState != ActionState.Playing)
			{
				timer += Time.deltaTime;
				yield return 0;
			}

			if (Platforming == PlatformingState.Grounded)
				break;

			yield return 0;
		}

		if (Platforming != PlatformingState.Grounded)
		{
			Platforming = PlatformingState.Falling;
		}
	}

	const string JUMP_COROUTINE = "JumpCoroutine";

	protected void DoJump()
	{
		if (Platforming == PlatformingState.Grounded)
			StartCoroutine(JUMP_COROUTINE);
	}

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

	bool changed = false;
	bool foundMatch = false;
	Vector2 tmpVelocity;
	Vector3 tmpScale;
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
		changed = false;
		foreach (Transform t in GroundCircles)
		{
			if (TestRaycastHit(t, LayerManager.EnemyAndTerrainLayers))
			{
				isGrounded = true;
				break;
			}
		}

		if (isGrounded && Platforming != PlatformingState.Grounded)
		{
			Platforming = PlatformingState.Grounded;
			changed = true;
		}
		else if (!isGrounded && Platforming == PlatformingState.Grounded)
		{
			Platforming = PlatformingState.Falling;
			changed = true;
		}

		if (Platforming == PlatformingState.Jumping)
		{
			hitHead = false;
			foreach (Transform t in HeadCircles)
			{
				if (TestRaycastHit(t, LayerManager.EnemyAndTerrainLayers))
				{
					hitHead = true;
					break;
				}
			}

			if (hitHead)
				Platforming = PlatformingState.Falling;
		}

		// Ask AI what to do
		if (DecideAction())
			changed = true;

		// Apply movement
		tmpVelocity = _r.velocity;
		tmpScale = _t.localScale;
		if (aiDirection != 0)
		{
			// Apply walking
			if (_r.velocity.x == 0 || Mathf.Sign(_r.velocity.x) != Mathf.Sign(aiDirection))
			{
				tmpVelocity.x = aiDirection * Speed;
				tmpScale.x = Mathf.Abs(tmpScale.x) * aiDirection;
				changed = true;
			}
		}
		else
		{
			if (_r.velocity.x != 0)
			{
				tmpVelocity.x = 0;
				changed = true;
			}
		}

		// Apply jumping
		if (aiJump)
			DoJump();
		else if (Platforming != PlatformingState.Jumping)
			StopCoroutine(JUMP_COROUTINE);

		if (Platforming == PlatformingState.Jumping && _r.velocity.y <= 0)
		{
			tmpVelocity.y = JumpForce;
			changed = true;
		}

		if (isFrozen)
		{
			tmpVelocity = Vector2.zero;
			changed = true;
			isFrozen = false;
		}

		// Apply falling
		if (Platforming == PlatformingState.Falling && _r.velocity.y >= -GravityForce)
		{
			tmpVelocity.y = -GravityForce;
			changed = true;
		}

		_t.localScale = tmpScale;
		if (changed)
			_r.velocity = tmpVelocity;
	}

	CounterBar healthBar;
	Transform tmpParticles;
	void DealDamage(float damage)
	{
		currentHealth -= damage;
		if (currentHealth <= 0)
		{
			tmpParticles = ParticlePrefabs[Random.Range(0, ParticlePrefabs.Length)];
			Instantiate(tmpParticles, _t.position, tmpParticles.rotation);
			DestroyObject(gameObject);
		}
		else if (currentHealth < Health)
		{
			if (healthBar == null)
				healthBar = CounterBar.InstantiateFromPool(_t, HealthBarOffset, Quaternion.identity, Color.red, currentHealth, Health);
			else
				healthBar.UpdateValue(currentHealth);
		}
	}

	protected virtual void DoHit(LightPower power)
	{
		if (power == LightPower.Damage)
		{
			DealDamage(LightDamage * Time.fixedDeltaTime);	
		}
		else if (power == LightPower.Freeze)
		{
			isFrozen = true;
		}
	}
}