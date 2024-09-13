using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform cam;
    [SerializeField] float moveSpeed = 20f;
    [SerializeField] float scrollSpeed = 10f;
    [SerializeField] float rotateSpeed = 100f;
    [SerializeField] float panSpeed = 20f;

    [HorizontalLine]

    [SerializeField] float minY = 20;
    [SerializeField] float maxY = 80;

    Vector3 dragOrigin;

    void Update()
    {
        Move();
        Scroll();
        Rotate();
        Pan();
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(h, 0, v);
        cam.Translate(direction * moveSpeed * Time.deltaTime, Space.Self);

        // Clamp the Y position of the camera between minY and maxY
        Vector3 pos = cam.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        cam.position = pos;
    }

    void Scroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 direction = cam.forward * scroll * scrollSpeed;
        cam.Translate(direction, Space.World);

        // Clamp the Y position of the camera between minY and maxY after scrolling
        Vector3 pos = cam.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        cam.position = pos;
    }

    void Rotate()
    {
        if (Input.GetMouseButton(1))
        {
            float h = Input.GetAxis("Mouse X");
            cam.RotateAround(cam.position, Vector3.up, h * rotateSpeed * Time.deltaTime);
        }
    }

    void Pan()
    {
        if (Input.GetMouseButtonDown(2)) dragOrigin = Input.mousePosition;
        if (Input.GetMouseButton(2))
        {
            Vector3 difference = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
            Vector3 move = new Vector3(difference.x * panSpeed, difference.y * panSpeed, 0);
            cam.Translate(move, Space.Self);
            dragOrigin = Input.mousePosition;
        }
    }
}
