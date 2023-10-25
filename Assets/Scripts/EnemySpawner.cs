using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public GameObject enemyPrefab;
	public bool enemyMoveLeft;
	public bool isSpawningOnce = false;
	public bool isBossControlled = false;

	private float bpm = 40f;
	private float nextMoveTime = 0f;
	private int beatCounter = 0;
	private bool isScriptActive = false;
	private bool isSpawned = false;
	private Vector2 spawnPoint;
	private float beatInterval;

	private BossController bossController;

	private void Start() {
		beatInterval = 60f / bpm;
		spawnPoint = gameObject.transform.position;
		enemyPrefab.GetComponent<EnemyController>().moveLeft = enemyMoveLeft;
		bossController = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossController>();
	}

	private void Update() {
		if (Time.time >= nextMoveTime) {
			if (isScriptActive && beatCounter == 0 && !isSpawned) {
				if (isBossControlled && bossController != null) {
					if (bossController.activateJumpEnemy) {
						Instantiate(enemyPrefab, spawnPoint, enemyPrefab.transform.rotation);
					}
				} else {
					Instantiate(enemyPrefab, spawnPoint, enemyPrefab.transform.rotation);
				}
				if (isSpawningOnce) {
					isSpawned = true;
				}
			}
			nextMoveTime = Time.time + beatInterval;
			beatCounter = (beatCounter + 1) % 4;
			if (beatCounter == 3) {
				isScriptActive = true;
			}
		}
	}
}
