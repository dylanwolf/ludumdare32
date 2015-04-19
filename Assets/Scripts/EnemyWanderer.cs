using UnityEngine;
using System.Collections;

public class EnemyWanderer : EnemyCharacter {

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
		if (Platforming == PlatformingState.Grounded)
		{
			if (aiDirection != currentDirection)
				decideResult = true;
			aiDirection = currentDirection;
		}
		else
		{
			if (aiDirection != 0)
				decideResult = true;
			aiDirection = 0;
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
		currentDirection *= -1;
	}
}
