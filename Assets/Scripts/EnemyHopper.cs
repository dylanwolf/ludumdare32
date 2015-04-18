using UnityEngine;
using System.Collections;

public class EnemyHopper : EnemyCharacter {

	public int InitialDirection = 1;
	public float ActivationDistance = 0.75f;
	int currentDirection;
	bool decideResult = false;
	bool hasActivated = false;

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

		if (Platforming == PlatformingState.Grounded)
		{
			decideResult = !aiJump;
			aiJump = true;
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

	void OnCollisionEnter2D(Collision2D collision)
	{
		currentDirection *= -1;
	}
}
