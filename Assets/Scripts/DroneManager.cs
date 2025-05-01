using UnityEngine;
using System.Collections;

public class DroneManager : MonoBehaviour
{
    public GameObject dronePrefab;

    public Transform kronusTarget;
    public Transform lyrionTarget;
    public Transform mystaraTarget;
    public Transform eclipsiaTarget;
    public Transform fioraTarget;

    private const int dronesPerSprite = 100;

    public void DeployDrones(int kronus, int lyrion, int mystara, int eclipsia, int fiora)
    {
        Deploy(kronus, kronusTarget);
        Deploy(lyrion, lyrionTarget);
        Deploy(mystara, mystaraTarget);
        Deploy(eclipsia, eclipsiaTarget);
        Deploy(fiora, fioraTarget);
    }

    void Deploy(int count, Transform target)
    {
        int spritesToCreate = Mathf.CeilToInt(count / (float)dronesPerSprite);

        for (int i = 0; i < spritesToCreate; i++)
        {
            GameObject drone = Instantiate(dronePrefab, GetRandomStart(), Quaternion.identity);
            StartCoroutine(MoveToPlanet(drone, target));
        }
    }

    Vector3 GetRandomStart()
    {
        return new Vector3(Random.Range(-10f, 10f), -6f, 0); // нижній край сцени
    }

    IEnumerator MoveToPlanet(GameObject drone, Transform target)
    {
        float duration = Random.Range(2f, 3f);
        Vector3 start = drone.transform.position;
        Vector3 end = target.position + (Vector3)(Random.insideUnitCircle * 1.5f); // трішки випадково біля планети

        float elapsed = 0f;
        while (elapsed < duration)
        {
            drone.transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        drone.transform.position = end;

        float fadeDuration = 0.5f;
        Vector3 originalScale = drone.transform.localScale;
        elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float t = 1f - (elapsed / fadeDuration);
            drone.transform.localScale = originalScale * t;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(drone);
    }
}
