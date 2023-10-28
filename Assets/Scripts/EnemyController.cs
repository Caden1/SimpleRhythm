using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
	[HideInInspector]
	public bool moveLeft = true;

	private int moveDirection;
	private float beatInterval;
	private float moveStartTime;
	private float moveDuration;
	private float startGravity;
	private string enemyName;

	private const float bpm = 40f;
	private const string JumpEnemyName = "JumpEnemy(Clone)";
	private const string DashEnemyName = "DashEnemy(Clone)";
	private const string ShieldEnemyName = "ShieldEnemy(Clone)";
	private const string ProjectileEnemyName = "ProjectileEnemy(Clone)";

	private float nextMoveTime = 0f;
	private float moveTimer = 0f;
	private int beatCounter = 0;

	private Vector2 currentVelocity = Vector2.zero;

	private Rigidbody2D rb;
	private AudioManager40bpm audioManager40bpm;
	private Animator animator;
	private BoxCollider2D boxCollider;
	private GameObject boss;

	private static Dictionary<string, int> jumpEnemyCount;
	private static Dictionary<string, int> dashEnemyCount;
	private static Dictionary<string, int> shieldEnemyCount;
	private static Dictionary<string, int> projectileEnemyCount;

	static EnemyController() {
		jumpEnemyCount = new Dictionary<string, int>();
		dashEnemyCount = new Dictionary<string, int>();
		shieldEnemyCount = new Dictionary<string, int>();
		projectileEnemyCount = new Dictionary<string, int>();
	}

	private void Start() {
		CacheComponents();
		InitializeVariables();
		InitializeEnemyCounts();
		InitializeMoveDirection();
	}

	private void Update() {
		if (IsTimeForNextMove()) {
			ProcessMove();
		}

		ApplyMovement();
		SnapTransform();
	}

	private void CacheComponents() {
		boss = GameObject.FindGameObjectWithTag("Boss");
		audioManager40bpm = GameObject.Find("AudioObject").GetComponent<AudioManager40bpm>();
		boxCollider = GetComponent<BoxCollider2D>();
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	private void InitializeVariables() {
		enemyName = gameObject.name;
		beatInterval = 60f / bpm;
		moveDuration = beatInterval * 0.5f;
		startGravity = rb.gravityScale;
	}

	private void InitializeEnemyCounts() {
		if (boss == null) {
			if (!jumpEnemyCount.ContainsKey(enemyName)) {
				jumpEnemyCount[enemyName] = 0;
			}
			if (!dashEnemyCount.ContainsKey(enemyName)) {
				dashEnemyCount[enemyName] = 0;
			}
			if (!shieldEnemyCount.ContainsKey(enemyName)) {
				shieldEnemyCount[enemyName] = 0;
			}
			if (!projectileEnemyCount.ContainsKey(enemyName)) {
				projectileEnemyCount[enemyName] = 0;
			}

			jumpEnemyCount[enemyName]++;
			dashEnemyCount[enemyName]++;
			shieldEnemyCount[enemyName]++;
			projectileEnemyCount[enemyName]++;
		}
	}

	private void InitializeMoveDirection() {
		if (moveLeft) {
			moveDirection = -1;
		} else {
			moveDirection = 1;
			if (enemyName == ShieldEnemyName) {
				GetComponent<SpriteRenderer>().flipX = true;
			}
		}

		if (enemyName == ShieldEnemyName) {
			rb.gravityScale = 0f;
		}
	}

	private bool IsTimeForNextMove() {
		return Time.time >= nextMoveTime;
	}

	private void ProcessMove() {
		float currentTime = Time.time;
		moveStartTime = currentTime;
		nextMoveTime = currentTime + beatInterval;

		float moveDistance;
		float moveXOffset;

		switch (enemyName) {
			case JumpEnemyName:
				moveDistance = 1f;
				moveXOffset = 0.4f;
				currentVelocity.x = (moveDistance + moveXOffset) * moveDirection;
				animator.Play("Pulse");
				if (beatCounter == 0) {
					if (boss == null) {
						audioManager40bpm.PlayEnemyDrums();
					} else {
						audioManager40bpm.LoopEnemyDrums();
					}
				}
				break;
			case DashEnemyName:
				moveDistance = 1f;
				moveXOffset = 0.4f;
				currentVelocity.x = (moveDistance + moveXOffset) * moveDirection;
				animator.Play("Fade");
				if (beatCounter == 0) {
					if (boss == null) {
						audioManager40bpm.PlayEnemyBass();
					} else {
						audioManager40bpm.LoopEnemyBass();
					}
				}
				break;
			case ShieldEnemyName:
				moveDistance = 2f;
				moveXOffset = 0.8f;
				currentVelocity.x = (moveDistance + moveXOffset) * moveDirection;
				animator.Play("Shoot");
				if (beatCounter == 0) {
					if (boss == null) {
						audioManager40bpm.PlayEnemyChords();
					} else {
						audioManager40bpm.LoopEnemyChords();
					}
				}
				break;
			case ProjectileEnemyName:
				currentVelocity.x = 0f;
				if (beatCounter == 0) {
					if (boss == null) {
						audioManager40bpm.PlayEnemyMelody();
					} else {
						audioManager40bpm.LoopEnemyMelody();
					}
				}
				break;
		}

		beatCounter = (beatCounter + 1) % 4;

		moveTimer = moveDuration;
	}

	private void ApplyMovement() {
		if (enemyName != ProjectileEnemyName) {
			if (moveTimer > 0) {
				if (enemyName == DashEnemyName) {
					rb.gravityScale = 0f;
					boxCollider.enabled = false;
				}
				rb.velocity = new Vector2(currentVelocity.x, rb.velocity.y);
				moveTimer -= Time.deltaTime;
			} else {
				if (enemyName == DashEnemyName) {
					boxCollider.enabled = true;
					rb.gravityScale = startGravity;
				}
				rb.velocity = new Vector2(0f, rb.velocity.y);
			}
		}
	}

	private void SnapTransform() {
		if (enemyName != ProjectileEnemyName) {
			float t = (Time.time - moveStartTime) / moveDuration;

			if (t > 0.95f) {
				transform.position = SnapToGrid(transform.position, 1f);
				animator.Play("EmptyState");
			}
		}
	}

	private void OnDestroy() {
		if (boss == null) {
			if (jumpEnemyCount.ContainsKey(enemyName)) {
				jumpEnemyCount[enemyName]--;
			}
			if (dashEnemyCount.ContainsKey(enemyName)) {
				dashEnemyCount[enemyName]--;
			}
			if (shieldEnemyCount.ContainsKey(enemyName)) {
				shieldEnemyCount[enemyName]--;
			}
			if (projectileEnemyCount.ContainsKey(enemyName)) {
				projectileEnemyCount[enemyName]--;
			}
			if (jumpEnemyCount[enemyName] <= 0) {
				audioManager40bpm.StopEnemyDrums();
			}
			if (dashEnemyCount[enemyName] <= 0) {
				audioManager40bpm.StopEnemyBass();
			}
			if (shieldEnemyCount[enemyName] <= 0) {
				audioManager40bpm.StopEnemyChords();
			}
			if (projectileEnemyCount[enemyName] <= 0) {
				audioManager40bpm.StopEnemyMelody();
			}
		}
	}

	private Vector2 SnapToGrid(Vector2 position, float gridSize) {
		float x = Mathf.Round(position.x / gridSize) * gridSize;
		float y = 0f;
		if (enemyName == JumpEnemyName) {
			y = Mathf.Round(position.y / gridSize) * gridSize;
		} else if (enemyName == DashEnemyName || enemyName == ShieldEnemyName) {
			y = Mathf.Round((position.y - 0.5f) / gridSize) * gridSize + 0.5f;
		}
		return new Vector2(x, y);
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Player")) {
			if (collision.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Shield")) {
				Destroy(gameObject);
			} else {
				Destroy(collision.gameObject);
			}
		} else if (collision.gameObject.CompareTag("PlayerProjectile")) {
			if (enemyName == ProjectileEnemyName) {
				Destroy(gameObject);
			}

			Destroy(collision.gameObject);
		}
	}
}
