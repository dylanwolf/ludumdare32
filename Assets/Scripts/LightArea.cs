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
			counter.UpdateValue(timer);
			yield return 0;
		}
		isInEffect = false;
		triggeredLight.color = Color.white;
		CounterBar.ReturnToPool(counter);
		affectedPower = null;
		counter = null;
	}

	Transform tmpPrefab;
	LightPower? affectedPower;
	void OnMouseUpAsButton()
	{
		if (isInEffect)
			return;

		isInEffect = true;
		triggeredLight.color = GameState.CurrentLightPower == LightPower.Damage ? new Color(1, 0, 0) : new Color(0, 0, 1);
		affectedPower = GameState.CurrentLightPower;
		tmpPrefab = ParticlePrefabs[Random.Range(0, ParticlePrefabs.Length)];
		Instantiate(tmpPrefab, particleSource.position, particleSource.rotation * tmpPrefab.rotation);
		counter = CounterBar.InstantiateFromPool(particleSource, TimerBarOffset, Quaternion.identity, new Color(64/255.0f, 109/255.0f, 151/255.0f), EffectTimerMax, EffectTimerMax);
		StartCoroutine("DoEffect");
	}

	void OnTriggerStay2D(Collider2D collider)
	{
		if (affectedPower.HasValue)
			collider.gameObject.SendMessage("DoHit", affectedPower.Value, SendMessageOptions.DontRequireReceiver);
	}
}
