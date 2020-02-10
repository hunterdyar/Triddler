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
    
[CreateAssetMenu(fileName = "puzzle",menuName = "Triddle/puzzle", order = 120)]
    public class TriddlePuzzle : ScriptableObject
    {
        public TridSize tridSize;
        public Dictionary<Vector2Int,int> level;
        public int[][] t_solution;
        public int[][] tl_solution;
        public int[][] tr_solution;
        public int[][] b_solution;
        public int[][] bl_solution;
        public int[][] br_solution;
    }
}