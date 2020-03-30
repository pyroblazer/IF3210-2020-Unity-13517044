﻿using System.Collections;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster gm;
    public AudioClip respawnAudio;

    void Start()
    {
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }
    }

    public Transform playerPrefab;
    public Transform spawnPoint;
    public float spawnDelay = 2;
    public Transform spawnPrefab;

    public IEnumerator RespawnPlayer ()
    {
        AudioSource.PlayClipAtPoint(respawnAudio, new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z), 0.5f);
        yield return new WaitForSeconds(spawnDelay);

        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        Transform clone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation) as Transform;
        Destroy(clone.gameObject, 3.0f);
    }

    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
        gm.StartCoroutine(gm.RespawnPlayer());
    }
}