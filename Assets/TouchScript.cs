using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class TouchScript : MonoBehaviour
{
    GameObject ExplosionPrefab;
    ScoreManagerScript scoreManagerScript;
    float particleLifeTime = 3.5f;

    void Start()
    {
        ExplosionPrefab = Resources.Load<GameObject>("Shine_blue");
        scoreManagerScript = FindFirstObjectByType<ScoreManagerScript>();
    }

    void OnMouseDown()
    {
        // May specific na gagawin if hindi 45 ang naclick
        if (gameObject.tag == "Bomb"){
            return;
        }

        Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject Explosion = Instantiate(ExplosionPrefab, clickPosition, Quaternion.identity);
        // Destroy(Explosion, ExplosionLifeTime);
        StartCoroutine(StopAndDestroy(Explosion));
        Destroy(Explosion, particleLifeTime);
        Destroy(gameObject);

        // Add Score
        scoreManagerScript.onScoreAdd();
        
    }

    IEnumerator StopAndDestroy(GameObject explosion)
{
    ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
    if (ps != null)
    {
        ps.Stop(); // Stop emitting new particles

        // Wait for all particles to disappear
        while (ps.IsAlive(true))
        {
            yield return null;
        }
    }

    // yield return new WaitForSeconds(particleLifeTime);
}
}
