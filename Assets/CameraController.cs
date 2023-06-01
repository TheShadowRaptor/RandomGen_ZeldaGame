using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float hightOffset = 5;
    [SerializeField] private float fowardOffset = 5;
    [SerializeField] private float rightOffset = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject player = PlayerController.Instance.gameObject;
        this.gameObject.transform.LookAt(player.transform.position);
        Vector3 pos = this.gameObject.transform.position;
        pos.x = player.transform.position.x + rightOffset;
        pos.y = player.transform.position.y + hightOffset;
        pos.z = player.transform.position.z + fowardOffset;
        this.gameObject.transform.position = pos;
    }
}
