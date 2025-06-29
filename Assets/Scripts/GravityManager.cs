using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GravityManager : MonoBehaviour
{
    public static GravityManager Instance;

    private List<PlanetGravity> planets = new List<PlanetGravity>();
    private Rocket rocket;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterPlanet(PlanetGravity planet)
    {
        if (!planets.Contains(planet))
            planets.Add(planet);
    }

    public void UnregisterPlanet(PlanetGravity planet)
    {
        planets.Remove(planet);
    }

    public void SetRocket(Rocket r)
    {
        rocket = r;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            LevelManager.Instance.pickupsCollected = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    private void FixedUpdate()
    {
        if (rocket == null) return;

        Rigidbody2D rb = rocket.GetComponent<Rigidbody2D>();
        Vector2 rocketPos = rocket.transform.position;

        foreach (var planet in planets)
        {
            Vector2 direction = (Vector2)planet.transform.position - rocketPos;
            float distance = direction.magnitude;
            if (distance < 0.01f) continue;

            float scaleFactor = planet.transform.localScale.x;
            float gravityStrength = planet.gravityBaseStrength * scaleFactor;

            Vector2 force = direction.normalized * gravityStrength / Mathf.Pow(distance, 2);
            rb.AddForce(force);
        }
    }
}

