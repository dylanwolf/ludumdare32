using UnityEngine;
using System.Collections;

public class BreakableTerrain : MonoBehaviour {

	public Transform ParticleSource;
	Transform tmpPrefab;
	public Transform[] ParticlePrefabs;

	void DoHit(LightPower power)
	{
		if (GameState.CurrentActionState != ActionState.Playing)
			return;

		if (power != LightPower.BreakTerrain)
			return;

		tmpPrefab = ParticlePrefabs[Random.Range(0, ParticlePrefabs.Length)];
		Instantiate(tmpPrefab, ParticleSource.position, ParticleSource.rotation * tmpPrefab.rotation);
		DestroyObject(gameObject);
	}
}
