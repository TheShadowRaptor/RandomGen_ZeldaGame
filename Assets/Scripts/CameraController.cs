using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float heightOffset = 5;
    [SerializeField] private float forwardOffset = 5;
    [SerializeField] private float rightOffset = 5;

    [SerializeField] private float followSpeed = 1f;
    [SerializeField] private float spawnWaitTime = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        // Start Camera Offest
        GameObject player = PlayerController.Instance.gameObject;

        Vector3 targetPosition = player.transform.position;
        targetPosition.y += 4;
        targetPosition.x += -2;
        targetPosition.z += -2;

        transform.position = targetPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        // After timer is finished zoom into game camera veiw
        spawnWaitTime -= Time.deltaTime;
        if (spawnWaitTime <= 0) 
        {
            spawnWaitTime = 0;
            GameObject player = PlayerController.Instance.gameObject;

            Vector3 targetPosition = player.transform.position +
                player.transform.right * rightOffset +
                player.transform.up * heightOffset +
                player.transform.forward * forwardOffset;

            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }

    }
}
