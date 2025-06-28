using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class PlanetGravity : MonoBehaviour
{
    public float gravityBaseStrength = 5f;
    public float resizeSpeed = 0.5f;
    public float minScale = 0.5f;
    public float maxScale = 3f;

    private Camera cam;
    private bool isMouseOver;

    private void Start()
    {
        cam = Camera.main;
        GravityManager.Instance.RegisterPlanet(this);
    }

    private void OnDestroy()
    {
        if (GravityManager.Instance != null)
            GravityManager.Instance.UnregisterPlanet(this);
    }

    private void Update()
    {
        HandleMouseHover();
        HandleScaling();
    }

    private void HandleMouseHover()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        isMouseOver = (hit != null && hit.gameObject == gameObject);
    }

    private void HandleScaling()
    {
        if (!isMouseOver) return;

        Vector3 scale = transform.localScale;

        if (Input.GetMouseButton(0))
            scale += Vector3.one * resizeSpeed * Time.deltaTime;
        else if (Input.GetMouseButton(1))
            scale -= Vector3.one * resizeSpeed * Time.deltaTime;

        scale = Vector3.Max(Vector3.one * minScale, Vector3.Min(Vector3.one * maxScale, scale));
        transform.localScale = scale;
    }
}