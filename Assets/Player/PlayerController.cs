using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;

    public float walkSpeed = 6.0f;
    public float sprintMultiplyer = 2.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    public float sensitivity = 1.0f;
    public float smoothing = 2.0f;

    public float scale = 1.0f;
    public float maxScale = 2.0f;
    public float minScale = 0.1f;

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }


    private Vector3 moveDirection = Vector3.zero;
    private Vector2 mouseLook;
    private Vector2 smoothV;

    void Update()
    {
        float sqrtScale = Mathf.Sqrt(scale);
        // Player movement
        if (characterController.isGrounded)
        {

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection *= walkSpeed * sqrtScale;

            if (Input.GetButton("Sprint") && moveDirection.z > 0)
            {
                moveDirection.z *= sprintMultiplyer;
            }
            
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed * sqrtScale;
            }

            moveDirection = transform.TransformDirection(moveDirection);
        }

        moveDirection.y -= gravity * Time.deltaTime * scale;
        characterController.Move(moveDirection * Time.deltaTime);

        // Player rotaion
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), 0);
        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
        mouseLook += smoothV;
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        gameObject.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, gameObject.transform.up);

        //Scale
        scale += Input.GetAxis("Mouse ScrollWheel");

        if (scale > maxScale)
            scale = maxScale;

        if (scale < minScale)
            scale = minScale;

        gameObject.transform.localScale = Vector3.one * scale;

        //Animator
        Vector3 velocity = transform.InverseTransformDirection(characterController.velocity);
        animator.SetFloat("ForwandSpeed", velocity.z / sqrtScale);
        animator.SetFloat("SideSpeed", velocity.x / sqrtScale);
        animator.SetBool("Grounded", characterController.isGrounded);
    }
}
