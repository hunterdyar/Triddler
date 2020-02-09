using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Blooper.Triangles{
[CreateAssetMenu(fileName = "puzzle",menuName = "Triddle/puzzle", order = 120)]
    public class TriddlePuzzle : ScriptableObject
    {
        public TridSize tridSize;
        public Dictionary<Vector2Int,int> level;
        public int[] t_solution;
        public int[] tl_solution;
        public int[] tr_solution;
        public int[] b_solution;
        public int[] bl_solution;
        public int[] br_solution;

        public static int[][] GetSolutions(PuzzleEdges edges, Dictionary<Vector2Int,int>level)
        {
            // //int 0-5 = t,tr,tl,bl,b,br. (same as the (axb)+(cxd) syntax plus (exf).)
            int[][] solutions = new int[6][];
            solutions[0] = new int[edges.topEdgeTriangles.Count];
            for(int i = 0;i<edges.topEdgeTriangles.Count;i++){
                Triangle mt = edges.topEdgeTriangles[edges.topEdgeTriangles.Count-1-i];
                Vector2Int m = mt.position;
                List<int> solution = new List<int>();
                solution.Add(level[m]);
                bool marching = true;
                while(marching){
                    if(level.ContainsKey(Triangle.MarchNegativeSlope(m,1))){
                        m = Triangle.MarchNegativeSlope(m,1);
                        solution.Add(level[m]);
                    }else{
                        marching = false;
                    }
                }
                Debug.Log("A column was found! - "+solution.Count);
                solutions[i] = solution.ToArray();
            }
            //
            return solutions;
        }
    }
}