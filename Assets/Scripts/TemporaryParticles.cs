using UnityEngine;
using System.Collections;

public class TemporaryParticles : MonoBehaviour {

	ParticleSystem particles;
	bool isPaused = false;

	void Awake()
	{
		particles = GetComponent<ParticleSystem>();
	}

	void Update()
	{
		if (GameState.CurrentActionState != ActionState.Playing && particles.isPlaying)
		{
			particles.Pause();
			isPaused = true;
		}
		else if (GameState.CurrentActionState == ActionState.Playing && !particles.isPlaying)
		{
			if (isPaused)
			{
				particles.Play();
				isPaused = false;
			}
			else
			{
				DestroyObject(gameObject);
			}
		}
	}
}
