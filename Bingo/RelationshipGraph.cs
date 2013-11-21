using System;
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
			Dictionary<String,Boolean> seen = new Dictionary<String, Boolean>();
			int levelCount = 0;
			GraphNode From = GetNode (F);
			GraphNode To = GetNode (T);
			
			if (From == null || To == null || From == To)
				return null;
			
			level.Add (From);
			levels.Add (level);
			while (true)
			{
				//	Console.WriteLine ("levelCount = " + levels.Count);
				level = new List<GraphNode>();
				foreach(GraphNode n in levels[levels.Count-1])
				{
					foreach (GraphEdge e in n.GetEdges())
					{
						if (e.To() == T) // is this the end node?
								goto EndBuild;
						if (!seen.ContainsKey(e.To()))// Deduplication
						{
							level.Add (e.ToNode ());
							seen.Add (e.To(),true);
						}
					}
				}
				levels.Add (level);
			}
		EndBuild:
			// figure out path in reverse
			path.Add (To);
			levelCount = levels.Count-1;
			try{
				while (path[path.Count-1] != From && levelCount >= 0)
				{
					
					//Console.WriteLine ("DEBUG: levelCount = " + levelCount + ", path.Count = " + path.Count);
					foreach (GraphEdge e in path[path.Count-1].GetEdges())
					{
						if (levels[levelCount].Contains (e.ToNode()))
						{
							path.Add(e.ToNode ());
							goto NextInWhile;
						}
					}
					// if we got here then this node had no up edge, so we look for a down edge
					foreach(GraphNode n in levels[levelCount]){
						foreach(GraphEdge e in n.GetEdges()){
							if (e.ToNode() == path[path.Count-1]){
								path.Add(n);
								goto NextInWhile;
							}
						}
					}
				NextInWhile: 	
						levelCount --;	
				}
			} catch (Exception e){
				Console.WriteLine (e.Message);
			}
			path.Reverse();
			if (path.Count == 1)
				path = null;
			return path;
		}
		
		public void decendents(String name){
			GraphNode root = GetNode (name);
			List<GraphNode> thisGeneration = new List<GraphNode>();
			List<GraphNode> nextGeneration = new List<GraphNode>();
			thisGeneration.Add (root);
			Console.WriteLine ("Decendents of " + root.Name() + ":");
			int generation = 0;
			List<String> Strings = new List<String>();
			
			while (thisGeneration.Count > 0){
				
				
				if (generation == 1)
					Console.Write ("Children: ");
				else if (generation > 1){
					for (int i = generation; i > 2; i --){
						Console.Write ("Great ");
					}
					Console.Write ("Grandchildren: ");
				}
				if (generation > 0)
					Console.WriteLine (String.Join (", ", (String[]) Strings.ToArray() ));
				
				generation ++;
				
				Strings = new List<string>();
				
				foreach (GraphNode n in thisGeneration){
					foreach (GraphEdge e in n.GetEdges("child"))
					{
						nextGeneration.Add (e.ToNode());
						Strings.Add (e.To ());
					}
				}

				thisGeneration = nextGeneration;
				nextGeneration = new List<GraphNode>();
			}
			
		}
		
		// cousins
		public void cousins(String person,int n, int k){
			n +=1;
			Dictionary<String,bool> seen = new Dictionary<String, bool>();
			Dictionary<GraphNode,bool> topLevel = new Dictionary<GraphNode,bool>();
			Dictionary<GraphNode,bool> endLevel = new Dictionary<GraphNode,bool>();
			if (GetNode(person) == null){
				Console.WriteLine ("No such person: " + person);
				return;
			}
			topLevel.Add (GetNode (person),true);
			seen.Add (person,true);
			for ( int i = 0; i < n + k; i ++){// go up n generations
				Dictionary<GraphNode,bool> level = new Dictionary<GraphNode,bool>();
				foreach (GraphNode gn in topLevel.Keys){
					foreach (GraphEdge e in gn.GetEdges ("parent")){
						if (!level.ContainsKey ( e.ToNode() ) )
							level.Add (e.ToNode(),true);
						if (i < n-1) // only do this up through the last generation
							seen.Add (e.To(),true);
					}
				}
				if (i == n-1 && k != 0)
					endLevel = level;
				topLevel = level;
			}
			
			// now go get decendents on the n'th +- K generation
			for (int i = 0; i < n ; i ++){
				Dictionary<GraphNode,bool> level = new Dictionary<GraphNode,bool>();
				foreach (GraphNode gn in topLevel.Keys){
					if (seen.ContainsKey(gn.Name ()))
					    continue;
					if (!seen.ContainsKey (gn.Name()))
						seen.Add (gn.Name(), true);
					foreach (GraphEdge e in gn.GetEdges ("child")){
						if (!level.ContainsKey ( e.ToNode() ) )
							level.Add (e.ToNode(),true);
					}
				}
				topLevel = level;
			}
			// now go get decendents on the n'th +- K generation
			for (int i = 0; i < n +k ; i ++){
				Dictionary<GraphNode,bool> level = new Dictionary<GraphNode,bool>();
				foreach (GraphNode gn in endLevel.Keys){
					if (seen.ContainsKey(gn.Name ()))
					    continue;
					if (!seen.ContainsKey (gn.Name()))
						seen.Add (gn.Name(), true);
					foreach (GraphEdge e in gn.GetEdges ("child")){
						if (!level.ContainsKey ( e.ToNode() ) )
							level.Add (e.ToNode(),true);
					}
				}
				endLevel = level;
			}
			
			
			// print out results
			List<String> printArray = new List<String>();
			foreach(GraphNode gn in topLevel.Keys)
				if (!seen.ContainsKey(gn.Name ()))
					printArray.Add(gn.Name ());
			foreach(GraphNode gn in endLevel.Keys)
				if (!seen.ContainsKey(gn.Name ()))
					printArray.Add(gn.Name ());
				
			
			Console.WriteLine (String.Join (", ",(String[]) printArray.ToArray()) );
		}
    }
}
