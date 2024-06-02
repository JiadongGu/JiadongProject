using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] float moveSpeed = 20f;
    [SerializeField] float scrollSpeed = 10f;
    [SerializeField] float rotateSpeed = 100f;
    [SerializeField] float panSpeed = 20f;
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
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.Self);
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, pos.y, pos.y); // Maintain the current y position
        transform.position = pos;
    }

    void Scroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 direction = transform.forward * scroll * scrollSpeed;
        transform.Translate(direction, Space.World);
    }

    void Rotate()
    {
        if (Input.GetMouseButton(1))
        {
            float h = Input.GetAxis("Mouse X");
            transform.RotateAround(transform.position, Vector3.up, h * rotateSpeed * Time.deltaTime);
        }
    }

    void Pan()
    {
        if (Input.GetMouseButtonDown(2)) dragOrigin = Input.mousePosition;
        if (Input.GetMouseButton(2))
        {
            Vector3 difference = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
            Vector3 move = new Vector3(difference.x * panSpeed, difference.y * panSpeed, 0);
            transform.Translate(move, Space.Self);
            dragOrigin = Input.mousePosition;
        }
    }
}
