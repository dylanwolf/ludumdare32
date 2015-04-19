using UnityEngine;
using System.Collections;

public class LayerManager : MonoBehaviour {

	public static LayerMask EnemyTriggerLayer;
	public static LayerMask EnemyCollisionLayer;
	public static LayerMask TerrainLayer;
	public static LayerMask PlayerLayer;

	public static int EnemyAndTerrainLayers;

	const string PLAYER = "Player";
	const string ENEMY_TRIGGER = "EnemyTrigger";
	const string ENEMY_COLLISION = "EnemyCollision";
	const string TERRAIN = "Terrain";

	void Awake()
	{
		PlayerLayer = LayerMask.NameToLayer(PLAYER);
		EnemyCollisionLayer = LayerMask.NameToLayer(ENEMY_COLLISION);

		EnemyAndTerrainLayers = LayerMask.GetMask(TERRAIN, ENEMY_COLLISION);
	}
}
