using UnityEngine;

public class AudioManager40bpm : MonoBehaviour
{
	public AudioClip texture;
	public AudioClip kickNoSnare;
	public AudioClip kickWithSnare;
	public AudioClip playerJumpShaker;
	public AudioClip enemyDrums;
	public AudioClip enemyBass;
	public AudioClip enemyChords;
	public AudioClip enemyMelody;

	private AudioSource kickNoSnareAudioSource;
	private AudioSource kickWithSnareAudioSource;
	private AudioSource textureAudioSource;
	private AudioSource playerJumpShakerAudioSource;
	private AudioSource enemyDrumsAudioSource;
	private AudioSource enemyBassAudioSource;
	private AudioSource enemyChordsAudioSource;
	private AudioSource enemyMelodyAudioSource;

	private void Awake() {
		textureAudioSource = gameObject.AddComponent<AudioSource>();
		kickNoSnareAudioSource = gameObject.AddComponent<AudioSource>();
		kickWithSnareAudioSource = gameObject.AddComponent<AudioSource>();
		playerJumpShakerAudioSource = gameObject.AddComponent<AudioSource>();
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

	public void StopTexture() {
		if (texture != null) {
			textureAudioSource.Stop();
		}
	}

	public void PlayKickNoSnare() {
		if (kickNoSnare != null) {
			kickNoSnareAudioSource.clip = kickNoSnare;
			kickNoSnareAudioSource.Play();
		}
	}

	public void StopKickNoSnare() {
		if (kickNoSnare != null) {
			kickNoSnareAudioSource.Stop();
		}
	}

	public void PlayKickWithSnare() {
		if (kickWithSnare != null) {
			kickWithSnareAudioSource.clip = kickWithSnare;
			kickWithSnareAudioSource.Play();
		}
	}

	public void StopKickWithSnare() {
		if (kickWithSnare != null) {
			kickWithSnareAudioSource.Stop();
		}
	}

	public void PlayPlayerJumpShaker() {
		if (playerJumpShaker != null) {
			playerJumpShakerAudioSource.clip = playerJumpShaker;
			playerJumpShakerAudioSource.Play();
		}
	}

	public void StopPlayerJumpShaker() {
		if (playerJumpShaker != null) {
			playerJumpShakerAudioSource.Stop();
		}
	}

	public void PlayEnemyDrums() {
		if (enemyDrums != null) {
			enemyDrumsAudioSource.clip = enemyDrums;
			enemyDrumsAudioSource.Play();
		}
	}

	public void StopEnemyDrums() {
		if (enemyDrums != null) {
			enemyDrumsAudioSource.Stop();
		}
	}

	public void PlayEnemyBass() {
		if (enemyBass != null) {
			enemyBassAudioSource.clip = enemyBass;
			enemyBassAudioSource.Play();
		}
	}

	public void StopEnemyBass() {
		if (enemyBass != null) {
			enemyBassAudioSource.Stop();
		}
	}

	public void PlayEnemyChords() {
		if (enemyChords != null) {
			enemyChordsAudioSource.clip = enemyChords;
			enemyChordsAudioSource.Play();
		}
	}

	public void StopEnemyChords() {
		if (enemyChords != null) {
			enemyChordsAudioSource.Stop();
		}
	}

	public void PlayEnemyMelody() {
		if (enemyMelody != null) {
			enemyMelodyAudioSource.clip = enemyMelody;
			enemyMelodyAudioSource.Play();
		}
	}

	public void StopEnemyMelody() {
		if (enemyMelody != null) {
			enemyMelodyAudioSource.Stop();
		}
	}
}
