using UnityEngine;

public class BossController : MonoBehaviour
{
	[HideInInspector] public bool activateJumpEnemy = false;
	[HideInInspector] public bool activateProjectileEnemy = false;

	public GameObject shieldEnemyPrefab;
	public GameObject bossWallPrefab;
	public GameObject[] projectileEnemySpawners;

	private float beatInterval;
	private float moveStartTime;
	private float moveDuration;
	private float xDistanceToPlayer;
	private float yDistanceToPlayer;

	private const float bpm = 40f;
	private const float moveDistance = 1f;
	private const float moveXOffset = 0.4f;
	private const float wallCheckDistance = 2f;
	private const int phase1Length = 14;
	private const int phase2Length = 10;
	private const int phase3Length = 6;

	private int moveDirection = -1;
	private int barCounterPhase1 = 0;
	private int barCounterPhase2 = 0;
	private int barCounterPhase3 = 0;
	private int beatCounter = 0;
	private float nextMoveTime = 0f;
	private float moveTimer = 0f;
	private bool isHorizontalDashAttack = false;
	private bool isVerticalDashAttack = false;
	private bool isStunned = false;
	private bool isMovingAfterStun = false;
	private bool isNearWall = false;
	private bool hitBoss = false;
	private bool isPhase1 = true;
	private bool isPhase2 = false;
	private bool isPhase3 = false;
	private bool isBossDefeated = false;

	private Vector2 currentVelocity = Vector2.zero;

	private Vector2 directionToPlayer;
	private LayerMask ignoreMask;
	private GameObject bossWallClone;
	private GameObject player;
	private Rigidbody2D rb;
	private AudioManager40bpm audioManager40bpm;
	private Animator animator;
	private PlayerController playerController;

	private void Start() {
		CacheComponents();
		InitializeVariables();
		SetupProjectileEnemySpawners();
	}

	private void Update() {
		PerformEnvironmentChecks();

		if (IsTimeForNextMove()) {
			ProcessMove();
			ProcessActionQueues();
		}

		ApplyMovement();

		if (!isStunned) {
			SnapTransform();
		}
	}

	private void CacheComponents() {
		audioManager40bpm = GameObject.Find("AudioObject").GetComponent<AudioManager40bpm>();
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		ignoreMask = ~(LayerMask.GetMask("Boss") | LayerMask.GetMask("Enemy") | LayerMask.GetMask("Player"));
		player = GameObject.FindGameObjectWithTag("Player");
		if (player != null) {
			playerController = player.GetComponent<PlayerController>();
		}
	}

	private void InitializeVariables() {
		beatInterval = 60f / bpm;
		moveDuration = beatInterval * 0.5f;
	}

	private void SetupProjectileEnemySpawners() {
		projectileEnemySpawners[0].SetActive(true);
		projectileEnemySpawners[1].SetActive(true);
		projectileEnemySpawners[2].SetActive(false);
		projectileEnemySpawners[3].SetActive(false);
	}

	private void PerformEnvironmentChecks() {
		Vector2 raycastDirection = (moveDirection == 1) ? Vector2.right : Vector2.left;
		isNearWall = Physics2D.Raycast(transform.position, raycastDirection, wallCheckDistance, ignoreMask);
	}

	private bool IsTimeForNextMove() {
		return Time.time >= nextMoveTime;
	}

	private void ProcessMove() {
		float currentTime = Time.time;
		moveStartTime = currentTime;
		nextMoveTime = currentTime + beatInterval;

		if (isNearWall) {
			moveDirection *= -1;
		}

		if (isPhase1) {
			HandleBarAndBeatActionsPhase1();
		} else if (isPhase2) {
			HandleBarAndBeatActionsPhase2();
		} else if (isPhase3) {
			//HandleBarAndBeatActionsPhase3();
		} else if (isBossDefeated) {
			//HandleBarAndBeatActionsBossDefeated();
		}

		beatCounter = (beatCounter + 1) % 4;

		if (isPhase1 && beatCounter == 0) {
			barCounterPhase1 = (barCounterPhase1 + 1) % phase1Length;
		} else if (isPhase2 && beatCounter == 0) {
			barCounterPhase2 = (barCounterPhase2 + 1) % phase2Length;
		} else if (isPhase3 && beatCounter == 0) {
			barCounterPhase3 = (barCounterPhase3 + 1) % phase3Length;
		}

		moveTimer = moveDuration;
	}

	private void HandleBarAndBeatActionsPhase1() {
		float groundCrashYOffset = 0.35f;
		float stunnedYOffset = 0.15f;

		switch (barCounterPhase1) {
			case 0:
				if (beatCounter == 3) {
					activateJumpEnemy = true;
					animator.Play("CloseEye");
				}
				break;
			case 1:
				if (beatCounter == 0) {
					animator.Play("ActivateJumpEnemies");
				} else if (beatCounter == 1) {
					animator.Play("EmptyState");
				}
				break;
			case 3:
				if (beatCounter == 3) {
					activateProjectileEnemy = true;
					animator.Play("CloseEye");
				}
				break;
			case 4:
				if (beatCounter == 0) {
					animator.Play("ActivateProjectileEnemies");
					activateProjectileEnemy = false;
				} else if (beatCounter == 1) {
					animator.Play("EmptyState");
				} else if (beatCounter == 3) {
					activateJumpEnemy = false;
				}
				break;
			case 8:
				if (beatCounter == 3) {
					animator.Play("CloseEye");
				}
				break;
			case 9:
				if (beatCounter == 0) {
					animator.Play("ActivateShieldEnemies");
					Instantiate(shieldEnemyPrefab,
						new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1.5f),
						shieldEnemyPrefab.transform.rotation);
				} else if (beatCounter == 1) {
					animator.Play("EmptyState");
				}
				break;
			case 11:
				if (beatCounter == 3) {
					animator.Play("CloseEye");
				}
				break;
			case 12:
				if (beatCounter == 0) {
					isHorizontalDashAttack = true;
					directionToPlayer = player.transform.position - transform.position;
					xDistanceToPlayer = Mathf.Abs(directionToPlayer.x);
					yDistanceToPlayer = Mathf.Abs(directionToPlayer.y);
					directionToPlayer.Normalize();
					audioManager40bpm.LoopEnemyBass();
					audioManager40bpm.LoopBossCounterMelody();
					animator.Play("Dash");

					bossWallClone = Instantiate(bossWallPrefab,
						new Vector2(player.transform.position.x + (5f * playerController.moveDirection), -4.5f),
						bossWallPrefab.transform.rotation);

				} else if (beatCounter == 1) {
					isVerticalDashAttack = true;
					isHorizontalDashAttack = false;
					animator.Play("Angry");
				} else if (beatCounter == 2) {
					isStunned = true;
					isVerticalDashAttack = false;
					animator.Play("GroundCrash");
					transform.position = new Vector2(transform.position.x, transform.position.y - groundCrashYOffset);
				} else if (beatCounter == 3) {
					animator.Play("Stunned");
					transform.position = new Vector2(transform.position.x, transform.position.y - stunnedYOffset);
				}
				break;
			case 13:
				if (beatCounter == 0) {
					transform.position = new Vector2(transform.position.x, transform.position.y + groundCrashYOffset + stunnedYOffset);
					isMovingAfterStun = true;
					isStunned = false;
					animator.Play("Angry");
					Destroy(bossWallClone);
				} else if (beatCounter == 1) {
					isMovingAfterStun = false;
					animator.Play("EmptyState");
				} else if (beatCounter == 3 && hitBoss) {
					hitBoss = false;
					isPhase1 = false;
					isPhase2 = true;
					projectileEnemySpawners[0].SetActive(false);
					projectileEnemySpawners[1].SetActive(false);
					projectileEnemySpawners[2].SetActive(true);
					projectileEnemySpawners[3].SetActive(true);
				}
				break;
		}
	}

	private void HandleBarAndBeatActionsPhase2() {
		float groundCrashYOffset = 0.35f;
		float stunnedYOffset = 0.15f;

		switch (barCounterPhase2) {
			case 0:
				if (beatCounter == 3) {
					animator.Play("CloseEye");
				}
				break;
			case 1:
				if (beatCounter == 0) {
					animator.Play("ActivateShieldEnemies");
					Instantiate(shieldEnemyPrefab,
						new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1.5f),
						shieldEnemyPrefab.transform.rotation);
				} else if (beatCounter == 1) {
					animator.Play("EmptyState");
				} else if (beatCounter == 3) {
					animator.Play("CloseEye");
				}
				break;
			case 2:
				if (beatCounter == 0) {
					animator.Play("ActivateShieldEnemies");
					Instantiate(shieldEnemyPrefab,
						new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1.5f),
						shieldEnemyPrefab.transform.rotation);
				} else if (beatCounter == 1) {
					animator.Play("EmptyState");
				}
				break;
			case 3:
				if (beatCounter == 3) {
					activateProjectileEnemy = true;
					animator.Play("CloseEye");
				}
				break;
			case 4:
				if (beatCounter == 0) {
					animator.Play("ActivateProjectileEnemies");
					activateProjectileEnemy = false;
				} else if (beatCounter == 1) {
					animator.Play("EmptyState");
				}
				break;
			case 7:
				if (beatCounter == 3) {
					animator.Play("CloseEye");
				}
				break;
			case 8:
				if (beatCounter == 0) {
					isHorizontalDashAttack = true;
					directionToPlayer = player.transform.position - transform.position;
					xDistanceToPlayer = Mathf.Abs(directionToPlayer.x);
					yDistanceToPlayer = Mathf.Abs(directionToPlayer.y);
					directionToPlayer.Normalize();
					audioManager40bpm.LoopEnemyBass();
					audioManager40bpm.LoopBossCounterMelody();
					animator.Play("Dash");

					bossWallClone = Instantiate(bossWallPrefab,
						new Vector2(player.transform.position.x + (5f * playerController.moveDirection), -4.5f),
						bossWallPrefab.transform.rotation);

				} else if (beatCounter == 1) {
					isVerticalDashAttack = true;
					isHorizontalDashAttack = false;
					animator.Play("Angry");
				} else if (beatCounter == 2) {
					isStunned = true;
					isVerticalDashAttack = false;
					animator.Play("GroundCrash");
					transform.position = new Vector2(transform.position.x, transform.position.y - groundCrashYOffset);
				} else if (beatCounter == 3) {
					animator.Play("Stunned");
					transform.position = new Vector2(transform.position.x, transform.position.y - stunnedYOffset);
				}
				break;
			case 9:
				if (beatCounter == 0) {
					transform.position = new Vector2(transform.position.x, transform.position.y + groundCrashYOffset + stunnedYOffset);
					isMovingAfterStun = true;
					isStunned = false;
					animator.Play("Angry");
					Destroy(bossWallClone);
				} else if (beatCounter == 1) {
					isMovingAfterStun = false;
					animator.Play("EmptyState");
				} else if (beatCounter == 3 && hitBoss) {
					hitBoss = false;
					isPhase2 = false;
					isPhase3 = true;
				}
				break;
		}
	}

	private void ProcessActionQueues() {
		if (isHorizontalDashAttack) {
			if (directionToPlayer.x >= 0f) {
				currentVelocity.x = ((xDistanceToPlayer + 1f) + ((xDistanceToPlayer + 1f) * 0.4f)) * 1f;
				currentVelocity.y = 0f;
			} else {
				currentVelocity.x = ((xDistanceToPlayer - 1f) + ((xDistanceToPlayer - 1f) * 0.4f)) * -1f;
				currentVelocity.y = 0f;
			}
		} else if (isVerticalDashAttack) {
			if (directionToPlayer.y >= 0f) {
				currentVelocity.x = 0f;
				currentVelocity.y = ((yDistanceToPlayer + 1f) + ((yDistanceToPlayer + 1f) * 0.4f)) * 1f;
			} else {
				currentVelocity.x = 0f;
				currentVelocity.y = ((yDistanceToPlayer - 1f) + ((yDistanceToPlayer - 1f) * 0.4f)) * -1f;
			}
		} else if (isStunned) {
			currentVelocity.x = 0f;
			currentVelocity.y = 0f;
		} else if (isMovingAfterStun) {
			currentVelocity.x = 0f;
			currentVelocity.y = (10f + (10f * 0.4f)) * 1f;
		} else {
			currentVelocity.x = (moveDistance + moveXOffset) * moveDirection;
			currentVelocity.y = 0f;
		}
	}

	private void ApplyMovement() {
		if (moveTimer > 0) {
			rb.velocity = new Vector2(currentVelocity.x, currentVelocity.y);
			moveTimer -= Time.deltaTime;
		} else {
			rb.velocity = new Vector2(0f, 0f);
		}
	}

	private void SnapTransform() {
		float t = (Time.time - moveStartTime) / moveDuration;

		if (t > 0.95f) {
			Vector2 snapToPosition = SnapToGrid(transform.position, 1f);
			transform.position = snapToPosition;
		}
	}

	private Vector2 SnapToGrid(Vector2 position, float gridSize) {
		float x = Mathf.Round(position.x / gridSize) * gridSize;
		float y = Mathf.Round((position.y) / gridSize) * gridSize;
		return new Vector2(x, y);
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Player")) {
			Destroy(collision.gameObject);
		}
		if (collision.gameObject.CompareTag("PlayerProjectile")) {
			Destroy(collision.gameObject);
			hitBoss = true;
		}
		if (collision.gameObject.CompareTag("Enemy")) {
			Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
		}
	}
}
