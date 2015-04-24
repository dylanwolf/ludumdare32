using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour {

	public static PlayerCharacter Current;
	public static Transform CurrentPosition;

	[System.NonSerialized]
	public PlatformingState Platforming = PlatformingState.Grounded;
	public Transform[] GroundCircles;
	public Transform[] HeadCircles;
	public float MaxSpeed = 1.0f;
	public float AccelerationRate = 1.0f;
	public float DeclerationRate = 1.5f;
	public float ReactPct = 0.3f;
	float speedPct = 0;
	Animator anim;

	bool isGrounded = false;
	bool hitHead = false;
	Collider2D[] childColliders;
	Collider2D[] raycastHits = new Collider2D[255];
	int hitCount = 0;
	const float CASTING_RADIUS = 0.1f;

	float inputDirection;
	float lastInputDirection;
	bool inputJump;

	Rigidbody2D _r;
	Transform _t;

	void Awake()
	{
		_r = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		_t = transform;

		Current = this;
		CurrentPosition = _t;
	}

	void Start()
	{
		childColliders = GetComponentsInChildren<Collider2D>();
	}

	public float MaxJumpTime = 0.5f;
	public float MaxJumpHeight = 4.0f;
	float jumpStartY = 0;
	float jumpEndY = 0;
	float jumpVelocity = 0;
	bool isHoldingJump = false;

	float jumpPosY = 0;
	float jumpT = 0;
	float jumpTSquared = 0;

	bool startedJump;
	IEnumerator JumpCoroutine()
	{
		Platforming = PlatformingState.Jumping;
		jumpVelocity = 0;

		jumpStartY = _t.position.y;
		jumpEndY = jumpStartY + MaxJumpHeight;

		jumpTSquared = Mathf.Pow(MaxJumpTime, 2);

		while (_t.position.y < jumpEndY && Platforming == PlatformingState.Jumping && isHoldingJump)
		{
			// Handle pause
			if (GameState.CurrentActionState != ActionState.Playing)
				yield return 0;

			// Adjust velocity
			jumpPosY = _t.position.y - jumpStartY;
			jumpT = Mathf.Sqrt(jumpTSquared - ((jumpPosY * jumpTSquared) / MaxJumpHeight));
			jumpVelocity = (2 * jumpT * MaxJumpHeight) / jumpTSquared;

			yield return 0;
		}

		if (Platforming != PlatformingState.Grounded)
			DoFall();
	}

	const string JUMP_COROUTINE = "JumpCoroutine";

	void DoFall()
	{
		if (!isAcceleratingFall)
			StartCoroutine(FALL_COROUTINE);
	}

	const string FALL_COROUTINE = "FallingCoroutine";

	bool isAcceleratingFall = false;
	float fallVelocity = 0;
	IEnumerator FallingCoroutine()
	{
		if (Platforming != PlatformingState.Falling)
		{
			isAcceleratingFall = true;
			Platforming = PlatformingState.Falling;
			fallVelocity = 0;
			jumpTSquared = Mathf.Pow(MaxJumpTime, 2);

			for (float t = 0; t < MaxJumpTime; t += Time.deltaTime)
			{
				if (GameState.CurrentActionState != ActionState.Playing)
				{
					t -= Time.deltaTime;
					yield return 0;
				}

				if (Platforming != PlatformingState.Falling)
					break;

				t += Time.deltaTime;
				fallVelocity = -(2 * t * MaxJumpHeight) / jumpTSquared;
				yield return 0;
			}

			isAcceleratingFall = false;
		}
	}

	void DoJump()
	{
		if (Platforming == PlatformingState.Grounded)
		{
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
			if (raycastHits[i] == null || raycastHits[i].isTrigger)
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

	int facing = 1;
	bool changed = false;
	bool foundMatch = false;
	Vector2 tmpVelocity;
	Vector3 tmpScale;
	bool paused = false;
	void FixedUpdate()
	{
		if (GameState.CurrentActionState != ActionState.Playing)
		{
			if (_r.velocity.x != 0 || _r.velocity.y != 0)
				_r.velocity = Vector3.zero;
			anim.enabled = false;
			paused = true;
			return;
		}
		if (paused)
			anim.enabled = true;

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
			DoFall();
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
				DoFall();

		}

		// Apply movement
		tmpVelocity = _r.velocity;
		tmpScale = _t.localScale;
		if (inputDirection != 0)
		{
			// Apply walking
			if (startedJump || speedPct < 1 || inputDirection != lastInputDirection)
			{
				speedPct += (Time.fixedDeltaTime * AccelerationRate);
				speedPct = Mathf.Clamp(speedPct, 0, 1);
				facing = (int)Mathf.Sign(inputDirection);
				tmpScale.x = Mathf.Abs(tmpScale.x) * facing;
				changed = true;
			}
			startedJump = false;
		}
		else
		{
			if (speedPct > 0)
			{
				speedPct -= (Time.fixedDeltaTime * DeclerationRate);
				speedPct = Mathf.Clamp(speedPct, 0, 1);
				changed = true;
			}
		}
		tmpVelocity.x = facing * Mathf.Sin(speedPct * 0.5f * Mathf.PI) * MaxSpeed;

		// Apply jumping
		if (inputJump)
			DoJump();

		if (Platforming == PlatformingState.Jumping)
		{
			tmpVelocity.y = jumpVelocity;
			changed = true;
		}

		// Apply falling
		if (Platforming == PlatformingState.Falling)
		{
			tmpVelocity.y = fallVelocity;
			changed = true;
		}

		_t.localScale = tmpScale;
		if (changed)
			_r.velocity = tmpVelocity;

		anim.SetBool(ANIM_JUMPING, Platforming == PlatformingState.Jumping || Platforming == PlatformingState.Falling);
		anim.SetBool(ANIM_WALKING, Platforming == PlatformingState.Grounded && _r.velocity.x != 0);
	}

	const string ANIM_JUMPING = "Jumping";
	const string ANIM_WALKING = "Walking";

	const string INPUT_HORIZONTAL = "Horizontal";
	const string INPUT_JUMP = "Jump";
	bool wasHoldingJump = false;
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();

		lastInputDirection = inputDirection;
		inputDirection = Input.GetAxisRaw(INPUT_HORIZONTAL);
		if (inputDirection != 0)
		{
			if (speedPct <= 0)
				speedPct = 0;
			else if (inputDirection != lastInputDirection)
				speedPct *= 0.5f;
		}

		isHoldingJump = Input.GetButton(INPUT_JUMP);
		if (isHoldingJump && !wasHoldingJump)
		{
			inputJump = true;
		}
		wasHoldingJump = isHoldingJump;
	}
}
