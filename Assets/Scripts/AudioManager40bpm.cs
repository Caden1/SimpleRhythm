using UnityEngine;

public class AudioManager40bpm : MonoBehaviour
{
	public AudioClip texture;
	public AudioClip kick;
	public AudioClip kickWithSnare;
	public AudioClip enemyDrums;
	public AudioClip enemyBass;
	public AudioClip enemyChords;
	public AudioClip enemyMelody;
	public AudioClip playerJump;
	public AudioClip playerShield;
	public AudioClip playerDash;
	public AudioClip playerProjectile;

	private AudioSource kickAudioSource;
	private AudioSource kickWithSnareAudioSource;
	private AudioSource textureAudioSource;
	private AudioSource enemyDrumsAudioSource;
	private AudioSource enemyBassAudioSource;
	private AudioSource enemyChordsAudioSource;
	private AudioSource enemyMelodyAudioSource;
	private AudioSource playerJumpAudioSource;
	private AudioSource playerShieldAudioSource;
	private AudioSource playerDashAudioSource;
	private AudioSource playerProjectileAudioSource;

	private void Awake() {
		textureAudioSource = gameObject.AddComponent<AudioSource>();
		kickAudioSource = gameObject.AddComponent<AudioSource>();
		kickWithSnareAudioSource = gameObject.AddComponent<AudioSource>();
		enemyDrumsAudioSource = gameObject.AddComponent<AudioSource>();
		enemyBassAudioSource = gameObject.AddComponent<AudioSource>();
		enemyChordsAudioSource = gameObject.AddComponent<AudioSource>();
		enemyMelodyAudioSource = gameObject.AddComponent<AudioSource>();
		playerJumpAudioSource = gameObject.AddComponent<AudioSource>();
		playerShieldAudioSource = gameObject.AddComponent<AudioSource>();
		playerDashAudioSource = gameObject.AddComponent<AudioSource>();
		playerProjectileAudioSource = gameObject.AddComponent<AudioSource>();
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

	public void PlayKick() {
		if (kick != null) {
			kickAudioSource.clip = kick;
			kickAudioSource.Play();
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

	public void LoopEnemyDrums() {
		if (enemyDrums != null) {
			enemyDrumsAudioSource.clip = enemyDrums;
			enemyDrumsAudioSource.loop = true;
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

	public void LoopEnemyBass() {
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

	public void LoopEnemyChords() {
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

	public void LoopEnemyMelody() {
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

	public void PlayPlayerJump() {
		if (playerJump != null) {
			playerJumpAudioSource.clip = playerJump;
			playerJumpAudioSource.Play();
		}
	}
	
	public void PlayPlayerShield() {
		if (playerShield != null) {
			playerShieldAudioSource.clip = playerShield;
			playerShieldAudioSource.Play();
		}
	}

	public void PlayPlayerDash() {
		if (playerDash != null) {
			playerDashAudioSource.clip = playerDash;
			playerDashAudioSource.Play();
		}
	}

	public void PlayPlayerProjectile() {
		if (playerProjectile != null) {
			playerProjectileAudioSource.clip = playerProjectile;
			playerProjectileAudioSource.Play();
		}
	}
}
