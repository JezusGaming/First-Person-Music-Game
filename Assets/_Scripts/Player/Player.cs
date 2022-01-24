using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterController m_controller;

    public float m_mouseSensitivity = 100.0f;
    public float m_speed = 12.0f;
    public float m_gravity = -9.8f;
    public float m_jumpHeight = 3.0f;

    public Transform m_orientation;
    public Transform m_playerCamera;

    public Transform m_groundCheck;
    public float m_groundDistance = 0.4f;
    public LayerMask m_groundMask;

    private float m_xRotation = 0f;
    private float m_desiredX;

    public Vector3 m_velocity;

    private bool m_isGrounded;

    private Transform m_originalTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        m_originalTransform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Look();
        Movement();
    }

    void Movement()
    {
        m_isGrounded = Physics.CheckSphere(m_groundCheck.position, m_groundDistance, m_groundMask);
        
        if(m_isGrounded && m_velocity.y < 0)
        {
            m_velocity.y = -2f;
		}

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        m_controller.Move(move * m_speed *Time.deltaTime);

        if(Input.GetButtonDown("Jump") && m_isGrounded)
        {
            m_velocity.y = Mathf.Sqrt(m_jumpHeight * -2f * m_gravity);
		}

        m_velocity.y += m_gravity * Time.deltaTime;

        m_controller.Move(m_velocity * Time.deltaTime);
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * m_mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * m_mouseSensitivity * Time.deltaTime;

        Vector3 _rot = m_playerCamera.transform.localRotation.eulerAngles;
        m_desiredX = _rot.y + mouseX;

        m_xRotation -= mouseY;
        m_xRotation = Mathf.Clamp(m_xRotation, -90.0f, 90.0f);

        m_playerCamera.transform.localRotation = Quaternion.Euler(m_xRotation, m_desiredX, 0);
        m_orientation.transform.localRotation = Quaternion.Euler(0, m_desiredX, 0);
    }
}
