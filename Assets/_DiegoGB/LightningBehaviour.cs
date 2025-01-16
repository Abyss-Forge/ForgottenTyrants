using System.Collections;
using System.Collections.Generic;
using ForgottenTyrants;
using Unity.Netcode;
using UnityEngine;

public class LightningBehaviour : NetworkBehaviour
{
    [SerializeField] private float _fadeDuration = 2f;
    [SerializeField] private float _groundDuration = 1f;
    [SerializeField] private float _speed = 30f;
    [SerializeField] private Transform _lightingModelStart;
    [SerializeField] private Transform _lightingModelEnd;
    [SerializeField] private int _keyPointsQuantity = 10;
    [SerializeField] private float _spread = 1f;
    [SerializeField] private float _frequency = 0.1f;

    private Terrain _terrain;
    private bool _shouldMove = true;
    private LineRenderer _line;
    private Material _lineMaterial;
    private float _time;
    private Vector3 _targetPosition;

    public override void OnNetworkSpawn()
    {
        _terrain = Terrain.activeTerrain;
        _targetPosition = ShowRandomPointOnTerrain();
        transform.LookAt(_targetPosition);
        // Vector3 direction = (_targetPosition - _lightingModelStart.position).normalized;
        // _lightingModelStart.rotation = Quaternion.LookRotation(direction);

        // float distanceBetweenStartAndEnd = Vector3.Distance(_lightingModelStart.position, _lightingModelEnd.position);
        // _lightingModelEnd.position = _lightingModelStart.position + direction * distanceBetweenStartAndEnd;

        _line = GetComponent<LineRenderer>();
        if (_line != null)
        {
            _lineMaterial = _line.material;
        }
    }

    private Vector3 GetRandomPointOnTerrain()
    {
        TerrainData terrainData = _terrain.terrainData;
        Vector3 terrainPosition = _terrain.transform.position;

        float x = Random.Range(0, terrainData.size.x);
        float z = Random.Range(0, terrainData.size.z);
        float y = terrainData.GetHeight(
            Mathf.FloorToInt(x / terrainData.size.x * terrainData.heightmapResolution),
            Mathf.FloorToInt(z / terrainData.size.z * terrainData.heightmapResolution)
        );

        return new Vector3(x + terrainPosition.x, y + terrainPosition.y, z + terrainPosition.z);
    }

    private Vector3 ShowRandomPointOnTerrain()
    {
        // Obtiene un punto aleatorio en el terreno
        Vector3 randomPoint = GetRandomPointOnTerrain();

        // Crea un marcador visual en la escena, como una esfera o un cubo
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.transform.position = randomPoint;

        // Ajusta el tamaño del marcador para que sea más visible
        marker.transform.localScale = Vector3.one * 0.5f;

        // Opcional: Cambiar el color del marcador
        Renderer markerRenderer = marker.GetComponent<Renderer>();
        markerRenderer.material.color = Color.red;

        // Opcional: Agregar una etiqueta para identificarlo en la jerarquía
        marker.name = "Random Point Marker";

        return randomPoint;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        if (_time > _frequency)
        {
            UpdatePoints();
            _time = 0;
        }

        if (_shouldMove)
        {
            Vector3 direction = (_targetPosition - transform.position).normalized;
            transform.position += direction * _speed * Time.deltaTime;
        }
    }

    private void UpdatePoints()
    {
        if (_line == null) return;

        List<Vector3> points = InterpolatePoints(_lightingModelStart.localPosition, _lightingModelEnd.localPosition, _keyPointsQuantity);
        _line.positionCount = points.Count;
        _line.SetPositions(points.ToArray());
    }

    private List<Vector3> InterpolatePoints(Vector3 start, Vector3 end, int totalPoints)
    {
        var points = new List<Vector3>();
        for (int i = 0; i < totalPoints; i++)
        {
            points.Add(Vector3.Lerp(start, end, (float)i / totalPoints) + RandomizeGap());
        }
        return points;
    }

    private Vector3 RandomizeGap()
    {
        return Random.insideUnitSphere.normalized * Random.Range(0, _spread);
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(_groundDuration);

        float elapsedTime = 0f;
        AnimationCurve originalCurve = _line.widthCurve;
        AnimationCurve fadingCurve = new AnimationCurve();

        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float fadeProgress = Mathf.Clamp01(elapsedTime / _fadeDuration);

            SetTransparency(Mathf.Lerp(1.0f, 0.0f, fadeProgress));
            SetEmissionIntensity(Mathf.Lerp(1.0f, 0.0f, fadeProgress));

            fadingCurve.keys = originalCurve.keys;
            for (int i = 0; i < fadingCurve.keys.Length; i++)
            {
                Keyframe key = fadingCurve.keys[i];
                key.value *= (1 - fadeProgress);
                fadingCurve.MoveKey(i, key);
            }
            _line.widthCurve = fadingCurve;

            yield return null;
        }

        SetTransparency(0.0f);
        SetEmissionIntensity(0.0f);
        _line.widthCurve = new AnimationCurve();

        Destroy(gameObject);
    }

    private void SetTransparency(float alpha)
    {
        if (_lineMaterial == null) return;

        Color baseColor = _lineMaterial.GetColor("_BaseColor");
        baseColor.a = alpha;
        _lineMaterial.SetColor("_BaseColor", baseColor);
    }

    private void SetEmissionIntensity(float intensity)
    {
        if (_lineMaterial == null) return;

        Color emissionColor = _lineMaterial.GetColor("_EmissionColor");
        _lineMaterial.SetColor("_EmissionColor", emissionColor * intensity);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == Layer.Terrain)
        {
            _shouldMove = false;
            StartCoroutine(FadeOut());
        }
    }


}
