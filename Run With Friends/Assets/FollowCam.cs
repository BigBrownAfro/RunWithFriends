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
            try
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            }
            catch (System.Exception)
            {

                Debug.Log("Player not spawned yet. Follow cam failed to follow.");
            }
            yield return null;
        }
        cam.Follow = player.transform;
    }
}
