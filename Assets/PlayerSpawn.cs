using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject player;

    public Vector2 min, max;

    void Start()
    {
        Vector2 pos = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
        PhotonNetwork.Instantiate(
            player.name, new Vector3(pos.x, transform.position.y, pos.y), Quaternion.identity);
    }

    public Vector3 Respawn() 
    {
        Vector2 pos = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
        return new Vector3(pos.x, transform.position.y, pos.y);
    }
}
