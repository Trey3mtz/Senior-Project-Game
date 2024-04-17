using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Cyrcadian
{
    public class Test_WFC_Gen : MonoBehaviour
    {
        public Tilemap tilemap;

        public TileBase oceanTile;
        public TileBase beachTile;
        public TileBase grasslandTile;
        public TileBase forestTile;

        public int worldWidth = 100;
        public int worldHeight = 100;


        private void Start()
        {
            GenerateWorld();
        }


        void GenerateWorld()
        {
            int[,] worldMap = new int[worldWidth, worldHeight];

            // Define the center of the island
            int centerX = worldWidth / 2;
            int centerY = worldHeight / 2;
            int islandRadius = Mathf.Min(worldWidth, worldHeight) / 4; // Adjust the island size as needed

            // Populate the world map with different tile types based on island shape
            for (int x = 0; x < worldWidth; x++)
            {
                for (int y = 0; y < worldHeight; y++)
                {
                    float distanceToCenter = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));

                    if (distanceToCenter < islandRadius)
                    {
                        // Inside the island boundary
                        if (distanceToCenter < islandRadius * 0.45f)
                        {
                            worldMap[x, y] = 3; // Forest
                        }
                        else if (distanceToCenter < islandRadius * 0.7f)
                        {
                            worldMap[x, y] = 2; // Grassland
                        }
                        else if (distanceToCenter < islandRadius * 0.95f)
                        {
                            worldMap[x, y] = 1; // Beach
                        }
                        else
                        {
                            worldMap[x, y] = 0; // Ocean
                        }
                    }
                    else
                    {
                        // Outside the island boundary
                        worldMap[x, y] = 0; // Ocean
                    }
                }
            }

            // Apply additional rules for tile connectivity around the forest tile
            for (int x = 0; x < worldWidth; x++)
            {
                for (int y = 0; y < worldHeight; y++)
                {
                    if (worldMap[x, y] == 3) // Forest tile at the center
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                int neighborX = x + dx;
                                int neighborY = y + dy;

                                if (neighborX >= 0 && neighborX < worldWidth && neighborY >= 0 && neighborY < worldHeight)
                                {
                                    int neighborTile = worldMap[neighborX, neighborY];

                                    // Apply rules for tile connectivity around the forest tile
                                    if (neighborTile != 3) // Ensure only forest and grassland tiles touch the forest
                                    {
                                        if (neighborTile == 0) // Ocean
                                        {
                                            worldMap[neighborX, neighborY] = 0; // Set to Ocean
                                        }
                                        else if (neighborTile == 1) // Beach
                                        {
                                            worldMap[neighborX, neighborY] = 1; // Set to Beach
                                        }
                                        else if (neighborTile == 2) // Grassland
                                        {
                                            worldMap[neighborX, neighborY] = 2; // Set to Grassland
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Set tiles based on the generated world map
            for (int x = 0; x < worldWidth; x++)
            {
                for (int y = 0; y < worldHeight; y++)
                {
                    switch (worldMap[x, y])
                    {
                        case 0: // Ocean
                            tilemap.SetTile(new Vector3Int(x, y, 0), oceanTile);
                            break;
                        case 1: // Beach
                            tilemap.SetTile(new Vector3Int(x, y, 0), beachTile);
                            break;
                        case 2: // Grassland
                            tilemap.SetTile(new Vector3Int(x, y, 0), grasslandTile);
                            break;
                        case 3: // Forest
                            tilemap.SetTile(new Vector3Int(x, y, 0), forestTile);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
           
    }
}
