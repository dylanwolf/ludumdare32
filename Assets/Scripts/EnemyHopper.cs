using UnityEngine;
using System.Collections;

public class EnemyHopper : EnemyCharacter {

	public int InitialDirection = 1;
	public float ActivationDistance = 0.75f;
	int currentDirection;
	bool decideResult = false;
	bool hasActivated = false;

	public int Hops = 1;

	protected override bool DecideAction()
	{
		if (!hasActivated)
		{
			if (Mathf.Abs(PlayerCharacter.CurrentPosition.position.x - _t.position.x) >= ActivationDistance)
				return false;

			hasActivated = true;
			currentDirection = InitialDirection;
			aiDirection = 0;
		}

		decideResult = false;
		if (aiDirection != currentDirection)
			decideResult = true;
		aiDirection = currentDirection;

		if (Platforming == PlatformingState.Grounded && Hops > 0)
		{
			decideResult = !aiJump;
			aiJump = true;
			Hops--;
		}
		else
		{
			decideResult = aiJump;
			aiJump = false;
		}

		return decideResult;
	}

	protected override void InitializeEnemy()
	{
		currentDirection = InitialDirection;
	}

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		base.OnCollisionEnter2D(collision);
		if (Mathf.Abs(collision.relativeVelocity.x) >= Mathf.Abs(collision.relativeVelocity.y))
		{
			currentDirection *= -1;
		}
	}
}
