using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Animal : MonoBehaviour
{
    Rigidbody2D rb;
    public Animator animator;
    bool isDragging;
    Vector3 baseScale;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;
    protected IEnumerator movementsHandlingCoroutine;
    Vector2 movement;
    protected bool inEnclosure;
    public Enclosure currentEnclosure;
    public int enclosureSlotUsed = 1; // 2 for Pigs
    public MovementState currentMovementState = MovementState.Standard;

    [System.Serializable]
    public class MovementProperties
    {
        public float moveSpeed;
        public float topSpeed;
        public float minMoveInterval; //Min time interval in seconds between random movement
        public float maxMoveInterval; //Max interval interval in seconds between random movement
        public float linearDrag; 
    }

    protected MovementProperties currentMovementProperties;

    protected MovementProperties standardMovement;
    protected MovementProperties rageMovement;
    protected MovementProperties chaseMovement;
    protected MovementProperties runMovement;


    //Pahtfinding variables
    Path path;
    public float nextWaypointDistance = 3f;
    int currentWaypoint = 0;
    protected bool reachedEndOfPath = true; //True if has finished pathfinding
    Seeker seeker;

    public enum MovementState
    {
        Standard,
        Rage,
        Chase,
        Run,
    }


    protected virtual void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        movementsHandlingCoroutine = MovemmentsHandling();
        StartCoroutine(movementsHandlingCoroutine);
        baseScale = transform.localScale;

        standardMovement = new MovementProperties();
        standardMovement.moveSpeed = 400f;
        standardMovement.topSpeed = 1f;
        standardMovement.minMoveInterval = 4f;
        standardMovement.maxMoveInterval = 7f;
        standardMovement.linearDrag = 3f;

        rageMovement = new MovementProperties();
        rageMovement.moveSpeed = 500f;
        rageMovement.topSpeed = 1.3f;
        rageMovement.minMoveInterval = 1f;
        rageMovement.maxMoveInterval = 2f;
        rageMovement.linearDrag = 4f;

        chaseMovement = new MovementProperties();
        chaseMovement.moveSpeed = 700f;
        chaseMovement.topSpeed = 1.3f;
        chaseMovement.minMoveInterval = 0f;
        chaseMovement.maxMoveInterval = 0.1f;
        chaseMovement.linearDrag = 5f;

        runMovement = new MovementProperties();
        runMovement.moveSpeed = 800f;
        runMovement.topSpeed = 10f;
        runMovement.minMoveInterval = 0f;
        runMovement.maxMoveInterval = 0.1f;
        runMovement.linearDrag = 5f;

        currentMovementProperties = new MovementProperties();
        currentMovementProperties.moveSpeed = standardMovement.moveSpeed;
        currentMovementProperties.topSpeed = standardMovement.topSpeed;
        currentMovementProperties.minMoveInterval = standardMovement.minMoveInterval;
        currentMovementProperties.maxMoveInterval = standardMovement.maxMoveInterval;
        currentMovementProperties.linearDrag = standardMovement.linearDrag;
    }

    //Move an animal to the target position
    public void MoveTo(Vector3 target)
    {
        seeker.StartPath(rb.position, target, OnPathComplete);
    }

    public void EnterEnclosure(Enclosure enclosure)
    {
        inEnclosure = true;
        currentEnclosure = enclosure;
    }

    public void LeaveEnclosure()
    {
        inEnclosure = false;
    }

    #region Behavior functions

    //Start the RageState coroutine
    public void Enrage()
    {
        reachedEndOfPath = true;
        animator.SetBool("IsRage", true);
        currentMovementState = MovementState.Rage;
    }

    //End the RageState coroutine
    public void Calm()
    {
        animator.SetBool("IsRage", false);
        currentMovementState = MovementState.Standard;
    }

    public void Run()
    {
        if (currentEnclosure == null) return;

        reachedEndOfPath = true; //End current path
        currentMovementState = MovementState.Run;

    }

    public void Chase(Animal animal)
    {
        if (currentEnclosure == null) return;

        reachedEndOfPath = true; //End current path
        currentMovementState = MovementState.Chase;
    }

    #endregion

    private void ComputeMovement(Vector3 Target, MovementProperties mProperties)
    {
        MoveTo(Target);
        currentMovementProperties.moveSpeed = mProperties.moveSpeed;
        currentMovementProperties.topSpeed = mProperties.topSpeed;
        currentMovementProperties.minMoveInterval = mProperties.minMoveInterval;
        currentMovementProperties.maxMoveInterval = mProperties.maxMoveInterval;
    }

    //Standard movement
    private IEnumerator MovemmentsHandling()
    {
        while (true)
        {
            if (currentEnclosure != null && reachedEndOfPath)
            {
                switch (currentMovementState)
                {
                    case MovementState.Standard:
                        ComputeMovement(currentEnclosure.RandomPoint(), standardMovement);
                        break;
                    case MovementState.Rage:
                        ComputeMovement(currentEnclosure.RandomPoint(), rageMovement);
                        break;
                    case MovementState.Run:
                        ComputeMovement(currentEnclosure.RandomPoint(), runMovement);
                        break;
                    case MovementState.Chase:
                        ComputeMovement(currentEnclosure.RandomPoint(), chaseMovement);
                        break;
                    default:
                        break;
                }

                yield return new WaitForSeconds(Random.Range(currentMovementProperties.minMoveInterval, currentMovementProperties.maxMoveInterval));
            }
            else
                yield return new WaitForSeconds(1f);
        }
    }


    //Callback for pathfinding generation
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    //Start drag
    public void OnMouseDown()
    {
        isDragging = true;
        animator.SetBool("IsDrag", true);
        transform.localScale = baseScale * 1.3f;
        audioSource.Play();
        transform.gameObject.tag = "Drag";
        path = null;
        Cursor.visible = false;
    }

    //Stop drag
    public void OnMouseUp()
    {
        isDragging = false;
        animator.SetBool("IsDrag", false);
        transform.localScale = baseScale;
        spriteRenderer.flipX = false;
        audioSource.Stop();
        reachedEndOfPath = true;
        transform.gameObject.tag = "Animal";
        Cursor.visible = true;
    }

    protected virtual void Update()
    {
        movement.x = Mathf.Clamp(rb.velocity.x, -1f, 1f);
        movement.y = Mathf.Clamp(rb.velocity.y, -1f, 1f);

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.magnitude);
        
        //Drag and drop
        if (isDragging)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousePosition);

            path = null; //Cancel movement
            rb.velocity = new Vector2(0, 0);

            if (Input.GetAxis("Mouse X") != 0)
                spriteRenderer.flipX = (Input.GetAxis("Mouse X") > 0);
        }
            
    }

    private void FixedUpdate()
    {
        #region PahtFinding

        if (path != null)
        {

            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                rb.velocity = new Vector2(0, 0);
            }
            else
            {
                reachedEndOfPath = false;

                Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
                Vector2 force = direction * currentMovementProperties.moveSpeed * Time.deltaTime;

                rb.AddForce(force);

                float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

                if (currentWaypoint == path.vectorPath.Count - 1)
                {
                    if (distance < 0.1f) currentWaypoint++;
                }
                else
                {
                    if (distance < nextWaypointDistance) currentWaypoint++;
                }

            }

            if (rb.velocity.magnitude > currentMovementProperties.topSpeed)
            {
                rb.velocity = rb.velocity.normalized * currentMovementProperties.topSpeed;
            }
        }
        #endregion    
    }
}
