using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Blooper.Triangles{
    public enum MarchDirections{
        horizontal_right,
        horizontal_left,
        positiveSlope_right,
        positiveSlope_left,
        negativeSlope_right,
        negativeSlope_left
    }
    public enum LevelEdge{
        top,
        topRight,
        topLeft,
        bottom,
        bottomRight,
        bottomLeft
    }
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

        public static int[][] GetSolutionsForEdge(List<Triangle> edgeTriangles, Dictionary<Vector2Int,int>level,MarchDirections mdir)
        {
            int[][] solutions = new int[edgeTriangles.Count][];//our array of arrays.
            for(int i = 0;i<edgeTriangles.Count;i++){
                Triangle mt = edgeTriangles[edgeTriangles.Count-1-i];
                Vector2Int m = mt.position;
                List<int> solution = new List<int>();
                solution.Add(level[m]);
                bool marching = true;
                while(marching){
                    Vector2Int key = Triangle.March(m,mdir);
                    if(level.ContainsKey(key)){
                        m = key;//key is in while, m is out of this scope.
                        solution.Add(level[m]);
                    }else{
                        marching = false;
                    }
                }
            solutions[i] = solution.ToArray();
            }
            //
            return solutions;
        }

        
    }
}