using UnityEngine;
using System.Collections;

public class LightFlash : MonoBehaviour {

	Light flashingLight;
	public float MinIntensity = 1;
	public float MaxIntensity = 8;
	public float Period = 1;

	void Awake()
	{
		flashingLight = GetComponent<Light>();
		StartCoroutine(Flash());
	}

	IEnumerator Flash()
	{
		float timer = 0;
		while (true)
		{
			timer += Time.deltaTime;
			while (timer >= Period)
				timer -= Period;

			flashingLight.intensity = MinIntensity + (Mathf.Sin((timer / Period) * Mathf.PI) * (MaxIntensity - MinIntensity));
			yield return 0;
		}
	}
}
