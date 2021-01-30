using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Animal : MonoBehaviour
{
    Rigidbody2D rb;
    public Animator animator;
    public float moveSpeed;
    public float topSpeed;
    bool isDragging;
    Vector3 baseScale;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;
    protected IEnumerator rageCoroutine;
    protected IEnumerator idleCoroutine;
    Vector2 movement;
    public float minMoveInterval = 10000f; //Min time interval in seconds between random movement
    public float maxMoveInterval = 10000f; //Max interval interval in seconds between random movement
    protected bool inEnclosure;
    public Enclosure currentEnclosure;

    //Pahtfinding variables
    Path path;
    public float nexWaypointDistance = 3f;
    int currentWaypoint = 0;
    protected bool reachedEndOfPath = true;
    Seeker seeker;


    protected virtual void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        idleCoroutine = IdleState();
        StartCoroutine(idleCoroutine);
        baseScale = transform.localScale;
    }

    //Move an animal to the target position
    public void MoveTo(Vector3 target)
    {
        seeker.StartPath(rb.position, target, OnPathComplete);
    }

    public void ComputeAnimalMovement()
    {
        if (reachedEndOfPath)
        {
            if (inEnclosure)
            {
                MoveTo(currentEnclosure.RandomPoint());
            }
            else
            {
                //Outside
            }
        }
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

    //Start the RageState coroutine
    public void Enrage()
    {
        reachedEndOfPath = true;
        animator.SetBool("IsRage", true);
        //rageCoroutine = RageState();
        //StartCoroutine(rageCoroutine);
    }

    //End the RageState coroutine
    public void Calm()
    {
        animator.SetBool("IsRage", false);
        StopCoroutine(rageCoroutine);
    }

    protected virtual IEnumerator RageState()
    {
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator IdleState()
    {
        while (true)
        {
            ComputeAnimalMovement();
            yield return new WaitForSeconds(Random.Range(minMoveInterval, maxMoveInterval));
        }

    }


    //Callback
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
        ComputeAnimalMovement();
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
            }
            else
            {
                reachedEndOfPath = false;

                Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
                Vector2 force = direction * moveSpeed * Time.deltaTime;

                rb.AddForce(force);

                float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

                if (currentWaypoint == path.vectorPath.Count - 1)
                {
                    if (distance < 0.7f) currentWaypoint++;
                }
                else
                {
                    if (distance < nexWaypointDistance) currentWaypoint++;
                }

            }
        }
        #endregion

        if (rb.velocity.magnitude > topSpeed)
            rb.velocity = rb.velocity.normalized * topSpeed;
            
    }


}
