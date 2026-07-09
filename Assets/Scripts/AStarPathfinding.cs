using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinding
{
    // Nodo interno per rappresentare le celle durante il calcolo
    private class Node
    {
        public Vector2Int position;
        public Node parent;
        public int gCost; // Costo dal punto di partenza
        public int hCost; // Distanza stimata dal punto di arrivo (Euristica)
        public int FCost => gCost + hCost; // Costo totale

        public Node(Vector2Int position)
        {
            this.position = position;
        }
    }

    // Il metodo principale che restituisce la lista di celle da percorrere
    public static List<Vector2Int> FindPath(HeatmapGrid grid, Vector2Int start, Vector2Int end)
    {
        // Controlli di sicurezza iniziali
        if (!grid.IsValid(start) || !grid.IsValid(end)) return null;
        if (!grid.IsWalkable(end)) return null; // Se la destinazione è un muro, non partiamo

        List<Node> openSet = new List<Node>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        Node startNode = new Node(start);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // Trova il nodo nell'openSet con il costo F più basso
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost ||
                    (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode.position);

            // Abbiamo raggiunto la destinazione! Ricostruiamo il percorso
            if (currentNode.position == end)
            {
                return RetracePath(startNode, currentNode);
            }

            // Controlliamo i vicini nelle 4 direzioni cardinali
            foreach (Vector2Int neighborPos in GetNeighbors(currentNode.position))
            {
                if (!grid.IsValid(neighborPos) || !grid.IsWalkable(neighborPos) || closedSet.Contains(neighborPos))
                {
                    continue; // Salta se fuori mappa, se è un muro o se è già stato elaborato
                }

                // MOVIMENTO CARDINALE: il costo base per spostarsi di 1 cella è 10
                // BONUS HEATMAP: aggiungiamo una penalità basata sul "freddo"? 
                // Oppure il giardiniere preferisce il caldo? Per ora usiamo il costo puro standard (10).
                int movementCostToNeighbor = currentNode.gCost + 10;

                Node neighborNode = openSet.Find(n => n.position == neighborPos);

                if (neighborNode == null)
                {
                    neighborNode = new Node(neighborPos);
                    neighborNode.gCost = movementCostToNeighbor;
                    neighborNode.hCost = GetManhattanDistance(neighborPos, end);
                    neighborNode.parent = currentNode;
                    openSet.Add(neighborNode);
                }
                else if (movementCostToNeighbor < neighborNode.gCost)
                {
                    neighborNode.gCost = movementCostToNeighbor;
                    neighborNode.parent = currentNode;
                }
            }
        }

        return null; // Nessun percorso trovato
    }

    // Ricostruisce il percorso andando a ritroso dal nodo finale a quello iniziale
    private static List<Vector2Int> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Reverse(); // Invertiamo la lista per averla da Partenza a Arrivo
        return path;
    }

    // Calcola la distanza Manhattan (perfetta per le 4 direzioni senza diagonali)
    private static int GetManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y)) * 10;
    }

    // Restituisce i vicini solo nelle 4 direzioni (Su, Giù, Destra, Sinistra)
    private static List<Vector2Int> GetNeighbors(Vector2Int position)
    {
        return new List<Vector2Int>
        {
            new Vector2Int(position.x, position.y + 1), // Su
            new Vector2Int(position.x, position.y - 1), // Giù
            new Vector2Int(position.x - 1, position.y), // Sinistra
            new Vector2Int(position.x + 1, position.y)  // Destra
        };
    }
}