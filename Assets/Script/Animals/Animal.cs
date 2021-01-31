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
    //public bool inEnclosure = false;
    public Enclosure currentEnclosure;
    public int enclosureSlotUsed = 1; // 2 for Pigs
    public MovementState currentMovementState = MovementState.Standard;
    Animal animalTarget; //Target animal for the chase

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
    float nextWaypointDistance = 5f;
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

        currentEnclosure = GameObject.Find("Enclosures/ExteriorEnclosure").GetComponent<Enclosure>();

        standardMovement = new MovementProperties();
        standardMovement.moveSpeed = 10000f;
        standardMovement.topSpeed = 10f;
        standardMovement.minMoveInterval = 4f;
        standardMovement.maxMoveInterval = 7f;
        standardMovement.linearDrag = 4f;

        rageMovement = new MovementProperties();
        rageMovement.moveSpeed = 30000f;
        rageMovement.topSpeed = 30f;
        rageMovement.minMoveInterval = 1f;
        rageMovement.maxMoveInterval = 2f;
        rageMovement.linearDrag = 3f;

        chaseMovement = new MovementProperties();
        chaseMovement.moveSpeed = 20000f;
        chaseMovement.topSpeed = 20f;
        chaseMovement.minMoveInterval = 0.5f;
        chaseMovement.maxMoveInterval = 0.5f;
        chaseMovement.linearDrag = 5f;

        runMovement = new MovementProperties();
        runMovement.moveSpeed = 40000f;
        runMovement.topSpeed = 40f;
        runMovement.minMoveInterval = 0.1f;
        runMovement.maxMoveInterval = 0.1f;
        runMovement.linearDrag = 5f;

        currentMovementProperties = new MovementProperties();
        currentMovementProperties.moveSpeed = standardMovement.moveSpeed;
        currentMovementProperties.topSpeed = standardMovement.topSpeed;
        currentMovementProperties.minMoveInterval = standardMovement.minMoveInterval;
        currentMovementProperties.maxMoveInterval = standardMovement.maxMoveInterval;
        currentMovementProperties.linearDrag = standardMovement.linearDrag;
    }

    public bool IsInEnclosure()
    {
        if (currentEnclosure == null)
        {
            return false;
        }
        return true;
    }

    //Move an animal to the target position
    public void MoveTo(Vector3 target)
    {
        seeker.StartPath(rb.position, target, OnPathComplete);
    }

    public void EnterEnclosure(Enclosure enclosure)
    {
        currentEnclosure = enclosure;
        //inEnclosure = !currentEnclosure.isExterior;
    }

    public void LeaveEnclosure()
    {
        //inEnclosure = !currentEnclosure.isExterior;
    }

    #region Behavior functions

    //Start the RageState coroutine
    public void Enrage()
    {
        if (currentMovementState != MovementState.Rage) 
            reachedEndOfPath = true; //End current path

        animator.SetBool("IsRage", true);
        currentMovementState = MovementState.Rage;
    }

    //End the RageState coroutine
    public void Calm()
    {
        if (currentMovementState != MovementState.Standard)
            reachedEndOfPath = true; //End current path

        animator.SetBool("IsRage", false);
        currentMovementState = MovementState.Standard;
    }

    public void Run()
    {
        if (currentEnclosure == null) return;

        if (currentMovementState != MovementState.Run)
            reachedEndOfPath = true; //End current path

        currentMovementState = MovementState.Run;
    }

    public void Chase(Animal animal)
    {
        if (currentEnclosure == null) return;

        if (currentMovementState != MovementState.Chase)
            reachedEndOfPath = true; //End current path

        animalTarget = animal;
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
            if (currentEnclosure != null && (reachedEndOfPath || currentMovementState == MovementState.Chase))
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
                        ComputeMovement(animalTarget.transform.position, chaseMovement);
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

        if (currentMovementState == MovementState.Rage
             || currentMovementState == MovementState.Run
                || currentMovementState == MovementState.Chase)
        {
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else if (isDragging)
        {
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }

        /*
        transform.position = new Vector3 (transform.position.x, transform.position.y, Mathf.Abs(transform.position.y));
        rb.position = new Vector3(rb.position.x, rb.position.y, 0);
        */
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
                    if (distance < 0.7f) currentWaypoint++;
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
