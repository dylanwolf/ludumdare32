using UnityEngine;
using System.Collections;

public class EnemyWanderer : EnemyCharacter {

	int currentDirection = 1;
	bool decideResult = false;

	protected override bool DecideAction()
	{
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
		return;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		currentDirection *= -1;
	}
}
