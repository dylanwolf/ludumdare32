using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour {

	public static PlayerCharacter Current;
	public static Transform CurrentPosition;

	[System.NonSerialized]
	public PlatformingState Platforming = PlatformingState.Falling;
	public Transform[] GroundCircles;
	public Transform[] HeadCircles;
	public float JumpTimerMax = 0.5f;
	public float JumpForce = 1.0f;
	public float GravityForce = 1.0f;
	public float Speed = 1.0f;
	public float AccelerationTime = 0.5f;

	bool isGrounded = false;
	bool hitHead = false;
	Collider2D[] childColliders;
	Collider2D[] raycastHits = new Collider2D[255];
	int hitCount = 0;
	const float CASTING_RADIUS = 0.1f;

	float inputDirection;
	float lastInputDirection;
	float? jumpInput;
	float moveSpeed = 0;
	bool inputJump;

	Rigidbody2D _r;
	Transform _t;

	void Awake()
	{
		_r = GetComponent<Rigidbody2D>();
		_t = transform;

		Current = this;
		CurrentPosition = _t;
	}

	void Start()
	{
		childColliders = GetComponentsInChildren<Collider2D>();
	}

	IEnumerator JumpCoroutine()
	{
		Platforming = PlatformingState.Jumping;
		for (float timer = JumpTimerMax + (jumpPower * JumpTimeBonus); timer >= 0; timer -= Time.deltaTime)
		{
			if (GameState.CurrentActionState != ActionState.Playing)
			{
				timer += Time.deltaTime;
				yield return 0;
			}

			if (Platforming != PlatformingState.Jumping)
				break;

			yield return 0;
		}

		jumpSpeedMultiplier = 1;
		if (Platforming != PlatformingState.Grounded)
		{
			Platforming = PlatformingState.Falling;
		}
	}

	const string JUMP_COROUTINE = "JumpCoroutine";

	bool startedJump = false;
	float jumpSpeedMultiplier = 1;
	void DoJump()
	{
		if (Platforming == PlatformingState.Grounded)
		{
			jumpInput = inputDirection;
			if (moveSpeed > 0.5)
			{
				jumpSpeedMultiplier = 1.5f;
				jumpPower *= 2;
			}
			StartCoroutine(JUMP_COROUTINE);
			startedJump = true;
		}
		inputJump = false;
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
	void FixedUpdate()
	{
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
			jumpInput = null;
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
					Debug.Log("Hit head");
					hitHead = true;
					break;
				}
			}

			if (hitHead)
				Platforming = PlatformingState.Falling;
		}

		// Apply movement
		tmpVelocity = _r.velocity;
		tmpScale = _t.localScale;
		if (inputDirection != 0)
		{
			// Apply walking
			if (startedJump || speedUpFromStopCoroutineRunning || _r.velocity.x == 0 || inputDirection != lastInputDirection)
			{
				tmpVelocity.x = inputDirection * Speed * moveSpeed * jumpSpeedMultiplier;
				tmpScale.x = Mathf.Abs(tmpScale.x) * Mathf.Sign(inputDirection);
				changed = true;
			}
			startedJump = false;
		}
		else
		{
			if (_r.velocity.x != 0)
			{
				tmpVelocity.x = 0;
				changed = true;
			}
		}
		lastInputDirection = inputDirection;

		// Apply jumping
		if (inputJump)
			DoJump();

		else if (Platforming != PlatformingState.Jumping)
			StopCoroutine(JUMP_COROUTINE);

		if (Platforming == PlatformingState.Jumping && _r.velocity.y <= 0)
		{
			tmpVelocity.y = JumpForce;
			changed = true;
		}

		// Apply falling
		if (Platforming == PlatformingState.Falling && _r.velocity.y >= 0)
		{
			tmpVelocity.y = -GravityForce;
			changed = true;
		}

		_t.localScale = tmpScale;
		if (changed)
			_r.velocity = tmpVelocity;
	}

	IEnumerator SpeedUpFromStop()
	{
		speedUpFromStopCoroutineRunning = true;
		for (float timer = 0; timer <= AccelerationTime; timer += Time.deltaTime)
		{
			if (GameState.CurrentActionState != ActionState.Playing)
			{
				timer -= Time.deltaTime;
				yield return 0;
			}

			moveSpeed = Mathf.Sin(((timer/AccelerationTime) - 1) * Mathf.PI * 0.5f) + 1;
			yield return 0;
		}
		moveSpeed = 1;
		speedUpFromStopCoroutineRunning = false;
	}

	float jumpPower = 0;
	public float MaxJumpHoldTime = 0.2f;
	public float JumpTimeBonus = 0.3f;
	bool isHoldingJump = false;
	bool holdJumpCoroutineRunning = false;
	bool speedUpFromStopCoroutineRunning = false;
	IEnumerator HoldJumpButton()
	{
		holdJumpCoroutineRunning = true;
		for (jumpPower = 0; jumpPower < MaxJumpHoldTime; jumpPower += Time.deltaTime)
		{
			if (GameState.CurrentActionState != ActionState.Playing)
			{
				jumpPower -= Time.deltaTime;
				yield return 0;
			}

			if (!isHoldingJump)
				break;

			yield return 0;
		}

		inputJump = true;
		holdJumpCoroutineRunning = false;
	}

	const string INPUT_HORIZONTAL = "Horizontal";
	const string INPUT_JUMP = "Jump";
	void Update()
	{
		if (jumpInput.HasValue)
			inputDirection = jumpInput.Value;
		else
			inputDirection = Input.GetAxis(INPUT_HORIZONTAL);
		if (inputDirection != 0 && lastInputDirection == 0)
			StartCoroutine(SpeedUpFromStop());

		isHoldingJump = Input.GetButton(INPUT_JUMP);
		if (isHoldingJump && !holdJumpCoroutineRunning)
			StartCoroutine(HoldJumpButton());
	}
}
