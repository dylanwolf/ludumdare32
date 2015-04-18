using UnityEngine;
using System.Collections;

public class LayerManager : MonoBehaviour {

	public static LayerMask EnemyTriggerLayer;
	public static LayerMask EnemyCollisionLayer;
	public static LayerMask TerrainLayer;

	public static int EnemyAndTerrainLayers;

	const string ENEMY_TRIGGER = "EnemyTrigger";
	const string ENEMY_COLLISION = "EnemyCollision";
	const string TERRAIN = "Terrain";

	void Awake()
	{
		EnemyAndTerrainLayers = LayerMask.GetMask(TERRAIN, ENEMY_COLLISION);
	}
}
