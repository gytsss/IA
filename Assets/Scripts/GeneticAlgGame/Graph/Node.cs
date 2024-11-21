// using System;
// using System.Collections.Generic;
// using Pathfinder;
//
// namespace GeneticAlgGame.Graph
// {
//     public enum NodeTypes
//     {
//         Default,
//         UrbanCenter,
//         GoldMine
//     }
//
//     public class Node<Coordinate> : INode, INode<Coordinate>, IEquatable<Node<Coordinate>>
//         where Coordinate : IEquatable<Coordinate>
//     {
//         private List<INode<Coordinate>> neighbors = new List<INode<Coordinate>>();
//         private bool isBlocked = false;
//         private NodeTypes nodeType;
//         private Coordinate coordinate;
//
//         public void SetCoordinate(Coordinate coordinate)
//         {
//             this.coordinate = coordinate;
//         }
//
//         public void AddNeighbor(INode<Coordinate> neighbor)
//         {
//             neighbors.Add(neighbor);
//         }
//
//         public void SetNeighbors(ICollection<INode<Coordinate>> neighbors)
//         {
//             throw new NotImplementedException();
//         }
//
//         ICollection<INode<Coordinate>> INode<Coordinate>.GetNeighbors()
//         {
//             return GetNeighbors();
//         }
//
//         public RTSNodeType GetRTSNodeType()
//         {
//             throw new NotImplementedException();
//         }
//
//         public int GetCost()
//         {
//             throw new NotImplementedException();
//         }
//
//         public void SetCost(int newCost)
//         {
//             throw new NotImplementedException();
//         }
//
//         public SimNodeType NodeType { get; set; }
//         public int Food { get; set; }
//
//         public List<INode<Coordinate>> GetNeighbors()
//         {
//             return neighbors;
//         }
//
//         public List<INode<Coordinate>> PassNeighbors()
//         {
//             foreach (var neighbor in neighbors)
//             {
//                 neighbor.QuitNeighbor(this, neighbor.GetNeighbors());
//             }
//
//             return neighbors;
//         }
//
//         public void QuitNeighbor(INode<Coordinate> neighborToQuit, List<INode<Coordinate>> neighborsList)
//         {
//             neighborsList.Remove(neighborToQuit);
//         }
//
//         public void SetNeighbors(List<INode<Coordinate>> neighbors)
//         {
//             foreach (INode<Coordinate> neighbor in neighbors)
//             {
//                 neighbor.AddNeighbor(this);
//             }
//
//
//             this.neighbors = neighbors;
//         }
//
//         public Coordinate GetCoordinate()
//         {
//             return coordinate;
//         }
//
//         public bool EqualsTo(INode other)
//         {
//             return coordinate.Equals(((Node<Coordinate>)other).coordinate);
//         }
//
//         public bool IsBlocked()
//         {
//             return isBlocked;
//         }
//
//         public NodeTypes GetNodeType()
//         {
//             return nodeType;
//         }
//
//         public void SetNodeType(NodeTypes nodeType)
//         {
//             this.nodeType = nodeType;
//         }
//
//         public bool Equals(Node<Coordinate> other)
//         {
//             if (other == null) return false;
//             return coordinate.Equals(other.coordinate);
//         }
//
//         public bool Equals(Coordinate other)
//         {
//             throw new NotImplementedException();
//         }
//     }
//
//     public class GoldMineNode<Coordinate> : Node<Coordinate>
//         where Coordinate : IEquatable<Coordinate>
//     {
//         private int goldAmount;
//         private int foodAmount;
//         private int maxFoodAmount;
//         private int maxGoldAmount;
//         private bool beingMined = false;
//
//         public GoldMineNode(Coordinate coordinate, int initialGoldAmount, int initialFoodAmount)
//         {
//             SetCoordinate(coordinate);
//             goldAmount = initialGoldAmount;
//             foodAmount = initialFoodAmount;
//             //SetNodeType(NodeTypes.GoldMine);
//         }
//
//         public GoldMineNode()
//         {
//             foodAmount = maxFoodAmount;
//             goldAmount = maxGoldAmount;
//             //SetNodeType(NodeTypes.GoldMine);
//         }
//
//         public int GetGoldAmount()
//         {
//             return goldAmount;
//         }
//
//         public void SetGoldAmount(int goldAmount)
//         {
//             this.goldAmount = goldAmount;
//         }
//
//         public int GetFoodAmount()
//         {
//             return foodAmount;
//         }
//
//         public void SetFoodAmount(int foodAmount)
//         {
//             if (foodAmount > maxFoodAmount)
//                 foodAmount = maxFoodAmount;
//
//             this.foodAmount = foodAmount;
//         }
//
//         public int GetMaxFoodAmount()
//         {
//             return maxFoodAmount;
//         }
//
//         public void SetMaxFoodAmount(int maxFoodAmount)
//         {
//             this.maxFoodAmount = maxFoodAmount;
//         }
//
//         public void SetMaxGoldAmount(int maxGoldAmount)
//         {
//             this.maxGoldAmount = maxGoldAmount;
//         }
//
//         public bool HasFood()
//         {
//             return foodAmount > 0;
//         }
//
//         public void ConsumeFood(int amount = 1)
//         {
//             foodAmount -= amount;
//             if (foodAmount < 0) foodAmount = 0;
//         }
//
//         public bool HasGold()
//         {
//             return goldAmount > 0;
//         }
//
//         public void MineGold(int amount = 1)
//         {
//             goldAmount -= amount;
//             if (goldAmount < 0) goldAmount = 0;
//         }
//
//         public bool IsBeingMined()
//         {
//             return beingMined;
//         }
//
//         public void SetBeingMined(bool beingMined)
//         {
//             this.beingMined = beingMined;
//         }
//     }
//
//     public class UrbanCenterNode<Coordinate> : Node<Coordinate>
//         where Coordinate : IEquatable<Coordinate>
//     {
//         private int gold = 0;
//         private int agentCapacity = 10;
//
//         public UrbanCenterNode(Coordinate coordinate)
//         {
//             SetCoordinate(coordinate);
//             gold = 0;
//             SetNodeType(NodeTypes.UrbanCenter);
//         }
//
//         public UrbanCenterNode()
//         {
//             gold = 0;
//             SetNodeType(NodeTypes.UrbanCenter);
//         }
//
//         public int GetGold()
//         {
//             return gold;
//         }
//
//         public void SetGold(int amount)
//         {
//             gold = amount;
//         }
//
//         public void AddGold(int amount)
//         {
//             gold += amount;
//         }
//
//         public int GetAgentCapacity()
//         {
//             return agentCapacity;
//         }
//
//         public bool CanGenerateAgent()
//         {
//             return agentCapacity > 0;
//         }
//
//         public void GenerateAgent()
//         {
//             if (CanGenerateAgent())
//             {
//                 agentCapacity--;
//                 // Logic for creating a new miner/caravan.
//             }
//         }
//     }
//
//     public class SimNode<Coordinate> : INode<Coordinate>
//         where Coordinate : IEquatable<Coordinate>
//     {
//         private Coordinate coordinate;
//         private int cost;
//         public int food;
//
//         private ICollection<INode<Coordinate>> neighbors;
//
//         public SimNode()
//         {
//         }
//
//         public SimNode(Coordinate coord)
//         {
//             coordinate = coord;
//         }
//
//         public SimNodeType NodeType { get; set; }
//
//         public bool Equals(INode<Coordinate> other)
//         {
//             return other != null && coordinate.Equals(other.GetCoordinate());
//         }
//
//         public bool IsBlocked()
//         {
//             return false;
//         }
//
//         public void SetCoordinate(Coordinate coordinate)
//         {
//             this.coordinate = coordinate;
//         }
//
//         public Coordinate GetCoordinate()
//         {
//             return coordinate;
//         }
//
//         public void AddNeighbor(INode<Coordinate> neighbor)
//         {
//             throw new NotImplementedException();
//         }
//
//         List<INode<Coordinate>> INode<Coordinate>.GetNeighbors()
//         {
//             throw new NotImplementedException();
//         }
//
//         public List<INode<Coordinate>> PassNeighbors()
//         {
//             throw new NotImplementedException();
//         }
//
//         public void QuitNeighbor(INode<Coordinate> neighborToQuit, List<INode<Coordinate>> neighborsList)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void SetNeighbors(List<INode<Coordinate>> neighbors)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void SetNeighbors(ICollection<INode<Coordinate>> neighbors)
//         {
//             this.neighbors = neighbors;
//         }
//
//         public ICollection<INode<Coordinate>> GetNeighbors()
//         {
//             return neighbors;
//         }
//
//         public NodeTypes GetNodeType()
//         {
//             return NodeTypes.Default;
//         }
//
//         public SimNodeType GetSimNodeType()
//         {
//             return NodeType;
//         }
//
//         public int GetCost()
//         {
//             return cost;
//         }
//
//         public void SetCost(int newCost)
//         {
//             cost = newCost;
//         }
//
//         public bool Equals(Coordinate other)
//         {
//             return coordinate.Equals(other);
//         }
//
//         public bool EqualsTo(INode<Coordinate> other)
//         {
//             return coordinate.Equals(other.GetCoordinate());
//         }
//
//         protected bool Equals(SimNode<Coordinate> other)
//         {
//             return coordinate.Equals(other.coordinate);
//         }
//
//         public override bool Equals(object obj)
//         {
//             if (ReferenceEquals(null, obj)) return false;
//             if (ReferenceEquals(this, obj)) return true;
//             if (obj.GetType() != GetType()) return false;
//             return Equals((SimNode<Coordinate>)obj);
//         }
//
//         public override int GetHashCode()
//         {
//             return EqualityComparer<Coordinate>.Default.GetHashCode(coordinate);
//         }
//     }
// }