using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerScript : MonoBehaviour
{

    [Header("Spawner Objects")]
    public GameObject[] Target;
    
    [Header("Others")]
    public Button StartButton;
    public BoxCollider2D SpawnerCollider;
    public float scale = 1f;
    public float particleScale = 0.1f;

    bool isStart = false;
    float minSpawnDelay = 0.25f;
    float maxSpawnDelay = 0.4f;
    float minAngle = -15f;
    float maxAngle = 15f;
    float minForce = 7f;
    float maxForce = 12f;
    float maxLifetime = 5f;

    void Start()
    {
        for (int i = 0; i < Target.Length; i++){
            Target[i].gameObject.SetActive(false);
        }
    }

    public void onStartGame(){
        isStart = true;
        StartButton.gameObject.SetActive(false);
        StartCoroutine(Spawner());
    }

    IEnumerator Spawner(){
        yield return new WaitForSeconds(2f);

        while (isStart){
            GameObject Spawn = Target[Random.Range(0, Target.Length)];

            Vector3 position = new Vector3{
                x = Random.Range(SpawnerCollider.bounds.min.x, SpawnerCollider.bounds.max.x),
                y = Random.Range(SpawnerCollider.bounds.min.y, SpawnerCollider.bounds.max.y),
                z = Random.Range(SpawnerCollider.bounds.min.z, SpawnerCollider.bounds.max.z)
            };

            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));
            Transform targettransform = GameObject.Find("Target/Canvas").transform;

            GameObject TargetClone = Instantiate(Spawn, position, rotation, targettransform);
            TargetClone.gameObject.SetActive(true);
            TargetClone.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
            Destroy(TargetClone, maxLifetime);

            float force = Random.Range(minForce, maxForce);
            TargetClone.GetComponent<Rigidbody2D>().AddForce(TargetClone.transform.up * force, ForceMode2D.Impulse);

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }

    public void onGameEnd(){
        isStart = false;
    }

    public void TargetSizeIncrease(){
        scale += 0.5f;
        particleScale += 0.05f;
    }

    public void TargetSizeDecrease(){
        scale -= 0.5f;
        particleScale -= 0.05f;
    }
}
