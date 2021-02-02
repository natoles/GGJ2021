using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MovementProperties
{
    public float moveSpeed;
    public float topSpeed;
    public float minMoveInterval; //Min time interval in seconds between random movement
    public float maxMoveInterval; //Max interval interval in seconds between random movement
    public float linearDrag;
}

public class Animal : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    bool isDragging;
    bool isFighting;
    Vector3 baseScale;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;
    public Coroutine movementsHandlingCoroutine;
    Vector2 movement;
    //public bool inEnclosure = false;
    public Enclosure currentEnclosure;
    public MyGameManager gameManager;
    public int enclosureSlotUsed = 1; // 2 for Pigs
    public MovementState currentMovementState = MovementState.Standard;
    Animal animalTarget; //Target animal for the chase
    Animal animalChasing; //Animal currently chasing us
    protected string type;
    public Transform fightPrefab;

    protected MovementProperties currentMovementProperties;

    protected MovementProperties standardMovement;
    protected MovementProperties rageMovement;
    protected MovementProperties chaseMovement;
    protected MovementProperties runMovement;
    protected MovementProperties followMovement;
    protected Vector3 lastPosOnClicDown;


    //Pahtfinding variables
    Path path;
    float nextWaypointDistance = 5f;
    int currentWaypoint = 0;
    public bool reachedEndOfPath = true; //True if has finished pathfinding
    Seeker seeker;

    public enum MovementState
    {
        Standard,
        Rage,
        ChaseFight,
        ChaseFlee,
        Run,
        Follow,
    }

    protected virtual void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        movementsHandlingCoroutine = StartCoroutine(MovemmentsHandling());
        baseScale = transform.localScale;

        currentEnclosure = GameObject.Find("Enclosures/ExteriorEnclosure").GetComponent<Enclosure>();
        gameManager = GameObject.Find("MyGameManagerDesu").GetComponent<MyGameManager>();

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

        return !currentEnclosure.isExterior;
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

    public void SetVisibility(bool visibility)
    {
        GetComponentInChildren<SpriteRenderer>().enabled = visibility;
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

    public void ChaseFight(Animal animal)
    {
        if (currentEnclosure == null) return;

        if (currentMovementState != MovementState.ChaseFlee)
            reachedEndOfPath = true; //End current path

        animalTarget = animal;
        animalTarget.isChasedBy(gameObject.GetComponent<Animal>());
        currentMovementState = MovementState.ChaseFight;
    }

    public bool IsTargetReachable()
    {
        if (animalTarget != null){
            List<Animal> neighbors = currentEnclosure.GetAnimals();
            return neighbors.Contains(animalTarget);
        }
        return false;
    }

    public void UpdateTargetReachable()
    {
        if (!IsTargetReachable()) animalTarget = null;
    }

    public void ChaseFlee(Animal animal)
    {
        if (currentEnclosure == null) return;

        if (currentMovementState != MovementState.ChaseFight)
            reachedEndOfPath = true; //End current path

        animalTarget = animal;
        animalTarget.isChasedBy(gameObject.GetComponent<Animal>());
        currentMovementState = MovementState.ChaseFlee;
    }

    public void Follow(Animal animal)
    {
        if (currentEnclosure == null) return;

        if (currentMovementState != MovementState.Follow)
            reachedEndOfPath = true; //End current path

        animalTarget = animal;
        currentMovementState = MovementState.Follow;
    }

    #endregion

    public void isChasedBy(Animal animal)
    {
        animalChasing = animal;
    }

    public bool IsCalm()
    {
        return (currentMovementState == MovementState.Standard);
    }

    #region Action functions

    //Fight = fight animation + kill
    public void Fight(Animal victim)
    {
        isFighting = true;
        victim.rb.velocity = Vector2.zero;
        rb.velocity = Vector2.zero;
        Instantiate(fightPrefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
        StartCoroutine(FightRoutine(victim));
    }

    //Hides killer while fight anim, then kills the victim 
    private IEnumerator FightRoutine(Animal victim)
    {
        SetVisibility(false);
        victim.SetVisibility(false);
        yield return new WaitForSeconds(1f); //FightScript.lifespan
        SetVisibility(true);
        victim.SetVisibility(true);
        victim.Kill();
        isFighting = false;
        animalTarget = null;
        Calm();
    }

    //Kill this instance and respawns an other
    public void Kill()
    {
        SpawnInfo spawnInfo = new SpawnInfo();
        spawnInfo.type = type;
        spawnInfo.quantity = 1;
        spawnInfo.time = Time.time + 3f;

        gameManager.SpawnAnimal(spawnInfo);

        currentEnclosure.RemoveAnimal(this);
        Destroy(gameObject);
    }

    #endregion

    #region Movement

    public void ComputeMovement(Vector3 Target, MovementProperties mProperties)
    {
        MoveTo(Target);
        currentMovementProperties.moveSpeed = mProperties.moveSpeed;
        currentMovementProperties.topSpeed = mProperties.topSpeed;
        currentMovementProperties.minMoveInterval = mProperties.minMoveInterval;
        currentMovementProperties.maxMoveInterval = mProperties.maxMoveInterval;
    }

    //Standard movement
    public IEnumerator MovemmentsHandling()
    {
        while (true)
        {
            if (currentEnclosure != null && (reachedEndOfPath || currentMovementState == MovementState.ChaseFight || currentMovementState == MovementState.ChaseFlee))
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
                    case MovementState.ChaseFight:
                        ComputeMovement(animalTarget.transform.position, chaseMovement);
                        break;
                    case MovementState.ChaseFlee:
                        ComputeMovement(animalTarget.transform.position, chaseMovement);
                        break;
                    case MovementState.Follow:
                        ComputeMovement(animalTarget.transform.position, followMovement);
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

    #endregion  

    #region Drag and Drop

    public void resetPosition()
    {
        transform.position = lastPosOnClicDown;
    }

    //Start drag
    public void OnMouseDown()
    {
        lastPosOnClicDown = transform.position;
        isDragging = true;
        animator.SetBool("IsDrag", true);
        transform.localScale = baseScale * 1.3f;
        audioSource.Play();
        transform.gameObject.tag = "Drag";
        path = null;
        Cursor.visible = false;
        StopCoroutine(MovemmentsHandling());
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
        StartCoroutine(MovemmentsHandling());
    }

    #endregion

    //Callback for pathfinding generation
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    protected virtual void Update()
    {
        movement.x = Mathf.Clamp(rb.velocity.x, -1f, 1f);
        movement.y = Mathf.Clamp(rb.velocity.y, -1f, 1f);

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.magnitude);
        
        if (animalTarget != null) UpdateTargetReachable();

        //Drag and drop
        if (isDragging)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousePosition);

            path = null; //Cancel movement
            rb.velocity = Vector2.zero;

            if (Input.GetAxis("Mouse X") != 0)
                spriteRenderer.flipX = (Input.GetAxis("Mouse X") > 0);
        }

        if (currentMovementState == MovementState.Rage
             || currentMovementState == MovementState.Run
                || currentMovementState == MovementState.ChaseFight
                    || currentMovementState == MovementState.ChaseFlee)
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

        //----------------------- Fight -------------------------//

        if (animalTarget != null && !isFighting && currentMovementState == MovementState.ChaseFight)
        {
            if (Vector2.Distance(new Vector2(animalTarget.transform.position.x, animalTarget.transform.position.y)
                                    , new Vector2(transform.position.x, transform.position.y)) < 5f)
            {
                Fight(animalTarget);
            }
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
                rb.velocity = Vector2.zero;
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
