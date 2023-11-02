using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[SerializeField] private GameObject enemyPrefab;
	[SerializeField] private bool enemyMoveLeft;
	[SerializeField] private bool isSpawningOnce = false;
	[SerializeField] private bool isBossControlled = false;

	private float beatInterval;
	private string enemyName;

	private const float bpm = 40f;
	private const string JumpEnemyName = "JumpEnemy";
	private const string DashEnemyName = "DashEnemy";
	private const string ProjectileEnemyName = "ProjectileEnemy";

	private int beatCounter = 0;
	private int barCounter = 0;
	private float nextSpawnTime = 0f;
	private bool isScriptActive = false;
	private bool isSpawned = false;

	private Vector2 spawnPoint;
	private GameObject boss;
	private BossController bossController;

	private void Start() {
		CacheComponents();
		InitializeVariables();
	}

	private void Update() {
		if (IsTimeForNextSpawn()) {
			SpawnEnemy();
		}
	}

	private void CacheComponents() {
		enemyPrefab.GetComponent<EnemyController>().moveLeft = enemyMoveLeft;
		boss = GameObject.FindGameObjectWithTag("Boss");
		if (boss != null) {
			bossController = boss.GetComponent<BossController>();
		}
	}

	private void InitializeVariables() {
		enemyName = enemyPrefab.name;
		beatInterval = 60f / bpm;
		spawnPoint = gameObject.transform.position;
	}

	private bool IsTimeForNextSpawn() {
		return Time.time >= nextSpawnTime;
	}

	private void SpawnEnemy() {
		if (isScriptActive && beatCounter == 0 && !isSpawned) {
			if (isBossControlled && bossController != null) {
				if (enemyName == JumpEnemyName && bossController.activateJumpEnemy) {
					Instantiate(enemyPrefab, spawnPoint, enemyPrefab.transform.rotation);
				} else if (enemyName == ProjectileEnemyName && bossController.activateProjectileEnemy) {
					Instantiate(enemyPrefab, spawnPoint, enemyPrefab.transform.rotation);
				}
			} else {
				if (enemyName == DashEnemyName) {
					if (barCounter == 1) {
						Instantiate(enemyPrefab, spawnPoint, enemyPrefab.transform.rotation);
					}
				} else {
					Instantiate(enemyPrefab, spawnPoint, enemyPrefab.transform.rotation);
				}
			}
			if (isSpawningOnce) {
				isSpawned = true;
			}
		}
		nextSpawnTime = Time.time + beatInterval;
		beatCounter = (beatCounter + 1) % 4;
		if (beatCounter == 0) {
			barCounter = (barCounter + 1) % 2;
		}
		if (beatCounter == 3) {
			isScriptActive = true;
		}
	}
}
