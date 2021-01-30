using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Animal : MonoBehaviour
{
    Rigidbody2D rb;
    public Animator animator;
    public float moveSpeed = 2000f;
    bool isDragging;
    

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
    }

    public void OnMouseUp()
    {
        isDragging = false;
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
    }

    private void FixedUpdate()
    {
        //Needs to be at the end
        #region PahtFinding

        if (path != null)
        {

            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * moveSpeed * Time.deltaTime;

            rb.AddForce(force);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nexWaypointDistance)
            {
                currentWaypoint++;
            }
        }

        

        

        #endregion
    }


}
