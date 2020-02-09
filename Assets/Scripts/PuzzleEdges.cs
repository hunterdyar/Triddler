using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Blooper.Triangles{
    public class PuzzleEdges
    {
        public List<Triangle> topEdgeTriangles;
        public List<Triangle> topRightEdgeTriangles;
        public List<Triangle> topLeftEdgeTriangles;
        public List<Triangle> bottomLeftEdgeTriangles;
        public List<Triangle> bottomEdgeTriangles;

        public List<Triangle> bottomRightEdgeTriangles;

        public PuzzleEdges(){
            //init lists.
            topEdgeTriangles = new List<Triangle>();
            topRightEdgeTriangles = new List<Triangle>();
            topLeftEdgeTriangles = new List<Triangle>();
            bottomLeftEdgeTriangles = new List<Triangle>();
            bottomEdgeTriangles = new List<Triangle>();
            bottomRightEdgeTriangles = new List<Triangle>();
        }

    }
}
