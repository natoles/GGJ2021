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


    //Pahtfinding variables
    Path path;
    public Transform target;
    public float nexWaypointDistance = 3f;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    Seeker seeker;

    Vector2 movement;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        baseScale = transform.localScale;

        MoveTo(target.position);
    }

    public void MoveTo(Vector3 target)
    {
        seeker.StartPath(rb.position, target, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    public void OnMouseDown()
    {
        isDragging = true;
        animator.SetBool("isRage", true);

        transform.localScale = baseScale * 1.3f;
    }

    public void OnMouseUp()
    {
        isDragging = false;
        MoveTo(target.position);
        animator.SetBool("isRage", false);

        transform.localScale = baseScale;
    }

    void Update()
    {
        movement.x = Mathf.Clamp(rb.velocity.x, -1f, 1f);
        movement.y = Mathf.Clamp(rb.velocity.y, -1f, 1f);

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.magnitude);

        if (isDragging)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousePosition);

            path = null; //Cancel movement
            rb.velocity = new Vector2(0, 0);
        }

        if (isDragging && Input.GetAxis("Mouse X") != 0)
            spriteRenderer.flipX = (Input.GetAxis("Mouse X") > 0);
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
