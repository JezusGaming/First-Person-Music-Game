using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MyPlayer : MonoBehaviour
{
	[Header("Enable Funtions")]
	public bool m_bCanSprint = false;
	public bool m_bCanJump = false;
	public bool m_bCanCrouch = false;
	public bool m_bCanProne = false;
	public bool m_bCanSlide = false;
	public bool m_bWillSlideOnSlopes = false;
	public bool m_bCanStrafe = false;

	[Header("KeyBindings")]
	public bool m_bHoldToJump;
	public KeyCode m_jump = KeyCode.Space;
	public bool m_bHoldToCrouch;
	public KeyCode m_crouch = KeyCode.C;
	public bool m_bHoldToProne;
	public KeyCode m_prone = KeyCode.Z;
	public bool m_bHoldToSprint;
	public KeyCode m_sprint = KeyCode.LeftShift;

	[Header("Camera Settings")]
	// Transform of the camera
	public Transform m_cameraTransform;
	//public Transform m_head;
	// Used to change how fast the camera moves between stances
	public float m_fCameraChangeStanceSpeed = 0.1f;
	// The height at which the camera is bound to
	public float m_fStandCameraYOffset = 0.65f;
	// Camera rotation X
	private float m_fRotX = 0.0f;
	// Camera rotation Y
	private float m_fRotY = 0.0f;
	// Used to set the FOV for the camera
	[Range(20.0f, 150.0f)]
	public float m_fFieldOfView;
	// Mouse X Sensitivity
	public float m_fXMouseSensitivity = 30.0f;
	// Mouse Y Sensitivity
	 public float m_fYMouseSensitivity = 30.0f;

	[Header("Ground Movement Settings")]
	// Used to go from sprinting to walking if ticked
	public bool m_bSprintToWalk = false;
	// Frame occuring factors
	public float m_fGravity = 20.0f;
	// Ground friction
	public float m_fFriction = 3;
	// Ground walk speed
	public float m_fWalkSpeed = 7.5f;
	// Ground run speed
	public float m_fRunSpeed = 10.0f;
	// Ground crouch speed
	public float m_fCrouchSpeed = 5.0f;
	// Ground prone speed
	public float m_fProneSpeed = 2.5f;
	// The speed at which the character's up axis gains when hitting jump
	public float m_fJumpSpeed = 8.0f;
	// Ground accel
	public float m_fGroundAcceleration = 10.0f;
	// Deacceleration that occurs when running on the ground
	public float m_fGroundDeceleration = 15.0f;
	// Stores the original friction
	private float m_fOriginalFriction;
	// Used for sprinting
	private bool m_bRun = false;
	// Vertical Movement
	private float m_fVerticalMovement;
	// Horizontal Movement
	private float m_fHorizontalMovement;

	[Header("Air Movement Settings")]
	// How much u accelerate in air when standing sill, is used when strafing is disabled
	public float m_fAirAcceleration = 10.0f;
	// Slows the player down oposite the direction they are jumping, is used when strafing is disabled
	public float m_fAirDeceleration = 100.0f;
	// How precise air control is, higher number means u will move directions in the air quicker, lower num recomened for strafing higher with strafing disabled
	public float m_fAirControl = 0.3f;
	// How fast acceleration occurs to get up to sideStrafeSpeed
	public float m_fSideStrafeAcceleration = 100.0f;
	// What the max speed to generate when side strafing
	public float m_fSideStrafeSpeed = 10.0f;
	// How much friction to apply when jumping
	public float m_fJumpFriction = 3.0f;
	private bool m_bDisableJump;
	private bool m_bIsJumping;

	[Header("Stance Settings")]
	// Used to set the camera up for crouch pos
	public float m_fCrouchCameraYOffset;
	// Used to set the camera up for prone pos
	public float m_fProneCameraYOffset;
	// Used to change the hitbox
	public float m_proneHeight;
	// Used to change the hitbox
	public float m_crouchHeight;
	// Used to change the hitbox
	private float m_standingHeight;
	// Used to check if no crouch, prone or sprint action is happening
	private bool m_bWalk = true;
	// Used to crouch
	private bool m_bCrouched = false;
	// Used to prone
	private bool m_bProned = false;

	[Header("Slope Settings")]
	public float m_fSlopeSpeed = 8.0f;
	public float m_fSlopeRayLength = 2.0f;
	// may need to be visible for sliding values
	public float m_fSlopeForce = 20.0f;
	private Vector3 m_v3HitPointNormal;
	private bool m_bIsSliding
	{
		get
		{
			if(FPSCC.isGrounded && m_bWillSlideOnSlopes && Physics.Raycast(transform.position, Vector3.down, out RaycastHit _slopeHit, 2f))
			{
				m_v3HitPointNormal = _slopeHit.normal;
				return Vector3.Angle(m_v3HitPointNormal, Vector3.up) > FPSCC.slopeLimit;
			}
			else
			{
				return false;
			}
		}
	}

	[Header("Slide Settings")]
	public float m_fSlideSpeed = 10.0f;
	public float m_fSlideSlopeSpeed = 0.2f;
	public float m_fSlideSlopeLimit = 20.0f;
	public float m_fSlideAcceleration = 1.0f;
	public float m_fSpeedToStartSlide = 5.0f;
	public float m_fSlideDeceleration = 2.0f;
	private bool m_bSlide;
	private bool m_bSlideStart;
	private float m_fOriginalSlideSpeed;
	private float m_fOldYPos;

	// Used to store the move direction normalised for the movment
	private Vector3 m_v3MoveDirectionNorm = Vector3.zero;
	// Stores the player velocity allowing for the player to have momentum
	private Vector3 m_v3PlayerVelocity = Vector3.zero;
	// Used to get the character controller values
	private CharacterController FPSCC;

	//--------------------------------------------------------------------------------
	// Awake used for initialization.
	//--------------------------------------------------------------------------------
	void Awake()
	{
		// Hide and lock the cursor
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		// Stores the original friction
		m_fOriginalFriction = m_fFriction;

		// Checks if the camera transform has been set
		if (m_cameraTransform == null)
		{
			// If the camera transform has not been set then use the main camera
			Camera mainCamera = Camera.main;
			if (mainCamera != null)
				m_cameraTransform = mainCamera.gameObject.transform;
		}
		// Moves the camera into a FPS postion in the player capsule
		m_cameraTransform.position = new Vector3(transform.position.x, transform.position.y + m_fStandCameraYOffset, transform.position.z);

		// Gets access to the player character controller
		FPSCC = GetComponent<CharacterController>();

		m_fOriginalSlideSpeed = m_fSlideSpeed;

		m_standingHeight = FPSCC.height;
	}

	//--------------------------------------------------------------------------------
	// Update is called once every frame. It allows the player to move as well as
	// changes the FOV, locks the camera and hides the cursor, moves the camera, 
	// and limits the max speed the player can go.
	//--------------------------------------------------------------------------------
	private void Update()
	{
		//Debug.Log(m_v3PlayerVelocity);
		// Sets the FOV of the camera
		Camera.main.fieldOfView = m_fFieldOfView;

		// Ensure that the cursor is locked into the screen 
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			if (Input.GetButtonDown("Fire1"))
				Cursor.lockState = CursorLockMode.Locked;
		}

		// Gets the mouses input to rotate the camera based on it
		m_fRotX -= Input.GetAxisRaw("Mouse Y") * m_fXMouseSensitivity * 0.02f;
		m_fRotY += Input.GetAxisRaw("Mouse X") * m_fYMouseSensitivity * 0.02f;

		// Clamp the X rotation and prevents gimbal lock
		if (m_fRotX < -90)
			m_fRotX = -90;
		else if (m_fRotX > 90)
			m_fRotX = 90;

		// Rotates the collider
		this.transform.rotation = Quaternion.Euler(0, m_fRotY, 0);
		// Rotates the camera
		m_cameraTransform.rotation = Quaternion.Euler(m_fRotX, m_fRotY, 0);

		// If the player is grounded then do ground move otherwise use airmove as they are in the air
		if (FPSCC.isGrounded)
			GroundMove();
		else if (!FPSCC.isGrounded)
			AirMove();

		if(m_bIsSliding)
		{
			m_bDisableJump = true;
			m_v3PlayerVelocity += new Vector3(m_v3HitPointNormal.x, -m_v3HitPointNormal.y, m_v3HitPointNormal.z) * m_fSlopeSpeed;
		}
		else if(!m_bIsSliding)
		{
			m_bDisableJump = false;
		}

		// Move the controller
		FPSCC.Move(m_v3PlayerVelocity * Time.deltaTime);

		if (FPSCC.isGrounded)
		{
			m_bIsJumping = false;
		}

		if (OnSlope() && !m_bIsJumping)
		{
			FPSCC.Move(Vector3.down * FPSCC.height / 2 * m_fSlopeForce * Time.deltaTime);
		}

		//Checks what stance the player is in and moves the camera to fit it
		if (m_bCrouched)
		{
			// Set the camera's position to the transform
			SetCameraYPos(m_fCrouchCameraYOffset);
		}
		else if (m_bProned)
		{
			// Set the camera's position to the transform
			SetCameraYPos(m_fProneCameraYOffset);
		}
		else
		{
			// Set the camera's position to the transform
			SetCameraYPos(m_fStandCameraYOffset);
		}

	}

	//--------------------------------------------------------------------------------
	// Sets the movement direction based on the input.
	//--------------------------------------------------------------------------------
	private void SetMoveDir()
	{
		// Sets the horizontal movement to the input
		m_fHorizontalMovement = Input.GetAxisRaw("Vertical");
		// Sets the vertical movement to the input
		m_fVerticalMovement = Input.GetAxisRaw("Horizontal");
	}

	//--------------------------------------------------------------------------------
	// Allows the player to control the character in the air, such as strafing which
	// allows the player to gain momentum by moving left and right in a specific way.
	//--------------------------------------------------------------------------------
	private void AirMove()
	{
		// Stores the direction the player wants to move
		Vector3 v3DesDir;
		// Used to accelerate and deccelerate the player
		float fAccel;
		// Sets the move direction based on player input
		SetMoveDir();
		// Sets the desired direction based on the movement
		v3DesDir = new Vector3(m_fVerticalMovement, 0, m_fHorizontalMovement);
		// Changes the desired direction from local space to world space
		v3DesDir = transform.TransformDirection(v3DesDir);
		// Sets the desired speed
		float fDesSpeed = v3DesDir.magnitude;

		// Changes the players stance from prone, crouch, sprint based on the input 
		ChangeStance();

		v3DesDir.Normalize();
		// Sets the normalized move direction
		m_v3MoveDirectionNorm = v3DesDir;

		// Checks if the dot product of player vel and the des direction is less then zero 
		// And accelerates or deccelerates based on the result 
		if (Vector3.Dot(m_v3PlayerVelocity, v3DesDir) < 0)
			fAccel = m_fAirDeceleration;
		else
			fAccel = m_fAirAcceleration;

		if(m_bCanStrafe)
		{
			// If the player is ONLY strafing left or right
			if (m_fVerticalMovement == 0 && m_fHorizontalMovement != 0)
			{
				// If the player is stafing increase the speed based on the variables
				if (fDesSpeed > m_fSideStrafeSpeed)
					fDesSpeed = m_fSideStrafeSpeed;
				fAccel = m_fSideStrafeAcceleration;
			}

			// If the player is ONLY strafing forward or backward
			if (m_fVerticalMovement != 0 && m_fHorizontalMovement == 0)
			{
				// If the player is stafing increase the speed based on the variables
				if (fDesSpeed > m_fSideStrafeSpeed)
					fDesSpeed = m_fSideStrafeSpeed;
				fAccel = m_fSideStrafeAcceleration;
			}
		}

		// Accelerates the player based on the values calculated above
		Accelerate(v3DesDir, fDesSpeed, fAccel);
		// If the air control is greater then zero then use the player air control movement
		// the higher air control the eaiser it is to move left right forward back without strafing
		//strafing can be used with if the air control is not too high as air controll makes the player move more in air based in movement input instead of mouse look / strafing
		if (m_fAirControl > 0)
			AirControl(v3DesDir, fDesSpeed);

		// Apply gravity
		m_v3PlayerVelocity.y -= m_fGravity * Time.deltaTime;
	}
	private void AirControl(Vector3 v3DesDir, float fDesSpeed)
	{
		float zspeed;
		float speed;
		float dot;
		float k;

		// Checks if there is movement as you can't control movement if not moving forward or backward
		if (Mathf.Abs(m_fVerticalMovement) < 0.001 || Mathf.Abs(fDesSpeed) < 0.001)
			return;
		// Stores the y player velocity
		zspeed = m_v3PlayerVelocity.y;
		// Sets the y player velocity to zero as the zspeed is storing the value
		m_v3PlayerVelocity.y = 0;

		// Sets the speed to the magnitude of the player velocity
		speed = m_v3PlayerVelocity.magnitude;
		// Normalizes the player vel
		m_v3PlayerVelocity.Normalize();

		// Sets dot to the Dot product of the player vel and desiered direction
		dot = Vector3.Dot(m_v3PlayerVelocity, v3DesDir);
		k = 32;
		// Calculates k which is used in changing the player vel
		k *= m_fAirControl * dot * dot * Time.deltaTime;

		// Change direction while slowing down
		if (dot > 0)
		{
			m_v3PlayerVelocity.x = m_v3PlayerVelocity.x * speed + v3DesDir.x * k;
			m_v3PlayerVelocity.y = m_v3PlayerVelocity.y * speed + v3DesDir.y * k;
			m_v3PlayerVelocity.z = m_v3PlayerVelocity.z * speed + v3DesDir.z * k;

			m_v3PlayerVelocity.Normalize();
			m_v3MoveDirectionNorm = m_v3PlayerVelocity;

		}
		// Adds the speed to the player velocity
		m_v3PlayerVelocity.x *= speed;
		m_v3PlayerVelocity.y = zspeed;
		m_v3PlayerVelocity.z *= speed;
	}

	//--------------------------------------------------------------------------------
	// Moves the player based on the movement input and stores the velocity to give
	// the feeling of gaining momentum, also makes the player slide down slopes and
	// lets them jump, crouch, prone, and sprint.
	//--------------------------------------------------------------------------------
	private void GroundMove()
	{
		// Stores the direction the player wants to move
		Vector3 v3DesDir;
		// Sets the move direction based on player input
		SetMoveDir();
		// Sets the desired direction based on the movement
		v3DesDir = new Vector3(m_fVerticalMovement, 0, m_fHorizontalMovement);
		// Changes the desired direction from local space to world space
		v3DesDir = transform.TransformDirection(v3DesDir);
		v3DesDir.Normalize();
		// Sets the normalized move direction
		m_v3MoveDirectionNorm = v3DesDir;
		// Sets the desired speed
		var fDesSpeed = v3DesDir.magnitude;

		// Reset the gravity velocity
		m_v3PlayerVelocity.y = -1f;
		//m_v3PlayerVelocity.y = -m_fGravity * Time.deltaTime;

		// Changes the players stance from prone, crouch, sprint based on the input
		ChangeStance();

		if (m_bCrouched)
		{
			Accelerate(v3DesDir, fDesSpeed *= m_fCrouchSpeed, m_fGroundAcceleration);
		}
		if (m_bProned)
		{
			Accelerate(v3DesDir, fDesSpeed *= m_fProneSpeed, m_fGroundAcceleration);
		}
		if (m_bWalk && !m_bCrouched && !m_bProned)
		{
			Accelerate(v3DesDir, fDesSpeed *= m_fWalkSpeed, m_fGroundAcceleration);
		}
		if (m_bRun && !m_bCrouched && !m_bProned)
		{
			Accelerate(v3DesDir, fDesSpeed *= m_fRunSpeed, m_fGroundAcceleration);
		}

		m_fFriction = m_fOriginalFriction;

		if(m_bCanJump && !m_bDisableJump)
		{
			if(m_bHoldToJump)
			{
				// Checks if the jump button is pressed and the player is not proned then jumps
				// Applies friction to the player to slow down their momentum HERE why is sprint jumping adding friction and not normal jumping, needs to check for 
				if (Input.GetKey(m_jump) && !m_bProned /*&& !m_bSliding*/)
				{
					m_v3PlayerVelocity.y = m_fJumpSpeed;
					m_bIsJumping = true;
					ApplyFriction(m_fJumpFriction);
				}
				else
					ApplyFriction(1.0f);
			}
			else if(!m_bHoldToJump)
			{
				// Checks if the jump button is pressed and the player is not proned then jumps
				// Applies friction to the player to slow down their momentum
				if (Input.GetKeyDown(m_jump) && !m_bProned /*&& !m_bSliding*/)
				{
					m_v3PlayerVelocity.y = m_fJumpSpeed;
					m_bIsJumping = true;
					ApplyFriction(m_fJumpFriction);
				}
				else
					ApplyFriction(1.0f);
			}
			
		}
		else
			ApplyFriction(1.0f);
	}

	private bool OnSlope()
	{
		RaycastHit _hit;
		if(Physics.Raycast(transform.position, Vector3.down, out _hit, FPSCC.height / 2 * m_fSlopeRayLength))
		{
			if(_hit.normal != Vector3.up)
			{
				return true;
			}
		}
		return false;
	}

	//--------------------------------------------------------------------------------
	// Applies friction to the player so they can slow down
	//
	// Param:
	//		t: Used to determine the amount of friction to apply
	//
	//--------------------------------------------------------------------------------
	private void ApplyFriction(float t)
	{
		// Sets the vec to equal the player vel
		Vector3 vec = m_v3PlayerVelocity;
		// Speed which will be used to calulate the friction speed
		float speed;
		// The new speed with friction applied
		float newspeed;
		float control;
		// The amount the speed should drop with the friction
		float drop;

		vec.y = 0.0f;
		speed = vec.magnitude;
		drop = 0.0f;

		// Only if the player is on the ground then apply friction
		// HERE could add a check if they want air friction
		if (FPSCC.isGrounded)
		{
			control = speed < m_fGroundDeceleration ? m_fGroundDeceleration : speed;
			drop = control * m_fFriction * Time.deltaTime * t;
		}
		// Sets the new speed based on the speed and drop
		newspeed = speed - drop;

		if (newspeed < 0)
			newspeed = 0;
		if (speed > 0)
			newspeed /= speed;
		// Changes the player vel to the new speed with friction
		m_v3PlayerVelocity.x *= newspeed;
		m_v3PlayerVelocity.z *= newspeed;
	}

	//--------------------------------------------------------------------------------
	// Accelerates the players velocity based on the values passed in.
	//
	// Param:
	//		v3DesDir: The desired direction the player should move
	//		fDesSpeed: The desired speed the player should move at
	//		fAccel: The rate the player should accelerate
	//--------------------------------------------------------------------------------
	private void Accelerate(Vector3 v3DesDir, float fDesSpeed, float fAccel)
	{
		float addspeed;
		float accelspeed;
		float currentspeed;

		// Sets the currecnt speed to the dot product of the player vel and des dir
		currentspeed = Vector3.Dot(m_v3PlayerVelocity, v3DesDir);
		// Add speed equals the desired speed minus the current speed
		addspeed = fDesSpeed - currentspeed;
		// If there is no speed being added exit this function
		if (addspeed <= 0)
			return;
		// Calculates the accel speed
		accelspeed = fAccel * Time.deltaTime * fDesSpeed;
		if (accelspeed > addspeed)
			accelspeed = addspeed;
		// Accelerats the players velocity
		m_v3PlayerVelocity.x += accelspeed * v3DesDir.x;
		m_v3PlayerVelocity.z += accelspeed * v3DesDir.z;
	}

	//--------------------------------------------------------------------------------
	// Lerps the camera between the current camera pos and the new offset, used when 
	// the player changes stance
	//
	// Param:
	//		yOffset: Used to change the y position of the camera
	//
	//--------------------------------------------------------------------------------
	private void SetCameraYPos(float yOffset)
	{
		// Moves the camera into a FPS desired y offset postion in the player capsule
		Vector3 m_v3NextCameraPos = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
		m_cameraTransform.position = Vector3.Lerp(m_cameraTransform.position, m_v3NextCameraPos, m_fCameraChangeStanceSpeed);
	}

	private void Crouch()
	{
		if(m_bHoldToCrouch)
		{
			if (Input.GetKey(m_crouch))
			{
				m_bCrouched = true;
				m_bProned = false;
				FPSCC.height = m_crouchHeight;
			}
			else if (m_bCrouched && !Physics.Raycast(m_cameraTransform.position, Vector3.up, 0.75f))
			{
				m_bCrouched = false;
				FPSCC.height = m_standingHeight;
			}
		}
		else if(!m_bHoldToCrouch)
		{
			if (Input.GetKeyDown(m_crouch) && !m_bCrouched)
			{
				m_bCrouched = true;
				m_bProned = false;
				FPSCC.height = m_crouchHeight;
			}
			else if (Input.GetKeyDown(m_crouch) && m_bCrouched && !Physics.Raycast(m_cameraTransform.position, Vector3.up, 0.75f))
			{
				m_bCrouched = false;
				FPSCC.height = m_standingHeight;
			}
		}
		
	}

	private void Prone()
	{
		if(FPSCC.isGrounded)
		{
			if(m_bHoldToProne)
			{
				if (Input.GetKey(m_prone))
				{
					m_bProned = true;
					m_bCrouched = false;
					FPSCC.height = m_proneHeight;
				}
				else if (m_bProned && !Physics.Raycast(m_cameraTransform.position, Vector3.up, 1.5f))
				{
					m_bProned = false;
					FPSCC.height = m_standingHeight;
				}
			}
			else if(!m_bHoldToProne)
			{
				if (Input.GetKeyDown(m_prone) && !m_bProned)
				{
					m_bProned = true;
					m_bCrouched = false;
					FPSCC.height = m_proneHeight;
				}
				else if (Input.GetKeyDown(m_prone) && m_bProned && !Physics.Raycast(m_cameraTransform.position, Vector3.up, 1.5f))
				{
					m_bProned = false;
					FPSCC.height = m_standingHeight;
				}
				RaycastHit _hit;
				Physics.Raycast(m_cameraTransform.position, Vector3.up, out _hit, 1.5f);
				//Debug.Log();
				//HERE prone detects somthing above
				//HERE and sliding on slopes contiunes which should stop if there is sprinting speed reaching low vel
				//HERE moving when sliding on slope allows to keep sliding up the slope and in directions
			}
		}
	}

	private void Sprint()
	{
		if(m_bHoldToSprint)
		{
			if (Input.GetKey(m_sprint) && !m_bSprintToWalk && !m_bSlide)
			{
				if (m_bProned && Physics.Raycast(m_cameraTransform.position, Vector3.up, 1.5f) || m_bCrouched && Physics.Raycast(m_cameraTransform.position, Vector3.up, 0.75f))
				{
					m_bRun = false;
				}
				else
				{
					m_bRun = true;
					m_bWalk = false;
					m_bProned = false;
					m_bCrouched = false;
					FPSCC.height = m_standingHeight;
				}
			}
			else if (!m_bSprintToWalk && m_bRun)
			{
				m_bWalk = true;
				m_bRun = false;
			}
			if (Input.GetKey(m_sprint) && m_bSprintToWalk)
			{
				m_bWalk = true;
				m_bRun = false;
			}
			else if (m_bSprintToWalk && m_bWalk)
			{
				m_bWalk = false;
				m_bRun = true;
			}
		}
		else if(!m_bHoldToSprint)
		{
			if (Input.GetKeyDown(m_sprint) && !m_bSprintToWalk && !m_bRun)
			{
				if (m_bProned && Physics.Raycast(m_cameraTransform.position, Vector3.up, 1.5f) || m_bCrouched && Physics.Raycast(m_cameraTransform.position, Vector3.up, 0.75f))
				{
					m_bRun = false;
				}
				else
				{
					m_bRun = true;
					m_bWalk = false;
					m_bProned = false;
					m_bCrouched = false;
					FPSCC.height = m_standingHeight;
				}
			}
			else if (Input.GetKeyDown(m_sprint) && !m_bSprintToWalk && m_bRun)
			{
				m_bWalk = true;
				m_bRun = false;
			}
			if (Input.GetKeyDown(m_sprint) && m_bSprintToWalk && !m_bWalk)
			{
				m_bWalk = true;
				m_bRun = false;
			}
			else if (Input.GetKeyDown(m_sprint) && m_bSprintToWalk && m_bWalk)
			{
				m_bWalk = false;
				m_bRun = true;
			}
		}
	}
	//Here split slide into 2 onslope and slide
	private void Slide()
	{
		if(FPSCC.isGrounded)
		{
			if(m_bRun && m_bCrouched)
			{
				m_bSlide = true;
			}
			// if crouch is held and player is over a certain mementum or velocity
			if(m_bCrouched && Mathf.Abs(m_v3PlayerVelocity.x) > m_fSpeedToStartSlide || m_bCrouched && Mathf.Abs(m_v3PlayerVelocity.z) > m_fSpeedToStartSlide)
			{
				//-------------if we are on a slideable slope then slide down slope-------------
				if (OnSlope() && Vector3.Angle(m_v3HitPointNormal, Vector3.up) > m_fSlideSlopeLimit)
				{
					Debug.Log(Vector3.Angle(m_v3HitPointNormal, Vector3.up));
					// Stores the direction the player wants to move
					Vector3 v3DesDir;
					// Sets the desired direction based on the velocity
					v3DesDir = new Vector3(m_v3PlayerVelocity.x, 0, m_v3PlayerVelocity.z);
					// Sets the desired speed
					var fDesSpeed = v3DesDir.magnitude;

					if (m_bSlideStart)
					{
						m_fOriginalSlideSpeed = m_fSlideSpeed;
						m_bSlideStart = false;
					}

					m_v3HitPointNormal = SlopeHit().normal;
					m_v3PlayerVelocity += new Vector3(m_v3HitPointNormal.x, -m_v3HitPointNormal.y, m_v3HitPointNormal.z) * m_fSlideSlopeSpeed;

					if (m_fOldYPos < transform.position.y)
					{
						m_fSlideSpeed -= m_fSlideDeceleration * Time.deltaTime;
					}
					else if (m_fOldYPos > transform.position.y)
					{
						m_fSlideSpeed = Vector3.Angle(m_v3HitPointNormal, Vector3.up);
					}

					if (m_fSlideSpeed <= 0)
					{
						m_fSlideSpeed = 0;
					}
					if (m_fSlideSpeed >= m_fOriginalSlideSpeed)
					{
						m_fSlideSpeed = m_fOriginalSlideSpeed;
					}

					Accelerate(v3DesDir, fDesSpeed * m_fSlideSpeed, m_fSlideAcceleration);

					m_fOldYPos = transform.position.y;
				}
				else
				{
					//---------increase speed or vel in the direction of last mevement inptu---------
					// Stores the direction the player wants to move
					Vector3 v3DesDir;
					// Sets the desired direction based on the velocity
					v3DesDir = new Vector3(m_v3PlayerVelocity.x, 0, m_v3PlayerVelocity.z);
					// Sets the desired speed
					var fDesSpeed = v3DesDir.magnitude;

					Accelerate(v3DesDir, fDesSpeed * m_fSlideSpeed, m_fSlideAcceleration);

					if (m_bSlideStart)
					{
						m_fOriginalSlideSpeed = m_fSlideSpeed;
						m_bSlideStart = false;
					}

					m_fSlideSpeed -= m_fSlideSpeed * Time.deltaTime;

					if (m_fSlideSpeed < 0)
					{
						m_fSlideSpeed = 0;
					}
				}
			}
			else if(!m_bCrouched || m_bCrouched && Mathf.Abs(m_v3PlayerVelocity.x) <= m_fSpeedToStartSlide || m_bCrouched && Mathf.Abs(m_v3PlayerVelocity.z) <= m_fSpeedToStartSlide)
			{
				if(!m_bSlideStart)
				{
					m_fSlideSpeed = m_fOriginalSlideSpeed;
				}
				m_bSlideStart = true;
				m_bSlide = false;
			}
		}
	}
	private RaycastHit SlopeHit()
	{
		RaycastHit _hit;
		if (Physics.Raycast(transform.position, Vector3.down, out _hit, FPSCC.height / 2 * m_fSlopeRayLength))
		{
			if (_hit.normal != Vector3.up)
			{
				return _hit;
			}
		}
		return _hit;
	}

	//--------------------------------------------------------------------------------
	// Changes the stance of the player from crouch to prone, standing, sprinting and
	// sliding.
	//
	// Param:
	//		v3DesDir: The desired direction the player should move
	//		fDesSpeed: The desired speed the player should move at
	//--------------------------------------------------------------------------------
	private void ChangeStance()
	{
		if(m_bCanProne)
		{
			Prone();
		}
		if(m_bCanSprint)
		{
			Sprint();
		}
		if(m_bCanCrouch)
		{
			Crouch();
		}
		if(m_bCanSlide)
		{
			Slide();
		}
	}
}
//HERE crouch/prone check up for objects need to be improved.
//HERE make it so if sliding not on slope and input opiste direction slowdown