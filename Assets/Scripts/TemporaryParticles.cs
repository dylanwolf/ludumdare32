using UnityEngine;
using System.Collections;

public class TemporaryParticles : MonoBehaviour {

	ParticleSystem particles;

	void Awake()
	{
		particles = GetComponent<ParticleSystem>();
	}

	void Update()
	{
		if (!particles.isPlaying)
			DestroyObject(gameObject);
	}
}
