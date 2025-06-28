using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
    public class PlanetGravity : MonoBehaviour
    {
        public float gravityBaseStrength = 5f;
        public float resizeSpeed = 2f;
        public float minScale = 0.5f;
        public float maxScale = 5f;

        private Rocket rocket;
        private Camera cam;
        private bool isMouseOver;

    private void Start()
    {
        rocket = FindObjectOfType<Rocket>();
        cam = Camera.main;
    }

        private void FixedUpdate()
        {
            if (rocket == null) return;

            Vector2 direction = transform.position - rocket.transform.position;
            float distance = direction.magnitude;
            if (distance < 0.01f) return;

            float scaleFactor = transform.localScale.x; // Assume uniform scaling
            float gravityStrength = gravityBaseStrength * scaleFactor;

            Vector2 force = direction.normalized * gravityStrength / Mathf.Pow(distance, 2);
            rocket.GetComponent<Rigidbody2D>().AddForce(force);
        }

        private void Update()
        {
        
        if(Input.GetKeyDown(KeyCode.R))
        {
            //for testing, remove later
            SceneManager.LoadScene(0);
        }
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

            if (Input.GetMouseButton(0)) // Left click = grow
            {
                scale += Vector3.one * resizeSpeed * Time.deltaTime;
            }
            else if (Input.GetMouseButton(1)) // Right click = shrink
            {
                scale -= Vector3.one * resizeSpeed * Time.deltaTime;
            }

            scale = Vector3.Max(Vector3.one * minScale, Vector3.Min(Vector3.one * maxScale, scale));
            transform.localScale = scale;
        }
    }
