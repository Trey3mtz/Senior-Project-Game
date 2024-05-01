using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class WaveFunction : MonoBehaviour
{
    public int dimensions;
    public Tile[] tileObjects;
    public List<Cell> gridComponents;
    public Cell cellObj;

    int iterations = 0;

    void Awake()
    {
        gridComponents = new List<Cell>();
        InitializeGrid();
    }

    void InitializeGrid()
    {
        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                Cell newCell = Instantiate(cellObj, new Vector2(x, y), Quaternion.identity);
                newCell.CreateCell(false, tileObjects);
                gridComponents.Add(newCell);
            }
        }

        StartCoroutine(CheckEntropy());
    }


    IEnumerator CheckEntropy()
    {
        List<Cell> tempGrid = new List<Cell>(gridComponents);

        tempGrid.RemoveAll(c => c.collapsed);

        tempGrid.Sort((a, b) => { return a.tileOptions.Length - b.tileOptions.Length; });

        int arrLength = tempGrid[0].tileOptions.Length;
        int stopIndex = default;

        for (int i = 1; i < tempGrid.Count; i++)
        {
            if (tempGrid[i].tileOptions.Length > arrLength)
            {
                stopIndex = i;
                break;
            }
        }

        if (stopIndex > 0)
        {
            tempGrid.RemoveRange(stopIndex, tempGrid.Count - stopIndex);
        }

        yield return new WaitForSeconds(0.01f);

        CollapseCell(tempGrid);
    }

    void CollapseCell(List<Cell> tempGrid)
{
    if (tempGrid.Count == 0)
    {
        // Handle the case where tempGrid is empty
        Debug.LogError("tempGrid is empty, cannot collapse cell.");
        return;
    }

    int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);
    Cell cellToCollapse = tempGrid[randIndex];

    cellToCollapse.collapsed = true;

    if (cellToCollapse.tileOptions.Length == 0)
    {
        // Handle the case where there are no tile options available
        Debug.LogError("No tile options available for the cell.");
        return;
    }

    Tile selectedTile = cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length)];
    cellToCollapse.tileOptions = new Tile[] { selectedTile };

    Tile foundTile = cellToCollapse.tileOptions[0];
    Instantiate(foundTile, cellToCollapse.transform.position, Quaternion.identity);

    UpdateGeneration();
}


    void UpdateGeneration()
    {
        List<Cell> newGenerationCell = new List<Cell>(gridComponents);

        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                var index = x + y * dimensions;
                if (gridComponents[index].collapsed)
                {
                    Debug.Log("called");
                    newGenerationCell[index] = gridComponents[index];
                }
                else
                {
                    List<Tile> options = new List<Tile>();
                    foreach (Tile t in tileObjects)
                    {
                        options.Add(t);
                    }

                    //update above
                    if (y > 0)
                    {
                        Cell up = gridComponents[x + (y - 1) * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in up.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].upNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //update right
                    if (x < dimensions - 1)
                    {
                        Cell right = gridComponents[x + 1 + y * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in right.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].leftNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //look down
                    if (y < dimensions - 1)
                    {
                        Cell down = gridComponents[x + (y + 1) * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in down.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].downNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //look left
                    if (x > 0)
                    {
                        Cell left = gridComponents[x - 1 + y * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in left.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].rightNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    Tile[] newTileList = new Tile[options.Count];

                    for (int i = 0; i < options.Count; i++)
                    {
                        newTileList[i] = options[i];
                    }

                    newGenerationCell[index].RecreateCell(newTileList);
                }
            }
        }

        gridComponents = newGenerationCell;
        iterations++;

        if(iterations < dimensions * dimensions)
        {
            StartCoroutine(CheckEntropy());
        }

    }

    void CheckValidity(List<Tile> optionList, List<Tile> validOption)
    {
        for (int x = optionList.Count - 1; x >= 0; x--)
        {
            var element = optionList[x];
            if (!validOption.Contains(element))
            {
                optionList.RemoveAt(x);
            }
        }
    }
}








































// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using Unity.Mathematics;
// using UnityEngine;

// public class WaveFunction : MonoBehaviour
// {
//     public int dimensions;
//     public Tile[] tileObjects;
//     public List<Cell> gridComponents;
//     public Cell cellObj;

//     int iterations = 0;

//     void Awake()
//     {
//         gridComponents = new List<Cell>();
//         InitializeGrid();
//     }
//     void InitializeGrid()
//     {
//         for (int y = 0; y < dimensions; y++)
//         {
//             for (int x = 0; x < dimensions; x++)
//             {
//                 Cell newCell = Instantiate(cellObj, new Vector2(x, y), Quaternion.identity);
//                 newCell.CreateCell(false, tileObjects);
//                 gridComponents.Add(newCell);
//             }
//         }

//         StartCoroutine(CheckEntropy());
//     }
//    // void InitializeGrid()
//   //  {
//      //   for (int y = 0; y < dimensions; y++)
//      //   {
//       //      for (int x = 0; x < dimensions; x++)
//       //      {
//     //            Cell newCell = Instantiate(cellObj, new Vector2(x, y), Quaternion.identity);
//      //           newCell.CreateCell(false, tileObjects);
//     //            gridComponents.Add(newCell);
//     //        }
//      //   }

//      //   StartCoroutine(CheckEntropy());
//     //}


//     IEnumerator CheckEntropy()
//     {
//         List<Cell> tempGrid = new List<Cell>(gridComponents);

//         tempGrid.RemoveAll(c => c.collapsed);

//         tempGrid.Sort((a, b) => { return a.tileOptions.Length - b.tileOptions.Length; });

//         int arrLength = tempGrid[0].tileOptions.Length;
//         int stopIndex = default;

//         for (int i = 1; i < tempGrid.Count; i++)
//         {
//             if (tempGrid[i].tileOptions.Length > arrLength)
//             {
//                 stopIndex = i;
//                 break;
//             }
//         }

//         if (stopIndex > 0)
//         {
//             tempGrid.RemoveRange(stopIndex, tempGrid.Count - stopIndex);
//         }

//         yield return new WaitForSeconds(0.01f);

//         CollapseCell(tempGrid);
//     }

//     void CollapseCell(List<Cell> tempGrid)
// {
//     if (tempGrid.Count == 0)
//     {
//         // Handle the case where tempGrid is empty
//         Debug.LogError("tempGrid is empty, cannot collapse cell.");
//         return;
//     }

//     int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);
//     Cell cellToCollapse = tempGrid[randIndex];

//     cellToCollapse.collapsed = true;

//     if (cellToCollapse.tileOptions.Length == 0)
//     {
//         // Handle the case where there are no tile options available
//         Debug.LogError("No tile options available for the cell.");
//         return;
//     }

//     Tile selectedTile = cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length)];
//     cellToCollapse.tileOptions = new Tile[] { selectedTile };

//     Tile foundTile = cellToCollapse.tileOptions[0];
//     Instantiate(foundTile, cellToCollapse.transform.position, Quaternion.identity);

//     UpdateGeneration();
// }

// void UpdateGeneration()
// {
//     List<Cell> newGenerationCell = new List<Cell>();

//     for (int y = 0; y < dimensions; y++)
//     {
//         for (int x = 0; x < dimensions; x++)
//         {
//             int index = x + y * dimensions;
//             Cell currentCell = gridComponents[index];

//             if (currentCell.collapsed)
//             {
//                 newGenerationCell.Add(currentCell);
//                 continue; // Move to the next cell if already collapsed
//             }

//             List<Tile> options = new List<Tile>(currentCell.tileOptions);

//             // Update above
//             if (y > 0)
//             {
//                 Cell up = gridComponents[x + (y - 1) * dimensions];
//                 options = CheckValidity(options, up.tileOptions);
//             }

//             // Update right
//             if (x < dimensions - 1)
//             {
//                 int rightIndex = x + 1 + y * dimensions;
//                 Debug.Log("Right Index: " + rightIndex);
//                 Cell right = gridComponents[rightIndex];
//                 options = CheckValidity(options, right.tileOptions);
//             }

//             // Update below
//             if (y < dimensions - 1)
//             {
//                 Cell down = gridComponents[x + (y + 1) * dimensions];
//                 options = CheckValidity(options, down.tileOptions);
//             }

//             // Update left
//             if (x > 0)
//             {
//                 int leftIndex = x - 1 + y * dimensions;
//                 Debug.Log("Left Index: " + leftIndex);
//                 Cell left = gridComponents[leftIndex];
//                 options = CheckValidity(options, left.tileOptions);
//             }

//             // Select a random tile from valid options
//             if (options.Count > 0)
//             {
//                 Tile selectedTile = options[UnityEngine.Random.Range(0, options.Count)];
//                 currentCell.tileOptions = new Tile[] { selectedTile };
//             }
//             else
//             {
//                 Debug.LogError("No valid tile options available for cell.");
//                 // Handle this case as needed (e.g., choose a default tile)
//             }

//             currentCell.collapsed = true;
//             newGenerationCell.Add(currentCell);
//         }
//     }

//     gridComponents = newGenerationCell;
//     iterations++;

//     if (iterations < dimensions * dimensions)
//     {
//         StartCoroutine(CheckEntropy());
//     }
// }

// List<Tile> CheckValidity(List<Tile> options, Tile[] neighborTiles)
// {
//     List<Tile> validOptions = new List<Tile>();

//     foreach (Tile possibleTile in options)
//     {
//         if (neighborTiles.Contains(possibleTile))
//         {
//             validOptions.Add(possibleTile);
//         }
//     }

//     return validOptions;
// }

//  //   void UpdateGeneration()
//  //   {
// //        List<Cell> newGenerationCell = new List<Cell>(gridComponents);

//   //      for (int y = 0; y < dimensions; y++)
//  //       {
//    //         for (int x = 0; x < dimensions; x++)
//     //        {
//    //             var index = x + y * dimensions;
//     //            if (gridComponents[index].collapsed)
//     //            {
//         //             Debug.Log("called");
//         //             newGenerationCell[index] = gridComponents[index];
//         //         }
//         //         else
//         //         {
//         //             List<Tile> options = new List<Tile>();
//         //             foreach (Tile t in tileObjects)
//         //             {
//         //                 options.Add(t);
//         //             }

//         //             //update above
//         //             if (y > 0)
//         //             {
//         //                 Cell up = gridComponents[x + (y - 1) * dimensions];
//         //                 List<Tile> validOptions = new List<Tile>();

//         //                 foreach (Tile possibleOptions in up.tileOptions)
//         //                 {
//         //                     var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
//         //                     var valid = tileObjects[valOption].upNeighbours;

//         //                     validOptions = validOptions.Concat(valid).ToList();
//         //                 }

//         //                 CheckValidity(options, validOptions);
//         //             }

//         //             //update right
//         //             if (x < dimensions - 1)
//         //             {
//         //                 Cell right = gridComponents[x + 1 + y * dimensions];
//         //                 List<Tile> validOptions = new List<Tile>();

//         //                 foreach (Tile possibleOptions in right.tileOptions)
//         //                 {
//         //                     var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
//         //                     var valid = tileObjects[valOption].leftNeighbours;

//         //                     validOptions = validOptions.Concat(valid).ToList();
//         //                 }

//         //                 CheckValidity(options, validOptions);
//         //             }

//         //             //look down
//         //             if (y < dimensions - 1)
//         //             {
//         //                 Cell down = gridComponents[x + (y + 1) * dimensions];
//         //                 List<Tile> validOptions = new List<Tile>();

//         //                 foreach (Tile possibleOptions in down.tileOptions)
//         //                 {
//         //                     var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
//         //                     var valid = tileObjects[valOption].downNeighbours;

//         //                     validOptions = validOptions.Concat(valid).ToList();
//         //                 }

//         //                 CheckValidity(options, validOptions);
//         //             }

//         //             //look left
//         //             if (x > 0)
//         //             {
//         //                 Cell left = gridComponents[x - 1 + y * dimensions];
//         //                 List<Tile> validOptions = new List<Tile>();

//         //                 foreach (Tile possibleOptions in left.tileOptions)
//         //                 {
//         //                     var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
//         //                     var valid = tileObjects[valOption].rightNeighbours;

//         //                     validOptions = validOptions.Concat(valid).ToList();
//         //                 }

//         //                 CheckValidity(options, validOptions);
//         //             }

//         //             Tile[] newTileList = new Tile[options.Count];

//         //             for (int i = 0; i < options.Count; i++)
//         //             {
//         //                 newTileList[i] = options[i];
//         //             }

//         //             newGenerationCell[index].RecreateCell(newTileList);
//         //         }
//         //     }
//         // }

//     //     gridComponents = newGenerationCell;
//     //     iterations++;

//     //     if(iterations < dimensions * dimensions)
//     //     {
//     //         StartCoroutine(CheckEntropy());
//     //     }

//     // }

//     // void CheckValidity(List<Tile> optionList, List<Tile> validOption)
//     // {
//     //     for (int x = optionList.Count - 1; x >= 0; x--)
//     //     {
//     //         var element = optionList[x];
//     //         if (!validOption.Contains(element))
//     //         {
//     //             optionList.RemoveAt(x);
//     //         }
//     //     }
//     // }
// }