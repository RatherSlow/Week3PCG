using UnityEngine;

public class CityAgent : MonoBehaviour
{
    public Vector2Int position; // Grid position
    public int steps;           // How many steps this agent can take
    public GameObject streetPrefab;
    public GameObject buildingPrefab;
    public GameObject parkPrefab;
    public TileType[,] cityGrid;

    public void Initialize(Vector2Int startPos, TileType[,] grid, int num_steps)
    {
        position = startPos;
        cityGrid = grid;
        steps = num_steps; // Example: agent will move 50 steps
    }

    public void Step()
    {
        if (steps <= 0) return;
        //Possible directions 
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,    //(0,1)
            Vector2Int.down,  //(0,-1)
            Vector2Int.left,  //(-1,0)
            Vector2Int.right, //(1,0)
        };
        //generate random number between 0 and direction length (number of possible directions) so 0, 1, 2 or 3 and pick the direction from the directions array
        Vector2Int randomDirection = directions[Random.Range(0, directions.Length)];

        //change position by random direction
        position += randomDirection;

        //Clamp position to stay inside grid, as grid values would be between 0 and grid lenght -1. GetLength 0 is one dimention, GetLenght 1 is the other.
        position.x = Mathf.Clamp(position.x, 0, cityGrid.GetLength(0) - 1);
        position.y = Mathf.Clamp(position.y, 0, cityGrid.GetLength(1) - 1);

        //generate random float between 0 and 1
        float rand = Random.Range(0f, 1f);

        //The tile that is to be placed
        TileType tileToPlace;

        //depending on the random float generated assign the appropriate tile
        if (rand < 0.6f)
        {
            tileToPlace = TileType.Street;
        }
        else if (rand < 0.9f)
        {
            tileToPlace = TileType.Building;
        }
        else
        {
            tileToPlace= TileType.Park;
        }

        //Place the assigned tile
        PlaceTile(tileToPlace);

        //reduce steps by 1
        steps--;
    }

    void PlaceTile(TileType type)
    {
        if (cityGrid[position.x, position.y] != TileType.Empty) return;

        cityGrid[position.x, position.y] = type;
        GameObject prefab = null;
        //assign the appropriate prefab
        switch (type)
        {
            case TileType.Street:
                prefab = streetPrefab;
                break;

            case TileType.Building:
                prefab = buildingPrefab;
                break;

            case TileType.Park:
                prefab = parkPrefab;
                break;
        }
        //when there is an appropriate prefab assigned instantiate the prefab
        if (prefab != null) 
        {
            Vector3 pos = new Vector3(position.x, 0, position.y);
            GameObject instance = Instantiate(prefab, pos, Quaternion.identity);

            //If the prefab is a building, randomise the height and colour
            if (prefab == buildingPrefab)
            {
                Vector3 scale = instance.transform.localScale;
                scale.y = Random.Range(1f, 3f); //height random between 1f and 3f
                instance.transform.localScale = scale;

                //Assign colour from array
                Color[] buildingColors = { Color.red, Color.blue};
                Renderer rend = instance.GetComponentInChildren<Renderer>();
                if (rend != null)
                {
                    rend.material.color = buildingColors[Random.Range(0, buildingColors.Length)];
                }

            }
        }
    }
}
