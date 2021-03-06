using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

    public GameObject spawn;
    public bool requireLanded = true;
    public int Index { get; set; }
    public int SnapshotIndex { get; set; }

    void Start()
    {
        if (spawn == null)
        {
            // default to closest
            float minDist = float.PositiveInfinity;
            foreach (GameObject respawn in GameObject.FindGameObjectsWithTag("Respawn"))
            {
                if (spawn == null) spawn = respawn;

                float dist = (transform.position - respawn.transform.position).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    spawn = respawn;
                }
            }
        }

        if (spawn.GetComponent<Renderer>() != null)
        {
            spawn.GetComponent<Renderer>().enabled = false;
        }
        if (spawn.GetComponent<Collider>() != null)
        {
            spawn.GetComponent<Collider>().enabled = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && (!requireLanded || Vector3.Angle(collision.contacts[0].normal, Vector3.down) < 80)) // disallow hitting a vertical surface to count as hitting the checkpoint
        {
            collision.gameObject.GetComponent<RespawnController>().CurrentCheckpoint = this;
        }
    }
}
