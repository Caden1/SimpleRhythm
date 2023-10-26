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
	private int moveDirection = -1;
	private float wallCheckDistance = 2f;
	private Vector2 snapToPosition;
	private Vector2 currentVelocity = Vector2.zero;
	private Rigidbody2D rb;
	private AudioManager40bpm audioManager40bpm;
	private int beatCounter = 0;
	private Animator animator;
	private CircleCollider2D circleCollider;
	private float startGravity;
	private int barCounter = 0;
	private LayerMask ignoreMask;

	private void Start() {
		circleCollider = GetComponent<CircleCollider2D>();
		audioManager40bpm = GameObject.Find("AudioObject").GetComponent<AudioManager40bpm>();
		ignoreMask = ~(LayerMask.GetMask("Boss"));
		beatInterval = 60f / bpm;
		moveDuration = beatInterval * 0.5f;
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		startGravity = rb.gravityScale;
	}

	private void Update() {
		if (Time.time >= nextMoveTime) {
			moveStartTime = Time.time;
			nextMoveTime = Time.time + beatInterval;

			Vector2 raycastDirection = (moveDirection == 1) ? Vector2.right : Vector2.left;
			bool isNearWall = Physics2D.Raycast(transform.position, raycastDirection, wallCheckDistance, ignoreMask);

			if (isNearWall) {
				moveDirection *= -1;
				return;
			}

			//Instantiate(shieldEnemyPrefab,
			//	new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1.5f),
			//	shieldEnemyPrefab.transform.rotation);
			//	animator.Play("ActivateShieldEnemies");

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
					if (beatCounter == 3) {
						activateProjectileEnemy = true;
						animator.Play("CloseEye");
					}
					break;
				case 6:
					if (beatCounter == 0) {
						animator.Play("ActivateProjectileEnemies");
					} else if (beatCounter == 1) {
						animator.Play("EmptyState");
					}
					break;
				case 7:
					break;
				case 8:
					break;
				case 9:
					break;
			}

			currentVelocity.x = (moveDistance + moveXOffset) * moveDirection;

			beatCounter = (beatCounter + 1) % 4;

			if (beatCounter == 0) {
				barCounter = (barCounter + 1) % 32; // 32 bars in total, then the boss loop repeats
			}

			moveTimer = moveDuration;
		}

		if (moveTimer > 0) {
			rb.velocity = new Vector2(currentVelocity.x, rb.velocity.y);
			moveTimer -= Time.deltaTime;
		} else {
			rb.velocity = new Vector2(0f, rb.velocity.y);
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
