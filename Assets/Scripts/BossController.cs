using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
	[HideInInspector] public bool activateJumpEnemy = false;
	[HideInInspector] public bool activateProjectileEnemy = false;

	public GameObject shieldEnemyPrefab;

	private float bpm = 40f;
	private float beatInterval;
	private float nextMoveTime = 0f;
	private float moveStartTime;
	private float moveDuration;
	private float moveTimer = 0f;
	private float moveDistance = 1f;
	private float moveXOffset = 0.4f;
	private float verticalDashDistance = 5f;
	private float verticalDashXOffset = 2f;
	private float horizontalDashDistance = 2f;
	private float horizontalDashXOffset = 0.8f;
	private int moveDirection = -1;
	private float wallCheckDistance = 2f;
	private float startingXPos;
	private bool isDashAttackDown = false;
	private bool isDashAttackHorizontal = false;
	private bool isDashAttackUp = false;
	private Vector2 snapToPosition;
	private Vector2 currentVelocity = Vector2.zero;
	private Rigidbody2D rb;
	private AudioManager40bpm audioManager40bpm;
	private int beatCounter = 0;
	private Animator animator;
	private CircleCollider2D circleCollider;
	private int barCounter = 0;
	private LayerMask ignoreMask;

	private void Start() {
		circleCollider = GetComponent<CircleCollider2D>();
		audioManager40bpm = GameObject.Find("AudioObject").GetComponent<AudioManager40bpm>();
		ignoreMask = ~(LayerMask.GetMask("Boss"));
		startingXPos = transform.position.x;
		beatInterval = 60f / bpm;
		moveDuration = beatInterval * 0.5f;
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
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
					break;
				case 4:
					if (beatCounter == 3) {
						activateJumpEnemy = false;
					}
					break;
				case 5:
					if (beatCounter == 0) {
						activateProjectileEnemy = true;
						animator.Play("CloseEye");
					} else if (beatCounter == 1) {
						animator.Play("ActivateProjectileEnemies");
					} else if (beatCounter == 2) {
						activateProjectileEnemy = false;
						animator.Play("EmptyState");
					}
					//if (beatCounter == 3) {
					//	activateProjectileEnemy = true;
					//	animator.Play("CloseEye");
					//}
					break;
				case 6:
					//if (beatCounter == 0) {
					//	animator.Play("ActivateProjectileEnemies");
					//} else if (beatCounter == 1) {
					//	activateProjectileEnemy = false;
					//	animator.Play("EmptyState");
					//}
					break;
				case 7:
					break;
				case 8:
					//if (beatCounter == 3) {
					//	animator.Play("CloseEye");
					//}
					break;
				case 9:
					//if (beatCounter == 0) {
					//	animator.Play("ActivateShieldEnemies");
					//	Instantiate(shieldEnemyPrefab,
					//		new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1.5f),
					//		shieldEnemyPrefab.transform.rotation);
					//		animator.Play("ActivateShieldEnemies");
					//} else if (beatCounter == 1) {
					//	animator.Play("EmptyState");
					//}
					break;
				case 10:
					if (beatCounter == 3) {
						animator.Play("CloseEye");
					}
					//if (beatCounter == 3) {
					//	animator.Play("CloseEye");
					//}
					break;
				case 11:
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
				case 12:
					//if (beatCounter == 0) {
					//	isDashAttackDown = true;
					//	animator.Play("Dash");
					//	circleCollider.enabled = false;
					//	audioManager40bpm.LoopEnemyBass();
					//} else if (beatCounter == 1) {
					//	animator.Play("EmptyState");
					//} else if (beatCounter == 2) {
					//	isDashAttackHorizontal = true;
					//	isDashAttackDown = false;
					//}
					break;
				case 13:
					//if (beatCounter == 2) {
					//	isDashAttackUp = true;
					//	isDashAttackHorizontal = false;
					//}
					break;
				case 14:
					//if (beatCounter == 0) {
					//	isDashAttackUp = false;
					//} else if (beatCounter == 2) {
					//	circleCollider.enabled = true;
					//}
					break;
				case 15:
					break;
				case 16:
					if (beatCounter == 0) {
						animator.Play("CloseEye");
					} else if (beatCounter == 1) {
						isDashAttackDown = true;
						animator.Play("Dash");
						//circleCollider.enabled = false;
					} else if (beatCounter == 2) {
						animator.Play("EmptyState");
					} else if (beatCounter == 3) {
						isDashAttackHorizontal = true;
						isDashAttackDown = false;
					}
					break;
				case 17:
					break;
				case 18:
					break;
				case 19:
					if (beatCounter == 2) {
						isDashAttackUp = true;
						isDashAttackHorizontal = false;
					}
					break;
				case 20:
					if (beatCounter == 0) {
						isDashAttackUp = false;
					} else if (beatCounter == 2) {
						//circleCollider.enabled = true;
					}
					break;
				case 21:
					break;
				case 22:
					break;
			}

			if (isDashAttackDown) {
				currentVelocity.x = 0f;
				currentVelocity.y = (verticalDashDistance + verticalDashXOffset) * -1;
			} else if (isDashAttackHorizontal) {
				currentVelocity.x = (horizontalDashDistance + horizontalDashXOffset) * moveDirection;
				currentVelocity.y = 0f;
			} else if (isDashAttackUp) {
				currentVelocity.x = 0f;
				currentVelocity.y = (verticalDashDistance + verticalDashXOffset) * 1;
				circleCollider.enabled = true;
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
			if (isDashAttackHorizontal) {
				circleCollider.enabled = false;
			}
			rb.velocity = new Vector2(currentVelocity.x, currentVelocity.y);
			moveTimer -= Time.deltaTime;
		} else {
			if (isDashAttackHorizontal) {
				circleCollider.enabled = true;
			}
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
			
		} else if (collision.gameObject.CompareTag("PlayerProjectile")) {
			
		}
	}
}
