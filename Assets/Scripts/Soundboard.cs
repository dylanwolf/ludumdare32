using UnityEngine;
using System.Collections;

public class Soundboard : MonoBehaviour {

	public static Soundboard Current;

	AudioSource source;

	public AudioClip[] BadGuyDamage;
	public AudioClip[] GoodGuyDamage;
	public AudioClip[] BadGuyDeath;
	public AudioClip[] CastSpell;
	public AudioClip[] ExitLevel;
	public AudioClip[] Rescue;
	public AudioClip[] Destroy;

	void Awake()
	{
		Current = this;
		source = GetComponent<AudioSource>();
	}

	void PlaySound(AudioClip clip)
	{
		if (clip == null)
			return;

		source.PlayOneShot(clip);
	}

	void PlayRandomSound(AudioClip[] clips)
	{
		if (clips == null || clips.Length == 0)
			return;

		PlaySound(clips[Random.Range(0, clips.Length)]);
	}

	public static void PlayBadGuyDamage()
	{
		if (Current != null) Current.PlayRandomSound(Current.BadGuyDamage);
	}

	public static void PlayGoodGuyDamage()
	{
		if (Current != null) Current.PlayRandomSound(Current.GoodGuyDamage);
	}

	public static void PlayBadGuyDeath()
	{
		if (Current != null) Current.PlayRandomSound(Current.BadGuyDeath);
	}

	public static void PlayCastSpell()
	{
		if (Current != null) Current.PlayRandomSound(Current.CastSpell);
	}

	public static void PlayExitLevel()
	{
		if (Current != null) Current.PlayRandomSound(Current.ExitLevel);
	}

	public static void PlayRescue()
	{
		if (Current != null) Current.PlayRandomSound(Current.Rescue);
	}

	public static void PlayDestroy()
	{
		if (Current != null) Current.PlayRandomSound(Current.Destroy);
	}
}
