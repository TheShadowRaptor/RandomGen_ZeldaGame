using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] List<Collider> visionColliders = new List<Collider>();
    PlayerController player;
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance.gameObject.GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool PlayerIsSeen() 
    {
        Vector3 newDist = player.gameObject.transform.position;
        if (Vector3.Distance(transform.position, newDist) < 5 ) 
        {
            return true;
        }
        return false;
    }
}
