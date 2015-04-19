using UnityEngine;
using System.Collections;

public class LightArea : MonoBehaviour {

	Light triggeredLight;
	Transform particleSource;

	public Vector3 TimerBarOffset = new Vector3(0, 1, 0);
	public float EffectTimerMax = 2.0f;
	bool isInEffect = false;
	CounterBar counter;

	public Transform[] ParticlePrefabs;

	// Use this for initialization
	void Start () {
		triggeredLight = transform.parent.GetComponentInChildren<Light>();
		particleSource = transform.GetChild(0).transform;
	}

	IEnumerator DoEffect()
	{
		for (float timer = EffectTimerMax; timer >= 0; timer -= Time.deltaTime)
		{
			if (GameState.CurrentActionState != ActionState.Playing)
			{
				timer += Time.deltaTime;
				yield return 0;
			}

			counter.UpdateValue(timer);
			yield return 0;
		}
		isInEffect = false;
		triggeredLight.color = Color.white;
		CounterBar.ReturnToPool(counter);
		AffectedPower = null;
		counter = null;
	}

	Transform tmpPrefab;
	public LightPower? AffectedPower;
	void OnMouseDown()
	{
		if (GameState.CurrentActionState != ActionState.Playing)
			return;

		if (isInEffect)
			return;

		isInEffect = true;
		Soundboard.PlayCastSpell();
		triggeredLight.color = EffectConstants.GetEffectColor(GameState.CurrentLightPower);
		AffectedPower = GameState.CurrentLightPower;
		tmpPrefab = ParticlePrefabs[Random.Range(0, ParticlePrefabs.Length)];
		Instantiate(tmpPrefab, particleSource.position, particleSource.rotation * tmpPrefab.rotation);
		counter = CounterBar.InstantiateFromPool(particleSource, TimerBarOffset, Quaternion.identity, EffectConstants.GetEffectColor(GameState.CurrentLightPower), EffectTimerMax, EffectTimerMax);
		StartCoroutine("DoEffect");
	}

	void OnTriggerStay2D(Collider2D collider)
	{
		if (GameState.CurrentActionState != ActionState.Playing)
			return;

		if (AffectedPower.HasValue)
		{
			collider.gameObject.SendMessage("DoHit", AffectedPower.Value, SendMessageOptions.DontRequireReceiver);
		}
	}
}
