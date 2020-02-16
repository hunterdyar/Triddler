using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Blooper.Triangles{
    public enum LevelEdge{
        top,
        topRight,
        topLeft,
        bottom,
        bottomRight,
        bottomLeft
    }
    public class PuzzleEdges
    {
        public List<Triangle> topEdgeTriangles;
        public List<Triangle> topRightEdgeTriangles;
        public List<Triangle> topLeftEdgeTriangles;
        public List<Triangle> bottomLeftEdgeTriangles;
        public List<Triangle> bottomEdgeTriangles;

        public List<Triangle> bottomRightEdgeTriangles;
        public Dictionary<Triangle,int[]> edgeTriangleToSolutionMap;
        public PuzzleEdges(){
            //init map
            edgeTriangleToSolutionMap = new Dictionary<Triangle, int[]>();
            //init lists.
            topEdgeTriangles = new List<Triangle>();
            topRightEdgeTriangles = new List<Triangle>();
            topLeftEdgeTriangles = new List<Triangle>();
            bottomLeftEdgeTriangles = new List<Triangle>();
            bottomEdgeTriangles = new List<Triangle>();
            bottomRightEdgeTriangles = new List<Triangle>();
        }
        public LevelEdge EdgeTriangleIsOn(Triangle et)
        {
            if(topEdgeTriangles.Contains(et)){return LevelEdge.top;}
            else if(topRightEdgeTriangles.Contains(et)){return LevelEdge.topRight;}
            else if(topLeftEdgeTriangles.Contains(et)){return LevelEdge.topLeft;}
            else if(bottomEdgeTriangles.Contains(et)){return LevelEdge.bottom;}
            else if(bottomRightEdgeTriangles.Contains(et)){return LevelEdge.bottomRight;}
            else if(bottomLeftEdgeTriangles.Contains(et)){return LevelEdge.bottomLeft;}
            else{ 
                Debug.LogError("triangle doesnt lie on an edge",et.drawingObject);
                return LevelEdge.top;
            }
        }
        public static MarchDirections EdgeToSolutionDir(LevelEdge edge){
            //This code is contained in the GetSolutionsForEdge function but i needed it elsewhere too
            //fuck this is messy.
            MarchDirections mdir = MarchDirections.positiveSlope_left;
            if(edge == LevelEdge.top){
                mdir = MarchDirections.positiveSlope_left;}
            else if(edge == LevelEdge.topRight){
                mdir = MarchDirections.positiveSlope_left;}
            else if(edge == LevelEdge.topLeft){
                mdir = MarchDirections.horizontal_right;}
            else if(edge == LevelEdge.bottom){
                mdir = MarchDirections.negativeSlope_left;}
            else if(edge == LevelEdge.bottomRight){
                mdir = MarchDirections.negativeSlope_left;}
            else if(edge == LevelEdge.bottomLeft){
                mdir = MarchDirections.horizontal_right;}
                return mdir;
        }
        public int[][] GetSolutionsForEdge(LevelEdge edge, Dictionary<Vector2Int,int>level)
        {   
            MarchDirections mdir = MarchDirections.horizontal_right;
            List<Triangle> edgeTriangles = new List<Triangle>();
            //
            if(edge == LevelEdge.top){edgeTriangles = topEdgeTriangles;
                mdir = MarchDirections.positiveSlope_left;}
            else if(edge == LevelEdge.topRight){edgeTriangles = topRightEdgeTriangles;
                mdir = MarchDirections.positiveSlope_left;}
            else if(edge == LevelEdge.topLeft){edgeTriangles = topLeftEdgeTriangles;
                mdir = MarchDirections.horizontal_right;}
            else if(edge == LevelEdge.bottom){edgeTriangles = bottomEdgeTriangles;
                mdir = MarchDirections.negativeSlope_left;}
            else if(edge == LevelEdge.bottomRight){edgeTriangles = bottomRightEdgeTriangles;
                mdir = MarchDirections.negativeSlope_left;}
            else if(edge == LevelEdge.bottomLeft){edgeTriangles = bottomLeftEdgeTriangles;
                mdir = MarchDirections.horizontal_right;}
        

            int[][] solutions = new int[edgeTriangles.Count][];//our array of arrays.
            // for(int i = edgeTriangles.Count-1;i>0;i--){
            foreach(Triangle mt in edgeTriangles){
                // Triangle mt = edgeTriangles[i];//-1-i
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
                // solutions[i] = solution.ToArray();
                edgeTriangleToSolutionMap.Add(mt,solution.ToArray());
            }
            //
            return solutions;
        }
    }
}
