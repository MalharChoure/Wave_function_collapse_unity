using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	const float minPathUpdateTime = .2f;
	const float pathUpdateMoveThreshold = .5f;

	public Transform target;
	public float speed = 20;
	public float turnSpeed = 3;
	public float turnDst = 5;
	public float stoppingDst = 10;

	private Animator npcAnimator;
	public Transform[] patrolPoints;
	public float chaseRadius = 10f;
	public float attackRadius = 2f;
	public float patrolSpeed = 2f;
	public float chaseSpeed = 4f;
	private int currentPatrolIndex;
	private float currentSpeed;

	Path path;
	private SeekerState seekerState;
	private enum SeekerState
    {
		patroling,
		following,
		attacking
    }

    private void Update()
    {
		float distanceToPlayer = Vector3.Distance(transform.position, target.position);
		switch (seekerState)
        {
			case SeekerState.patroling:
				Patrol();
				if (distanceToPlayer <= chaseRadius)
				{
					seekerState = SeekerState.following;
					currentSpeed = chaseSpeed;
					StartCoroutine(UpdatePath(target));
				}
				break;

			case SeekerState.following:
				FollowPath();
				if (distanceToPlayer <= attackRadius)
				{
					seekerState = SeekerState.attacking;
					Attack();
				}
				break;

			case SeekerState.attacking:
				Attack();
				if (distanceToPlayer > attackRadius)
				{
					seekerState = SeekerState.following;
					StartCoroutine(UpdatePath(target));
				}
				break;
        }
    }
    void Start() {
		seekerState = SeekerState.patroling;
		currentSpeed = patrolSpeed;
		currentPatrolIndex = 0;
		npcAnimator = GetComponent<Animator>();
		//StartCoroutine (UpdatePath ());
	}

	void Attack()
    {

    }
	void Patrol()
    {

		npcAnimator.SetBool("isRunning", true);
		Transform targetPoint = patrolPoints[currentPatrolIndex];
		RaycastHit hit;
		if(Physics.Raycast(targetPoint.position, Vector3.down, out hit, 100))
        {
			Debug.Log(hit.point);
			transform.LookAt(hit.point);
			//targetPoint.position = hit.point;
		}
		
		//transform.position = Vector3.MoveTowards(transform.position, hit.point, currentSpeed * Time.deltaTime);
		StartCoroutine(UpdatePath(targetPoint));
		// Check if reached the patrol point
		if (Vector3.Distance(targetPoint.position,transform.position)<5f)
		{
			// Move to the next patrol point
			currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
		}
	}
	public void OnPathFound(Vector3[] waypoints, bool pathSuccessful) {
		if (pathSuccessful) {
			path = new Path(waypoints, transform.position, turnDst, stoppingDst);

			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator UpdatePath(Transform target) {

		if (Time.timeSinceLevelLoad < .3f) {
			yield return new WaitForSeconds (.3f);
		}
		PathRequestManager.RequestPath (new PathRequest(transform.position, target.position, OnPathFound));

		float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
		Vector3 targetPosOld = target.position;

		while (true) {
			yield return new WaitForSeconds (minPathUpdateTime);
			//print (((target.position - targetPosOld).sqrMagnitude) + "    " + sqrMoveThreshold);
			if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold) {
				PathRequestManager.RequestPath (new PathRequest(transform.position, target.position, OnPathFound));
				targetPosOld = target.position;
			}
		}
	}

	IEnumerator FollowPath() {

		bool followingPath = true;
		int pathIndex = 0;
		transform.LookAt (path.lookPoints [0]);

		float speedPercent = 1;

		while (followingPath) {
			Vector2 pos2D = new Vector2 (transform.position.x, transform.position.z);
			while (path.turnBoundaries [pathIndex].HasCrossedLine (pos2D)) {
				if (pathIndex == path.finishLineIndex) {
					followingPath = false;
					break;
				} else {
					pathIndex++;
				}
			}

			if (followingPath) {

				if (pathIndex >= path.slowDownIndex && stoppingDst > 0) {
					speedPercent = Mathf.Clamp01 (path.turnBoundaries [path.finishLineIndex].DistanceFromPoint (pos2D) / stoppingDst);
					if (speedPercent < 0.01f) {
						followingPath = false;
					}
				}

				Quaternion targetRotation = Quaternion.LookRotation (path.lookPoints [pathIndex] - transform.position);
				transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
				transform.Translate (Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
			}

			yield return null;

		}
	}

	public void OnDrawGizmos() {
		if (path != null) {
			path.DrawWithGizmos ();
		}
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position,chaseRadius);
	}
}
