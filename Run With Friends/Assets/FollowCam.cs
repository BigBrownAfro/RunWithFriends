using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    Player player;
    Cinemachine.CinemachineVirtualCamera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        StartCoroutine(FindPlayer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator FindPlayer()
    {
        while (!player)
        {
            Debug.Log("Didn't find player");
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            yield return null;
        }
        cam.Follow = player.transform;
    }
}
