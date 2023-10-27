using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class BossController : MonoBehaviour
{
	[HideInInspector] public bool activateJumpEnemy = false;
	[HideInInspector] public bool activateProjectileEnemy = false;

	public GameObject shieldEnemyPrefab;
	public GameObject bossWallPrefab;

	private GameObject player;
	private PlayerController playerController;
	private GameObject bossWallClone;
	private float bpm = 40f;
	private float beatInterval;
	private float nextMoveTime = 0f;
	private float moveStartTime;
	private float moveDuration;
	private float moveTimer = 0f;
	private float moveDistance = 1f;
	private float moveXOffset = 0.4f;
	private int moveDirection = -1;
	private float wallCheckDistance = 2f;
	private bool isHorizontalDashAttack = false;
	private bool isVerticalDashAttack = false;
	private bool isStunned = false;
	private bool isMovingAfterStun = false;
	private Vector2 directionToPlayer;
	private float xDistanceToPlayer;
	private float yDistanceToPlayer;
	private Vector2 snapToPosition;
	private Vector2 currentVelocity = Vector2.zero;
	private Rigidbody2D rb;
	private AudioManager40bpm audioManager40bpm;
	private int beatCounter = 0;
	private Animator animator;
	private int barCounter = 0;
	private LayerMask ignoreMask;

	private void Start() {
		player = GameObject.FindGameObjectWithTag("Player");
		audioManager40bpm = GameObject.Find("AudioObject").GetComponent<AudioManager40bpm>();
		ignoreMask = ~(LayerMask.GetMask("Boss") | LayerMask.GetMask("Enemy") | LayerMask.GetMask("Player"));
		beatInterval = 60f / bpm;
		moveDuration = beatInterval * 0.5f;
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		if (player != null) {
			playerController = player.GetComponent<PlayerController>();
		}
	}

	private void Update() {
		if (Time.time >= nextMoveTime) {
			moveStartTime = Time.time;
			nextMoveTime = Time.time + beatInterval;

			Vector2 raycastDirection = (moveDirection == 1) ? Vector2.right : Vector2.left;
			bool isNearWall = Physics2D.Raycast(transform.position, raycastDirection, wallCheckDistance, ignoreMask);

			if (isNearWall) {
				moveDirection *= -1;
			}

			switch (barCounter) {
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
				case 2:
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
				case 5:
					break;
				case 6:
					break;
				case 7:
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
						animator.Play("ActivateShieldEnemies");
					} else if (beatCounter == 1) {
						animator.Play("EmptyState");
					}
					break;
				case 10:
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
						animator.Play("EmptyState");
					} else if (beatCounter == 2) {
						isStunned = true;
						isVerticalDashAttack = false;
					}
					break;
				case 13:
					if (beatCounter == 0) {
						isMovingAfterStun = true;
						isStunned = false;
						Destroy(bossWallClone);
					} else if (beatCounter == 1) {
						isMovingAfterStun = false;
					}
					break;
			}

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

			beatCounter = (beatCounter + 1) % 4;

			if (beatCounter == 0) {
				barCounter = (barCounter + 1) % 32; // 32 bars in total, then the boss loop repeats
			}

			moveTimer = moveDuration;
		}

		if (moveTimer > 0) {
			rb.velocity = new Vector2(currentVelocity.x, currentVelocity.y);
			moveTimer -= Time.deltaTime;
		} else {
			rb.velocity = new Vector2(0f, 0f);
		}

		float t = (Time.time - moveStartTime) / moveDuration;

		if (t > 0.95f) {
			StartCoroutine(HandlePositionSnap());
			transform.position = snapToPosition;
		}
	}

	private IEnumerator HandlePositionSnap() {
		snapToPosition = SnapToGrid(transform.position, 1f);
		yield return null;
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
			Destroy(gameObject);
		}
		if (collision.gameObject.CompareTag("Enemy")) {
			Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
		}
	}
}
