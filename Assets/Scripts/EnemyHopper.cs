using UnityEngine;
using System.Collections;

public class EnemyHopper : EnemyCharacter {

	int currentDirection = 1;
	bool decideResult = false;

	protected override bool DecideAction()
	{
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
		return;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		currentDirection *= -1;
	}
}
