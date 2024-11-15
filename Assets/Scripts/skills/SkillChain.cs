using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillChain : MonoBehaviour
{
    [SerializeField] GameObject chainLinkPrefab;
    [SerializeField] int initialNumberOfLinks;
    [SerializeField] float linkDistance;
    [SerializeField] float DurationSkill;
    [SerializeField] GameObject PointerSkill;
    [SerializeField] float ForceChain;
    private int currentNumberOfLinks;
    private GameObject[] chainLinks;
    private bool chainActivated = false;
    private float followTimer = 0f;

    void Update()
    {
        // Escucha si se presiona el botón "1" 
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {

            CreateChain();
        }

        if (chainActivated)
        {


           // DestroyChain();
        }

    }

    void CreateChain()
    {

        if (!chainActivated)
        {

            Debug.Log("Ataque cadenas creado");

            currentNumberOfLinks = initialNumberOfLinks;
            chainLinks = new GameObject[currentNumberOfLinks];

            for (int i = 0; i < currentNumberOfLinks; i++)
            {
                chainLinks[i] = Instantiate(chainLinkPrefab, PointerSkill.transform);
                chainLinks[i].transform.localPosition = new Vector3(0, -i * linkDistance, 0);
                if (i > 0)
                {
                    HingeJoint hingeJoint = chainLinks[i].AddComponent<HingeJoint>();
                    hingeJoint.connectedBody = chainLinks[i - 1].GetComponent<Rigidbody>();
                }
            }

            chainActivated = true;
        }
    }

    void DestroyChain()
    {

        followTimer += Time.deltaTime;

        if (followTimer > DurationSkill)
        {
            Debug.Log("Ataque destruido");

            if (chainLinks != null)
            {
                foreach (GameObject link in chainLinks)
                {
                    Destroy(link);
                }
            }
            followTimer = 0;
            chainLinks = null;
            chainActivated = false;

        }
    }
}