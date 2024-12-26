using UnityEngine;

public class ConsoleDebugTest : Singleton<ConsoleDebugTest>
{
    [SerializeField] private GameObject _quantumConsolePrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            _quantumConsolePrefab.SetActive(!_quantumConsolePrefab.activeSelf);
        }
    }
}