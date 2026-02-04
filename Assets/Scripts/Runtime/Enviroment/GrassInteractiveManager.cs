using UnityEngine;
using System.Collections.Generic;

public class GrassInteractiveManager : Singleton<GrassInteractiveManager>
{

    [Header("Shader Settings")]
    [SerializeField] private int maxInteractionPoints = 16;

    private static readonly int PositionsID = Shader.PropertyToID("_InteractionPositions");
    private static readonly int RadiiID = Shader.PropertyToID("_InteractionRadii");
    private static readonly int CountID = Shader.PropertyToID("_InteractionCount");

    private Vector4[] positions;
    private float[] radii;
    private List<GrassInteractor> registeredInteractors = new List<GrassInteractor>();

  

    void Start()
    {
        positions = new Vector4[maxInteractionPoints];
        radii = new float[maxInteractionPoints];
    }

    void Update()
    {
        UpdateShaderProperties();
    }

    public void RegisterInteractor(GrassInteractor interactor)
    {
        if (!registeredInteractors.Contains(interactor) && registeredInteractors.Count < maxInteractionPoints)
        {
            registeredInteractors.Add(interactor);
        }
    }

    public void UnregisterInteractor(GrassInteractor interactor)
    {
        registeredInteractors.Remove(interactor);
    }

    void UpdateShaderProperties()
    {
        int count = Mathf.Min(registeredInteractors.Count, maxInteractionPoints);

        for (int i = 0; i < count; i++)
        {
            if (registeredInteractors[i] != null && registeredInteractors[i].IsActive)
            {
                // Get position with Y offset (+1 from object position)
                Vector3 pos = registeredInteractors[i].transform.position;
                //pos.y += 1f; // Offset for ground check

                positions[i] = new Vector4(pos.x, pos.y, pos.z, 0);
                radii[i] = registeredInteractors[i].InteractionRadius;
            }
            else
            {
                // Clear invalid entries
                positions[i] = new Vector4(0, -1000, 0, 0);
                radii[i] = 0;
            }
        }

        // Clear unused slots
        for (int i = count; i < maxInteractionPoints; i++)
        {
            positions[i] = new Vector4(0, -1000, 0, 0);
            radii[i] = 0;
        }

        Shader.SetGlobalVectorArray(PositionsID, positions);
        Shader.SetGlobalFloatArray(RadiiID, radii);
        Shader.SetGlobalInt(CountID, count);
    }
}