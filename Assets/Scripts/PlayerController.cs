using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera mainCamera;
    Rigidbody2D playerBody;
    private StatsController statsController;
    public bool mouseLook = true;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = GetComponentInChildren<Rigidbody2D>();
        statsController = GetComponent<StatsController>();
    }

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(horizontalInput, verticalInput);

        playerBody.velocity += (movement * statsController.MoveSpeed);

        if (mouseLook)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
            Vector2 direction = new Vector2(mousePosition.x - playerBody.transform.position.x, mousePosition.y - playerBody.transform.position.y);

            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
            targetRotation = Quaternion.RotateTowards(playerBody.transform.rotation, targetRotation, statsController.RotateSpeed * Time.fixedDeltaTime);
            playerBody.transform.rotation = targetRotation;
        }
        else if (!mouseLook && movement != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, movement);
            targetRotation = Quaternion.RotateTowards(playerBody.transform.rotation, targetRotation, statsController.RotateSpeed * Time.fixedDeltaTime);
            playerBody.transform.rotation = targetRotation;
        }
    }

    void LateUpdate()
    {
        mainCamera.transform.position = new Vector3(playerBody.transform.position.x, playerBody.transform.position.y, playerBody.transform.position.z - 30);

        if (Input.GetKeyDown(KeyCode.Tab)){
            mouseLook = !mouseLook;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject.FindObjectOfType<PauseGameUIController>().Toggle();
        }

        if (Input.GetKeyDown(KeyCode.Insert))
        {
            statsController.SaveStats();
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            statsController.LoadStats();
        }
    }
}
