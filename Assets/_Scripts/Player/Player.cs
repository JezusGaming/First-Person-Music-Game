using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterController m_controller;
    [Header("MouseSettings")]
    [SerializeField] private float m_mouseSensitivity = 100.0f;
    private float m_xRotation = 0f;
    private float m_desiredX;

    [Header("Controls")]
    [SerializeField] private KeyCode m_jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode m_crouchKey = KeyCode.LeftControl;

    [Header("Movement Options")]
    [SerializeField] private float m_speed = 12.0f;
    [SerializeField] private float m_gravity = -9.8f;
    [SerializeField] private float m_jumpHeight = 3.0f;
    // Should make get and set for velocity and change it to private
    public Vector3 m_velocity;

    [Header("Crouch Options")]
    [SerializeField] private float m_crouchSpeed = 6.0f;
    [SerializeField] private float m_crouchHeight = 1.5f;
    //[SerializeField] private float m_standHeight = 2.0f;
    //[SerializeField] private float m_crouchTime = 0.25f;
    private bool m_isCrouching;

    [Header("Transform Setup")]
    [SerializeField] private Transform m_orientation;
    [SerializeField] private Transform m_playerCamera;
    private Transform m_originalTransform;

    [Header("Ground Options")]
    [SerializeField] private Transform m_groundCheck;
    [SerializeField] private float m_groundDistance = 0.4f;
    [SerializeField] private LayerMask m_groundMask;
    private bool m_isGrounded;

    
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

    void Crouch()
    {
        if(!m_isCrouching && Input.GetKeyDown(m_crouchKey))
        {
            float _currentHeight = m_controller.height;
            m_controller.height = Mathf.Lerp(_currentHeight, m_crouchHeight, m_crouchSpeed * Time.deltaTime);
            if (m_controller.height == m_crouchHeight)
            {
                m_isCrouching = false;
            }
            m_isCrouching = true;
		}
        if(m_isCrouching)
        {
            float _currentHeight = m_controller.height;
            m_controller.height = Mathf.Lerp(_currentHeight, m_crouchHeight, m_crouchSpeed * Time.deltaTime);
            if(m_controller.height == m_crouchHeight)
            {
                m_isCrouching = false;
			}
        }
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

        if(Input.GetKeyDown(m_jumpKey) && m_isGrounded)
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
