using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PenaltyManager : MonoBehaviour
{
    public int width = 100;
    public int height = 80;
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public Transform mazeParent;

    private int[,] maze;
    private Vector2Int startPos;
    private Vector2Int ballPos;
    private Vector2Int goalPos;

    public Vector3 mazeOffset = new Vector3(10, 0, 5);
    [SerializeField]
    private GameObject ballPrefab;

    public NavMeshSurface navMeshSurface;

    private void OnEnable()
    {
        GenerateMaze();
        SpawnMaze();
    }
    void GenerateMaze()
    {
        maze = new int[width, height];
        maze[0, 0] = 1; 

        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        startPos = new Vector2Int(width / 2, height - 1); 
        ballPos = new Vector2Int(Random.Range(1, width - 2), Random.Range(1, height - 2));
        goalPos = new Vector2Int(width / 2, 0); 

        maze[startPos.x, startPos.y] = 1; 
        stack.Push(startPos);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                Vector2Int between = (current + next) / 2;

                maze[next.x, next.y] = 1;
                maze[between.x, between.y] = 1;

                stack.Push(next);
            }
            else
            {
                stack.Pop();
            }
        }

        maze[ballPos.x, ballPos.y] = 1;
        maze[goalPos.x, goalPos.y] = 1;
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] directions = {
            new Vector2Int(2, 0), new Vector2Int(-2, 0),
            new Vector2Int(0, 2), new Vector2Int(0, -2)
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = pos + dir;
            if (neighbor.x > 0 && neighbor.x < width - 1 && neighbor.y > 0 && neighbor.y < height - 1 && maze[neighbor.x, neighbor.y] == 0)
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    void SpawnMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x, 0, y) + mazeOffset;
                if (maze[x, y] == 0)
                {
                    Instantiate(wallPrefab, pos, Quaternion.identity, mazeParent);
                }
         
            }
        }

        GameObject ball = Instantiate(ballPrefab);

        ball.transform.position = new Vector3(ballPos.x, 0.5f, ballPos.y) + mazeOffset;
        ball.tag = "Ball";
        navMeshSurface.BuildNavMesh();

    }

    public int[,] GetMazeGrid()
    {
        return maze;
    }

}
