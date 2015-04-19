using UnityEngine;
using System.Collections;

public class MusicToggle : MonoBehaviour {

	public Renderer OnIcon;
	public float Margin = 0.3f;
	Camera _c;
	AudioSource _music;

	void Awake()
	{
		_c = GetComponentInParent<Camera>();
		_music = GetComponent<AudioSource>();
	}

	void Start()
	{
		Vector3 tmpPos = transform.localPosition;
		tmpPos.x = (_c.aspect * _c.orthographicSize) - Margin;
		tmpPos.y = -(_c.orthographicSize - Margin);
		transform.localPosition = tmpPos;
	}

	void OnMouseUpAsButton()
	{
		if (_music.isPlaying)
		{
			_music.Pause();
			OnIcon.enabled = false;
		}
		else
		{
			_music.Play();
			OnIcon.enabled = true;
		}
	}
}
