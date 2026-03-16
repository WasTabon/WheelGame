using UnityEngine;
using System.Collections.Generic;

public class FXManager : MonoBehaviour
{
    public static FXManager Instance { get; private set; }

    private ParticleSystem correctFXPrefab;
    private ParticleSystem wrongFXPrefab;
    private ParticleSystem collectFXPrefab;
    private ParticleSystem starFXPrefab;

    private Material particleMaterial;
    private List<ParticleSystem> activeParticles = new List<ParticleSystem>();

    private void Awake()
    {
        Instance = this;
        CreateMaterial();
        CreatePrefabs();
    }

    private void CreateMaterial()
    {
        particleMaterial = new Material(Shader.Find("Sprites/Default"));
        particleMaterial.SetFloat("_Mode", 0);
    }

    private void CreatePrefabs()
    {
        correctFXPrefab = CreateParticleSystem("FX_Correct",
            new Color(0.2f, 1f, 0.4f, 1f),
            new Color(0.1f, 0.8f, 0.3f, 0f),
            20, 0.5f, 0.8f, 0.3f, 1.5f, true);

        wrongFXPrefab = CreateParticleSystem("FX_Wrong",
            new Color(1f, 0.25f, 0.2f, 1f),
            new Color(0.9f, 0.1f, 0.1f, 0f),
            15, 0.4f, 0.6f, 0.2f, 1.0f, false);

        collectFXPrefab = CreateParticleSystem("FX_Collect",
            new Color(1f, 0.85f, 0.1f, 1f),
            new Color(1f, 0.6f, 0f, 0f),
            35, 0.7f, 1.2f, 0.15f, 2.5f, false);

        starFXPrefab = CreateParticleSystem("FX_Star",
            new Color(1f, 0.9f, 0.3f, 1f),
            new Color(1f, 1f, 0.5f, 0f),
            25, 0.6f, 1.0f, 0.1f, 2.0f, false);
    }

    private ParticleSystem CreateParticleSystem(string name, Color startColor, Color endColor,
        int count, float lifetime, float speed, float size, float radius, bool converge)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(transform);
        obj.SetActive(false);

        ParticleSystem ps = obj.AddComponent<ParticleSystem>();
        ParticleSystemRenderer psr = obj.GetComponent<ParticleSystemRenderer>();

        var main = ps.main;
        main.playOnAwake = false;
        main.duration = 0.1f;
        main.loop = false;
        main.startLifetime = lifetime;
        main.startSpeed = speed;
        main.startSize = size;
        main.maxParticles = count + 10;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 0.3f;
        main.startColor = startColor;

        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, (short)count)
        });

        var shape = ps.shape;
        shape.enabled = true;
        if (converge)
        {
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = radius;
        }
        else
        {
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.1f;
        }

        var colorOverLife = ps.colorOverLifetime;
        colorOverLife.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(startColor, 0f),
                new GradientColorKey(endColor, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.8f, 0.3f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLife.color = grad;

        var sizeOverLife = ps.sizeOverLifetime;
        sizeOverLife.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 1f);
        sizeCurve.AddKey(0.5f, 0.7f);
        sizeCurve.AddKey(1f, 0f);
        sizeOverLife.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        psr.renderMode = ParticleSystemRenderMode.Billboard;
        psr.material = particleMaterial;
        psr.sortingOrder = 50;

        return ps;
    }

    public void PlayCorrect(Vector3 position)
    {
        SpawnFX(correctFXPrefab, position);
    }

    public void PlayWrong(Vector3 position)
    {
        SpawnFX(wrongFXPrefab, position);
    }

    public void PlayCollect(Vector3 position)
    {
        SpawnFX(collectFXPrefab, position);
    }

    public void PlayStar(Vector3 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
        SpawnFX(starFXPrefab, worldPos);
    }

    private void SpawnFX(ParticleSystem prefab, Vector3 position)
    {
        if (prefab == null) return;

        prefab.gameObject.SetActive(true);
        prefab.transform.position = position;
        prefab.Clear();
        prefab.Play();

        float duration = prefab.main.startLifetime.constant + prefab.main.duration + 0.1f;
        StartCoroutine(DeactivateAfter(prefab.gameObject, duration));
    }

    private System.Collections.IEnumerator DeactivateAfter(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
            obj.SetActive(false);
    }
}
