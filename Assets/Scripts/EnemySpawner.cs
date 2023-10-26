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
	private string enemyName;

	private GameObject boss;
	private BossController bossController;

	private const string JumpEnemyName = "JumpEnemy";
	private const string ProjectileEnemyName = "ProjectileEnemy";

	private void Start() {
		enemyName = enemyPrefab.name;
		beatInterval = 60f / bpm;
		spawnPoint = gameObject.transform.position;
		enemyPrefab.GetComponent<EnemyController>().moveLeft = enemyMoveLeft;
		boss = GameObject.FindGameObjectWithTag("Boss");
		if (boss != null) {
			bossController = boss.GetComponent<BossController>();
		}
	}

	private void Update() {
		if (Time.time >= nextMoveTime) {
			if (isScriptActive && beatCounter == 0 && !isSpawned) {
				if (isBossControlled && bossController != null) {
					if (enemyName == JumpEnemyName && bossController.activateJumpEnemy) {
						Instantiate(enemyPrefab, spawnPoint, enemyPrefab.transform.rotation);
					}

					if (enemyName == ProjectileEnemyName && bossController.activateProjectileEnemy) {
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
