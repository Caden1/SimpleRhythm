using UnityEngine;

public class AudioManager40bpm : MonoBehaviour
{
	public AudioClip texture;
	public AudioClip kickNoSnare;
	public AudioClip kickWithSnare;
	public AudioClip enemyDrums;
	public AudioClip enemyBass;
	public AudioClip enemyChords;
	public AudioClip enemyMelody;

	private AudioSource kickNoSnareAudioSource;
	private AudioSource kickWithSnareAudioSource;
	private AudioSource textureAudioSource;
	private AudioSource enemyDrumsAudioSource;
	private AudioSource enemyBassAudioSource;
	private AudioSource enemyChordsAudioSource;
	private AudioSource enemyMelodyAudioSource;

	private void Awake() {
		textureAudioSource = gameObject.AddComponent<AudioSource>();
		kickNoSnareAudioSource = gameObject.AddComponent<AudioSource>();
		kickWithSnareAudioSource = gameObject.AddComponent<AudioSource>();
		enemyDrumsAudioSource = gameObject.AddComponent<AudioSource>();
		enemyBassAudioSource = gameObject.AddComponent<AudioSource>();
		enemyChordsAudioSource = gameObject.AddComponent<AudioSource>();
		enemyMelodyAudioSource = gameObject.AddComponent<AudioSource>();
	}

	public void PlayTexture() {
		if (texture != null) {
			textureAudioSource.clip = texture;
			textureAudioSource.Play();
		}
	}

	public void PlayKickNoSnare() {
		if (kickNoSnare != null) {
			kickNoSnareAudioSource.clip = kickNoSnare;
			kickNoSnareAudioSource.Play();
		}
	}

	public void PlayKickWithSnare() {
		if (kickWithSnare != null) {
			kickWithSnareAudioSource.clip = kickWithSnare;
			kickWithSnareAudioSource.Play();
		}
	}

	public void PlayEnemyDrums() {
		if (enemyDrums != null) {
			enemyDrumsAudioSource.clip = enemyDrums;
			enemyDrumsAudioSource.Play();
		}
	}

	public void PlayEnemyBass() {
		if (enemyBass != null) {
			enemyBassAudioSource.clip = enemyBass;
			enemyBassAudioSource.Play();
		}
	}

	public void PlayEnemyChords() {
		if (enemyChords != null) {
			enemyChordsAudioSource.clip = enemyChords;
			enemyChordsAudioSource.Play();
		}
	}

	public void PlayEnemyMelody() {
		if (enemyMelody != null) {
			enemyMelodyAudioSource.clip = enemyMelody;
			enemyMelodyAudioSource.Play();
		}
	}
}
