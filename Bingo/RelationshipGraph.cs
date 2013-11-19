﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bingo
{
    /// <summary>
    /// Represents a directed labeled graph with a string name at each node
    /// and a string label for each edge.
    /// </summary>
    class RelationshipGraph
    {
        /*
         *  This data structure contains a list of nodes (each of which has
         *  an adjacency list) and a dictionary (hash table) for efficiently 
         *  finding nodes by name
         */
        /*  nodes property should probably be private, with accessor 
	 *  functions, but it's public at the moment.
	 */
        public List<GraphNode> nodes;
        private Dictionary<String, GraphNode> nodeDict;

        // constructor builds empty relationship graph
        public RelationshipGraph()
        {
            nodes = new List<GraphNode>();
            nodeDict = new Dictionary<String,GraphNode>();
        }

        // AddNode creates and adds a new node if there isn't already one by that name
        public void AddNode(string name)
        {
            if (!nodeDict.ContainsKey(name))
            {
                GraphNode n = new GraphNode(name);
                nodes.Add(n);
                nodeDict.Add(name, n);
            }
        }

        // AddEdge adds the edge, creating endpoint nodes if necessary.
        // Edge is added to adjacency list of from edges.
        public void AddEdge(string name1, string name2, string relationship) 
        {
            AddNode(name1);                     // create the node if it doesn't already exist
            GraphNode n1 = nodeDict[name1];     // now fetch a reference to the node
            AddNode(name2);
            GraphNode n2 = nodeDict[name2];
            GraphEdge e = new GraphEdge(n1, n2, relationship);
            n1.AddIncidentEdge(e);
        }

        // Get a node by name using dictionary
        public GraphNode GetNode(string name)
        {
            if (nodeDict.ContainsKey(name))
                return nodeDict[name];
            else
                return null;
        }

        // Return a text representation of graph
        public void dump()
        {
            foreach (GraphNode n in nodes)
            {
                Console.Write(n.ToString());
            }
        }
		
		public void printOrphans()
		{
			Console.WriteLine ("Orphans:");
			foreach(GraphNode n in nodes)
			{
				if (n.GetEdges("parent").Count == 0)
				Console.WriteLine("\t" + n.Name());
			}
		}
		public List<GraphNode> ShortestPath(String F, String T){
			// do a breadth first search to find the shortest path.
			List<GraphNode> path = new List<GraphNode>();
			List<List<GraphNode>> levels = new List<List<GraphNode>>();
			List<GraphNode> level = new List<GraphNode>();
			int levelCount = 0;
			
			
			GraphNode From = GetNode (F);
			GraphNode To = GetNode (T);
			GraphNode Pointer;
			
			level.Add (From);
			levels.Add (level);
			
			while (!(levels[levelCount].Contains(To)))
			{
				levelCount++;
				level = new List<GraphNode>();
				foreach(GraphNode n in levels[levelCount-1])
				{
					foreach (GraphEdge e in n.GetEdges())
					{
						level.Add (e.ToNode ());
					}
				}
				levels.Add (level);
			}
			
			// figure out path in reverse
			path.Add (To);
			while (path[path.Count-1] != From)
			{
				levelCount --;
				foreach (GraphEdge e in path[path.Count-1].GetEdges())
				{
					if (levels[levelCount].Contains (e.ToNode()))
					{
						path.Add(e.ToNode ());
						break;
					}
				}
				
			}
			
			path.Reverse();
			return path;
		}
    }
}
