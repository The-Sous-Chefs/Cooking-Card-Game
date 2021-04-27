using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThirdPersonMovement : MonoBehaviour
{

    public CharacterController controller;
    public Transform cam;

    public float speed = 6f;
    public float sprintSpeed = 12f;
    public float gravity = -19.81f;
    public float jumpHeight = 3f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    Vector3 velocity;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;


    public float buildingDistance = 0.2f;
    public LayerMask buildingMask;

    public GameObject player;
    
    private void Start()
    {
        /**
        if (Globals.player != null)
        {
            player.transform.position = Globals.player.transform.position;
            player.transform.rotation = Globals.player.transform.rotation;
            Destroy(Globals.player);
            Globals.player = player;
        }
        **/
    }

    // Update is called once per frame
    void Update()
    {
        bool inBuilding = Physics.CheckSphere(groundCheck.position, buildingDistance, buildingMask);
        if (inBuilding && Input.GetButtonDown("Interact"))
        {
            SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
        }

        bool isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        var direction = new Vector3(horizontal, 0f, vertical).normalized;


        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            var effectiveSpeed = speed;
            if (Input.GetButton("Sprint")) effectiveSpeed += sprintSpeed;
            var moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * effectiveSpeed * Time.deltaTime);

        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
    }
}
