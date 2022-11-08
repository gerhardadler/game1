using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform playerBody;
    public MouseLook MouseLook;

    public float speed = 12f;
    public float sprintMultiplier = 1.5f;
    public float friction = 1f;
    public float gravity = -9.81f;
    public float flipSpeed = 3f;
    public float jumpHeight = 5f;
    public float airSpeedMultiplier = 0.5f;
    public float airFrictionMultiplier = 0.5f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    bool canJump = true;

    bool isPlayerFalling()
    {
        if (gravity < 0)
        {
            return velocity.y > 0;
        } 
        else
        {
            return velocity.y < 0;
        }
    }

    IEnumerator RotatePlayerForSeconds(float seconds, float degrees)
    {
        float origRotation = transform.eulerAngles.z;
        float secondRotation = degrees / seconds;
        Vector3 origDirection = transform.forward;
        float cameraSecondRotation = (MouseLook.xRotation * -2) / seconds; // rotates to make camera stay pointing the direction
        while (seconds > 0)
        {
            playerBody.Rotate(origDirection, secondRotation * Time.deltaTime, Space.World);
            MouseLook.xRotation += cameraSecondRotation * Time.deltaTime;
            seconds -= Time.deltaTime;
            yield return null;
        }

        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            origRotation + 180
        );
        canJump = true;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && !isPlayerFalling())
        {
            velocity.y = 0f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        if (isGrounded)
        {
            if (Input.GetButton("Fire3")) {
                speed *= sprintMultiplier;
            }
            velocity.x += (x * transform.right.x + z * transform.forward.x) * speed - velocity.x * friction;
            velocity.z += (z * transform.forward.z + x * transform.right.z) * speed - velocity.z * friction;
            if (Input.GetButton("Fire3")) {
                speed /= sprintMultiplier;
            }
        }
        else
        {
            speed *= airSpeedMultiplier;
            friction *= airFrictionMultiplier;
            velocity.x += (x * transform.right.x + z * transform.forward.x) * speed - velocity.x * friction;
            velocity.z += (z * transform.forward.z + x * transform.right.z) * speed - velocity.z * friction;
            speed /= airSpeedMultiplier;
            friction /= airFrictionMultiplier;
        }

        if (Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            canJump = false;
            StartCoroutine(RotatePlayerForSeconds(flipSpeed, 180));
            gravity *= -1;
            velocity.y = gravity;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
