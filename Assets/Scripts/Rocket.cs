using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(LineRenderer))]
public class Rocket : MonoBehaviour
{
    [Header("Launch Settings")]
    [SerializeField]
    private float launchForceMultiplier = 10f;
    [SerializeField]
    private float maxDragDistance = 5f;
    [Header("Audio")]
    [SerializeField]
    private AudioSource launchSound;
    private Rigidbody2D rb;
    private LineRenderer lineRenderer;

    private Vector2 dragStart;

    private Vector2 startPos;
    private Quaternion startRot;

    private bool isLaunched;
    private bool isDragging;

    private Camera cam;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        cam = Camera.main;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        startPos = transform.position;
        startRot = transform.localRotation;
        GravityManager.Instance.SetRocket(this);
    }

    private void Update()
    {
        if (isLaunched) return;

        HandleInput();
        if (isDragging) RotateTowardMouse();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = GetMouseWorldPosition();
            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            if (hit != null && hit.gameObject == gameObject)
            {
                isDragging = true;
                dragStart = mousePos;
                lineRenderer.enabled = true;
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 currentPos = GetMouseWorldPosition();
            Vector2 clamped = Vector2.ClampMagnitude(dragStart - currentPos, maxDragDistance);

            // Draw line from rocket toward drag
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, (Vector2)transform.position + clamped);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector2 dragEnd = GetMouseWorldPosition();
            Vector2 launchVector = dragStart - dragEnd;
            Launch(launchVector);
            isDragging = false;
            lineRenderer.enabled = false;
        }
    }

    private void Launch(Vector2 force)
    {
        rb.isKinematic = false;
        rb.AddForce(force * launchForceMultiplier, ForceMode2D.Impulse);
        isLaunched = true;
        launchSound.Play();
    }

    private void RotateTowardMouse()
    {
        Vector2 mousePosition = GetMouseWorldPosition();
        Vector2 direction = mousePosition - (Vector2)transform.position;

        if (direction.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 270f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private Vector2 GetMouseWorldPosition()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("PickUp"))
        {
            Destroy(collision.gameObject);
        }
        if(collision.gameObject.CompareTag("Planet"))
        {
            transform.position = startPos;
            rb.velocity = Vector2.zero;
            transform.localRotation = startRot;
            isLaunched = false;
            rb.isKinematic = true;
        }
    }
}