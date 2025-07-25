using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class PlanetGravity : MonoBehaviour
{
    [Header("Gravity")]
    public float gravityBaseStrength = 5f;

    private Camera cam;
    private bool isMouseOver;
    private bool isDragging;
    private Vector3 dragOffset;
    [SerializeField]
    Rocket rocket;

    private void Start()
    {
        cam = Camera.main;
        GravityManager.Instance.RegisterPlanet(this);
    }
    //Not sure I will be destroying the planets
    private void OnDestroy()
    {
        if (GravityManager.Instance != null)
            GravityManager.Instance.UnregisterPlanet(this);
    }

    private void Update()
    {
        HandleMouseHover();
        HandleDragging();
       
    }

    private void HandleMouseHover()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        isMouseOver = (hit != null && hit.gameObject == gameObject);
    }

    private void HandleDragging()
    {
        if (rocket.isLaunched) //Drag and drop for the planets
        {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            if (Input.GetMouseButtonDown(0) && isMouseOver)
            {
                isDragging = true;
                dragOffset = transform.position - mouseWorldPos;
            }

            if (Input.GetMouseButton(0) && isDragging)
            {
                transform.position = mouseWorldPos + dragOffset;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }
    }
}
