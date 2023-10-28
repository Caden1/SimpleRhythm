using UnityEngine;

public class BossShieldEnemy : MonoBehaviour
{
	private float moveStartTime;
	private float moveDuration;
	private float beatInterval;

	private const float bpm = 40f;
	
	private int beatCounter = 0;
	private float nextMoveTime = 0f;
	private float moveTimer = 0f;

	private Vector2 currentVelocity = Vector2.zero;

	private GameObject player;
	private Rigidbody2D rb;
	private Animator animator;
	private AudioManager40bpm audioManager40bpm;

	private void Start() {
		CacheComponents();
		InitializeVariables();
	}

	private void Update() {
		if (IsTimeForNextMove()) {
			ProcessMove();
		}

		ApplyMovement();
		ApplyRotation();
		SnapTransform();
	}

	private void CacheComponents() {
		player = GameObject.FindGameObjectWithTag("Player");
		audioManager40bpm = GameObject.Find("AudioObject").GetComponent<AudioManager40bpm>();
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	private void InitializeVariables() {
		beatInterval = 60f / bpm;
		moveDuration = beatInterval * 0.5f;
		transform.rotation = Quaternion.AngleAxis(90f, Vector3.forward);
	}

	private bool IsTimeForNextMove() {
		return Time.time >= nextMoveTime;
	}

	private void ProcessMove() {
		float currentTime = Time.time;
		moveStartTime = currentTime;
		nextMoveTime = currentTime + beatInterval;

		float moveDistance = 2f;
		float moveOffset = 0.8f;

		Vector2 directionToPlayer = player.transform.position - transform.position;
		directionToPlayer.Normalize();
		currentVelocity = Vector2.zero;
		if (Mathf.Abs(directionToPlayer.x) >= Mathf.Abs(directionToPlayer.y)) {
			if (directionToPlayer.x >= 0f) {
				currentVelocity.x = (moveDistance + moveOffset) * 1f;
			} else {
				currentVelocity.x = (moveDistance + moveOffset) * -1f;
			}
		} else {
			if (directionToPlayer.y >= 0f) {
				currentVelocity.y = (moveDistance + moveOffset) * 1f;
			} else {
				currentVelocity.y = (moveDistance + moveOffset) * -1f;
			}
		}

		animator.Play("Shoot");
		if (beatCounter == 0) {
			audioManager40bpm.LoopEnemyChords();
		}

		beatCounter = (beatCounter + 1) % 4;

		moveTimer = moveDuration;
	}

	private void ApplyMovement() {
		if (moveTimer > 0) {
			rb.velocity = currentVelocity;
			moveTimer -= Time.deltaTime;
		} else {
			rb.velocity = new Vector2(0f, rb.velocity.y);
		}
	}

	private void ApplyRotation() {
		float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg - 180f;  // Subtract 180 degrees because prefab is facing left
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
	private void SnapTransform() {
		float t = (Time.time - moveStartTime) / moveDuration;

		if (t > 0.95f) {
			transform.position = SnapToGrid(transform.position, 1f);
			animator.Play("EmptyState");
		}
	}

	private Vector2 SnapToGrid(Vector2 position, float gridSize) {
		float x = Mathf.Round(position.x / gridSize) * gridSize;
		float y = Mathf.Round(position.y / gridSize) * gridSize;
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
