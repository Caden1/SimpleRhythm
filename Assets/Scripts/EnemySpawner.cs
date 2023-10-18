using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public GameObject enemyPrefab;

	private float bpm = 40f;
	private float nextMoveTime = 0f;
	private int beatCounter = 0;
	private bool isScriptActive = false;
	private Vector2 spawnPoint;
	private float beatInterval;

	private void Start() {
		beatInterval = 60f / bpm;
		spawnPoint = gameObject.transform.position;
	}

	private void Update() {
		if (Time.time >= nextMoveTime) {
			if (isScriptActive && beatCounter == 0) {
				Instantiate(enemyPrefab, spawnPoint, enemyPrefab.transform.rotation);
			}
			nextMoveTime = Time.time + beatInterval;
			beatCounter = (beatCounter + 1) % 4;
			if (beatCounter == 3) {
				isScriptActive = true;
			}
		}
	}
}
