using UnityEngine;
using System.Collections;

public class EnemyPace : EnemyCharacter {

	public float MinX = 0;
	public float MaxX = 0;
	public int InitialDirection = 1;
	int currentDirection;
	bool decideResult = false;

	protected override bool DecideAction()
	{
		if ((_t.position.x < MinX && currentDirection < 0) || (_t.position.x > MaxX && currentDirection > 0))
			currentDirection *= -1;

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
