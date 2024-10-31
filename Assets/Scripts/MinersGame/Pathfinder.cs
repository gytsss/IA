using System;
using System.Collections.Generic;
using System.Linq;

//NodeType es la info que necesitas del nodo, si es mina, etc, tipo de nodo
//CoordinateType es el tipo de coordenada, float, int, Vector2, Vector2Int etc. identificar el tipo del nodo para hacer chequeos
//TCoordinate es necesario para hacer cuentas de distancias, conseguir x,y, etc.
public abstract class Pathfinder<NodeType, CoordinateType, TCoordinate>
    where NodeType : INode<CoordinateType>, INode, new()
    where CoordinateType : IEquatable<CoordinateType>
    where TCoordinate : ICoordinate<CoordinateType>, new()
{
    public Graph<NodeType, CoordinateType> graph;
    protected List<NodeType> goldMines;

    //public delegate int TransitionCostDelegate(Node<Vector2> fromNode, Node<Vector2> toNode);

    public List<NodeType> FindPath(NodeType startNode, NodeType destinationNode, float distanceBetweenNodes)
    {
        Dictionary<NodeType, (NodeType Parent, int AcumulativeCost, int Heuristic)> nodes =
            new Dictionary<NodeType, (NodeType Parent, int AcumulativeCost, int Heuristic)>();

        foreach (NodeType node in graph.nodes)
        {
            nodes.Add(node, (default, 0, 0));
        }

        List<NodeType> openList = new List<NodeType>();
        List<NodeType> closedList = new List<NodeType>();

        openList.Add(startNode);

        foreach (var node in nodes.Keys.ToList())
        {
            if (NodesEquals(startNode, node)) continue;

            var nodeData = nodes[node];
            nodes.Remove(node);
            nodes[startNode] = nodeData;
            break;
        }

        while (openList.Count > 0)
        {
            NodeType currentNode = openList[0];
            int currentIndex = 0;

            for (int i = 1; i < openList.Count; i++)
            {
                if (nodes[openList[i]].AcumulativeCost + nodes[openList[i]].Heuristic >=
                    nodes[currentNode].AcumulativeCost + nodes[currentNode].Heuristic) continue;

                currentNode = openList[i];
                currentIndex = i;
            }

            openList.RemoveAt(currentIndex);
            closedList.Add(currentNode);
            
            
            if (NodesEquals(currentNode, destinationNode))
            {
                return GeneratePath(startNode, destinationNode);
            }

            foreach (NodeType neighbor in GetNeighbors(currentNode, distanceBetweenNodes))
            {
                if (IsBlocked(neighbor))
                    continue;
                if (closedList.Contains(neighbor))
                    continue;
                if (!nodes.ContainsKey(neighbor))
                    continue;

                
                int tentativeNewAcumulatedCost = 0;

                tentativeNewAcumulatedCost += nodes[currentNode].AcumulativeCost;
                tentativeNewAcumulatedCost += MoveToNeighborCost(currentNode, neighbor, distanceBetweenNodes);

                if (!openList.Contains(neighbor) || tentativeNewAcumulatedCost < nodes[currentNode].AcumulativeCost)
                {
                    TCoordinate neighborCoordinate = new TCoordinate();
                    neighborCoordinate.SetCoordinate(neighborCoordinate.GetCoordinate());

                    TCoordinate destinationNodeCoordinate = new TCoordinate();
                    destinationNodeCoordinate.SetCoordinate(destinationNodeCoordinate.GetCoordinate());

                    nodes[neighbor] = (currentNode, tentativeNewAcumulatedCost,
                        Distance(neighborCoordinate, destinationNodeCoordinate));

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null;

        List<NodeType> GeneratePath(NodeType startNode, NodeType goalNode)
        {
            List<NodeType> path = new List<NodeType>();
            NodeType currentNode = goalNode;

            while (!NodesEquals(currentNode, startNode))
            {
                path.Add(currentNode);

                foreach (var node in nodes.Keys.ToList().Where(node => NodesEquals(currentNode, node)))
                {
                    currentNode = nodes[node].Parent;
                    break;
                }
            }

            path.Reverse();
            return path;
        }
    }


    protected abstract int Distance(TCoordinate A, TCoordinate B);

    protected abstract bool NodesEquals(NodeType A, NodeType B);


    protected abstract bool IsBlocked(NodeType node);
    protected abstract ICollection<INode<CoordinateType>> GetNeighbors(NodeType node, float distance);
    protected abstract int MoveToNeighborCost(NodeType A, NodeType B, float distanceBetweenNodes);
}