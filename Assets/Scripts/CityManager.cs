using UnityEngine;
using System.Collections.Generic;

public class CityManager : MonoBehaviour
{
    public int width = 50;
    public int height = 50;
    public GameObject streetPrefab;
    public GameObject buildingPrefab;
    public GameObject parkPrefab;
    public int numAgents = 5;
    public int steps = 50;

    private TileType[,] cityGrid;
    private List<CityAgent> agents = new List<CityAgent>();

    void Start()
    {
        cityGrid = new TileType[width, height];

        for (int i = 0; i < numAgents; i++)
        {
            GameObject agentObj = new GameObject("Agent" + i);
            CityAgent agent = agentObj.AddComponent<CityAgent>();
            agent.streetPrefab = streetPrefab;
            agent.buildingPrefab = buildingPrefab;
            agent.parkPrefab = parkPrefab;

            Vector2Int startPos = new Vector2Int(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
            agent.Initialize(startPos, cityGrid, steps);
            agents.Add(agent);
        }

        StartCoroutine(RunSimulation());
    }

    IEnumerator<WaitForSeconds> RunSimulation()
    {
        bool active = true;
        while (active)
        {
            active = false;
            foreach (var agent in agents)
            {
                if (agent.steps > 0)
                {
                    agent.Step();
                    active = true;
                }
            }
            yield return new WaitForSeconds(0.05f); // Slow down simulation
        }
    }
}
