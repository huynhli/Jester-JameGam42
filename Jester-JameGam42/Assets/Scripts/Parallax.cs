using UnityEngine;
using UnityEngine.Tilemaps;

public class Parallax : MonoBehaviour
{
    [Header("Parallax Settings")]
    public Camera cam;
    public float parallaxEffect;
    
    [Header("Auto Movement")]
    public bool autoMove = true;
    public float autoMoveSpeed = 2f;
    
    private float autoOffset = 0f;
    private float tileWidth = 18f;
    private Vector3 startPos;
    private Tilemap tilemap;
    private TilemapRenderer tilemapRenderer;

    void Start()
    {
        startPos = transform.position;
        tilemap = GetComponent<Tilemap>();
        tilemapRenderer = GetComponent<TilemapRenderer>();
        
        // If tileWidth not set, try to calculate from bounds
        if (tileWidth <= 0)
        {
            tileWidth = tilemapRenderer.bounds.size.x;
        }
        
        // If no camera assigned, use main camera
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    void Update()
    {
        // Auto movement
        if (autoMove)
        {
            autoOffset += autoMoveSpeed * Time.deltaTime;
        }

        // Calculate parallax position
        float camPosX = cam.transform.position.x + autoOffset;
        float parallaxX = camPosX * parallaxEffect;

        // Apply parallax movement
        Vector3 newPos = new Vector3(startPos.x - parallaxX, transform.position.y, transform.position.z);
        transform.position = newPos;

        // Loop tilemap
        if (transform.position.x <= -18f)
        {
            transform.position = new Vector3(0f, transform.position.y, transform.position.z);
            startPos.x += 18f;
        }

    }
}