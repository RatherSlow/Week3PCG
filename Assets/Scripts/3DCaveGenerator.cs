using UnityEngine;

public class CACaveGenerator3D : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public int depth = 10;

    //how far away will neighbors effect generation, 1 = immidiate neighbors, 2 = neighbors up to 2 away effect it, etc.
    public int neighborRadius = 2;

    [Range(0f, 1f)]
    public float fillProb = 0.25f;

    public int iterations = 5;

    // if neighbor number > birthLimit, set the current cell as a cube
    public int birthLimit = 14;
    // if neighbor number < deathLimit, set the current cell as empty, different weight depending on if at edge or now
    public int edgeDeathLimit = 6;
    public int centerDeathLimit = 4;

    public GameObject cubePrefab; // Assign a cube prefab in the inspector

    private int[,,] grid;

    void Start()
    {
        GenerateCave();
    }

    public void GenerateCave()
    {
        // Step 1: Initialize grid
        grid = new int[width, height, depth];
        InitializeGrid();

        // Step 2: Run cellular automata
        for (int i = 0; i < iterations; i++)
            grid = RunCA(grid);

        // Step 3: Render the cave
        RenderCave();
    }

    void InitializeGrid()
    {
        //for cells starting at position [0, , ] increasing by 1
        for (int x = 0; x < width; x++)
        {
            //for cells starting at position [ ,0, ] increasing by 1
            for (int  y = 0; y < height; y++)
            {
                //for cells starting at position [ , ,0] increasing by 1
                for (int z = 0; z < depth; z++)
                {
                    //if cell is on the border (w, h, or d == 0 or max) place cube (grid = 1)
                    if (x == 0 || x == width - 1 ||  y == 0 || y == height - 1 || z == 0 || z == depth -1)
                    {
                        grid[x, y, z] = 1;
                    }
                    //else
                    else
                    {
                        //generate random float between 0 and 1
                        float rand = Random.Range(0f, 1f);
                        //if generated number is less than fillProb then grid = 1 else grid = 0
                        if (rand < fillProb)
                        {
                            grid[x, y, z] = 1;
                        }
                        else
                        {
                            grid[x, y, z] = 0;
                        }
                    }
                }
            }
        }
    }

    int[,,] RunCA(int[,,] oldGrid)
    {
        int deathLimit;
        int[,,] newGrid = new int[width, height, depth];

        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                for (int z = 1; z < depth - 1; z++)
                {
                    //count number of Neighbors with CountSolidNeighbors()
                    float Neighbors = CountSolidNeighbors(x, y, z, oldGrid);
                    //if cell is near the edge use edgeDeathLimit, else centerDeathLimit. using x<=1 not x=0 due to already skipping the very border above
                    if (x <= 1 || x >= width - 2 || y <= 1 || y >= height - 2 || z <= 1 || z >= depth - 2)
                    {
                        deathLimit = edgeDeathLimit;
                    }
                    else
                    {
                        deathLimit = centerDeathLimit;
                    }
                    //if original cell is a 1(a cube)
                    if (oldGrid[x,y,z] == 1)
                    {
                        //if number of neighbors less than deathLimit
                        if (Neighbors < deathLimit)
                        {
                            //make cell = 0 (empty)
                            newGrid[x,y,z] = 0;
                        }
                        else
                        {
                            //keep as cube
                            newGrid[x,y,z] = 1; 
                        }
                    }
                    //else original cell is a 0(empty)
                    else
                    {
                        //if number of neighbors more than birthlimit
                        if (Neighbors > birthLimit)
                        {
                            //make cell = 1(a cube)
                            newGrid[x,y,z] = 1;
                        }
                        else
                        {
                            newGrid[x,y,z] = 0;
                        }
                    }
                }
            }
        }

        return newGrid;
    }

    float CountSolidNeighbors(int x, int y, int z, int[,,] grid)
    {
        float count = 0;        
        //Loop through neighbors, dx, dy and dz being offsets from current cell
        for (int dx = -neighborRadius; dx <= neighborRadius; dx++)
        {
            for (int dy = -neighborRadius; dy <= neighborRadius; dy++)
            {
                for (int dz = -neighborRadius; dz <= neighborRadius; dz++)
                {
                    //Skip center cell itself
                    if (dx == 0 && dy == 0 && dz == 0)
                    {
                        continue;
                    }
                    //Get actual coordinates of neighbor being checked
                    int nx = x + dx;
                    int ny = y + dy;
                    int nz = z + dz;

                    //Check if coordinates within grid
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height && nz >= 0 && nz < depth)
                    {
                        //if so and if neighbor is a cube increase count by 1
                        if (grid[nx, ny, nz] == 1)
                        {
                            //weighting if neighborRadius is not 1
                            if (neighborRadius != 1)
                            {
                            //Calculate distance from center cell
                            int distance = Mathf.Abs(dx) + Mathf.Abs(dy) + Mathf.Abs(dz);

                            //calulate fall off 1 away = 1, 2 away = 0.5, 3 away = 0.25, etc
                            float weight = 1f / Mathf.Pow(2f, distance - 1);
                            count += weight;
                            }
                            else
                            {
                                count++;
                            }
                        }
                    }
                }
            }
        }
        //return neighbor count
        return count;
    }

    void RenderCave()
    {
        // Clear previous cubes
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        //for each position in the grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++) 
            {
                for(int z = 0; z < depth; z++)
                {
                    //if grid == 1 then spawn a cubeprefab in that position
                    if (grid[x,y,z] == 1)
                    {
                        Vector3 pos = new Vector3(x, y, z);
                        Instantiate(cubePrefab, pos, Quaternion.identity, transform);
                    }
                }
            }
        }
    }
}
