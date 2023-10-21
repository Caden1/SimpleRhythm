using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
	[HideInInspector]
	public bool moveLeft = true;

	private float bpm = 40f;
	private float beatInterval;
	private float nextMoveTime = 0f;
	private float moveStartTime;
	private int moveDirection;
	private float moveDuration;
	private float moveTimer = 0f;
	private Vector2 snapToPosition;
	private Vector2 currentVelocity = Vector2.zero;
	private Rigidbody2D rb;
	private AudioManager40bpm audioManager40bpm;
	private int beatCounter = 0;
	private Animator animator;
	private string enemyName;
	private BoxCollider2D boxCollider;
	private float startGravity;

	private const string JumpEnemyName = "JumpEnemy(Clone)";
	private const string DashEnemyName = "DashEnemy(Clone)";
	private const string ShieldEnemyName = "ShieldEnemy(Clone)";

	private void Start() {
		enemyName = gameObject.name;
		boxCollider = GetComponent<BoxCollider2D>();
		audioManager40bpm = GameObject.Find("AudioObject").GetComponent<AudioManager40bpm>();
		beatInterval = 60f / bpm;
		moveDuration = beatInterval * 0.5f;
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		startGravity = rb.gravityScale;
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

	private void Update() {
		// Handle horizontal movement
		if (Time.time >= nextMoveTime) {
			moveStartTime = Time.time;
			nextMoveTime = Time.time + beatInterval;

			if (enemyName == JumpEnemyName) {
				float moveDistance = 1f;
				float moveXOffset = 0.4f;
				currentVelocity.x = (moveDistance + moveXOffset) * moveDirection;
				animator.Play("Pulse");
				if (beatCounter == 0) {
					audioManager40bpm.PlayEnemyDrums();
				}
			} else if (enemyName == DashEnemyName) {
				float moveDistance = 1f;
				float moveXOffset = 0.4f;
				currentVelocity.x = (moveDistance + moveXOffset) * moveDirection;
				animator.Play("Fade");
				if (beatCounter == 0) {
					audioManager40bpm.PlayEnemyBass();
				}
			} else if (enemyName == ShieldEnemyName) {
				float moveDistance = 2f;
				float moveXOffset = 0.8f;
				currentVelocity.x = (moveDistance + moveXOffset) * moveDirection;
				animator.Play("Shoot");
				if (beatCounter == 0) {
					audioManager40bpm.PlayEnemyChords();
				}
			}

			beatCounter = (beatCounter + 1) % 4;

			moveTimer = moveDuration;
		}

		// Perform horizontal movement
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

		float t = (Time.time - moveStartTime) / moveDuration;

		if (t > 0.95f) {
			StartCoroutine(HandlePositionSnap());
			transform.position = snapToPosition;
			animator.Play("EmptyState");
		}
	}

	private IEnumerator HandlePositionSnap() {
		snapToPosition = SnapToGrid(transform.position, 1f);
		yield return null;
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
		}
	}
}
