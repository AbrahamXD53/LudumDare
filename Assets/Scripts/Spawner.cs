using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    public int spawnerCounter = 0;
    public int playerCounter = 0;
    public PartyController controller;
    public Color[] colors;
    private Camera cam;

    private void Start()
    {
        controller = GetComponent<PartyController>();
        cam = GetComponentInChildren<Camera>();
    }

    private int getNextSpawner(int counter, Transform[] spawnPoints)
    {
        return (counter < spawnPoints.Length) ? counter++ : 0;
    }

	void createPlayer() {
        if (spawnPoints.Length > 0)
        {
            spawnerCounter = getNextSpawner(spawnerCounter, spawnPoints);
            GameObject player = Instantiate(playerPrefab, new Vector2(spawnPoints[spawnerCounter].position.x, spawnPoints[spawnerCounter].position.y), Quaternion.identity);
            player.name += "" + playerCounter;
            player.transform.parent = gameObject.transform;
            player.transform.localScale = new Vector3(50f, 50f, 1f);
            player.GetComponent<SpriteRenderer>().sortingLayerName = "Front";
            player.GetComponent<SpriteRenderer>().sortingOrder = 1;
            player.GetComponent<CharacterMovementController>().controllers = controller;
            player.GetComponent<SpriteRenderer>().color = colors[playerCounter % colors.Length];
            CameraLocator cl = cam.GetComponent<CameraLocator>();
            if(cl != null)
                cl.targets.Add(player.transform);
            CharacterMovementController cmc = player.GetComponent<CharacterMovementController>();
            cmc.playerNumber = playerCounter;
            playerCounter++;
        }
	}

    // Update is called once per frame
    private void Update()
    {
        if(controller != null && controller.players.Count > playerCounter)
        {
            // we need to create a new player
            createPlayer();
        }
    }
}
