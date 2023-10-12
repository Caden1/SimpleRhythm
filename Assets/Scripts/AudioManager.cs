using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioClip kickClip;
	public AudioClip hiHatClip;

	private AudioSource kickAudioSource;
	private AudioSource hiHatAudioSource;

	private void Awake() {
		kickAudioSource = gameObject.AddComponent<AudioSource>();
		hiHatAudioSource = gameObject.AddComponent<AudioSource>();
	}

	public void PlayKick() {
		if (kickClip != null) {
			kickAudioSource.clip = kickClip;
			kickAudioSource.Play();
		}
	}

	public void PlayHiHat() {
		if (hiHatClip != null) {
			hiHatAudioSource.clip = hiHatClip;
			hiHatAudioSource.Play();
		}
	}
}
