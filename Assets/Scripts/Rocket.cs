using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Rocket : MonoBehaviour
{
    //Script that handles launching the rocket with a pinball-like system
    [Header("Launch Settings")]
    [SerializeField]
    private float launchForceMultiplier = 10f;

    private Rigidbody2D rb;
    private Vector2 dragStart;
    private bool isLaunched;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.isKinematic = true;
    }

    private void Update()
    {
        if (isLaunched) return;

        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragStart = GetMouseWorldPosition();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 dragEnd = GetMouseWorldPosition();
            Vector2 launchVector = dragStart - dragEnd;

            Launch(launchVector);
        }
    }

    private void Launch(Vector2 force)
    {
        rb.isKinematic = false;
        rb.AddForce(force * launchForceMultiplier, ForceMode2D.Impulse);
        isLaunched = true;
    }

    private Vector2 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}