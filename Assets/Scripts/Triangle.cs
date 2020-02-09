using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blooper.Triangles {
    [System.Serializable]
    public class Triangle {
        //Triangle Implementation Scheme stolen from here:
        //https://github.com/mhwombat/grid/wiki/Implementation%3A-Triangular-tiles

        //I started rolling my own by defining triangles as vertices, which worked great for drawing and findint the path along axis,
        //but stunk for generating and level coordinate definitions.
        public Triangle (int x, int y, float _edgeLength = 1f) {
            position = new Vector2Int (x, y);
            edgeLength = _edgeLength;
            drawingObject = null;
            status = 0;
        }
        public Triangle (Vector2Int _position, float _edgeLength = 1f) {
            position = _position;
            edgeLength = _edgeLength;
            drawingObject = null;
            status = 0;
        }
        public Triangle (Vector2 positionf, float _edgeLength = 1f) {
            position = new Vector2Int ((int) positionf.x, (int) positionf.y);
            edgeLength = _edgeLength;
            drawingObject = null;
            status = 0;
        }
        public Vector2Int position;
        public int status;
        public float edgeLength;
        public GameObject drawingObject;
        //z = -x - y (when y is even).
        //z = -x - y + 1 (when y is odd).
        public bool pointsUp { get { return position.x % 2 == 0; } } //f the x-coordinate is even, the triangle points up; if it's odd, the triangle points down.
        private Vector3Int position3 { get { int zelta = 0; if (position.y % 2 != 0) { zelta = 1; } return new Vector3Int (position.x, position.y, -position.x - position.y + zelta); } }

        public bool IsValid () {

            if (position.y > 0) {
                if (Mathf.Abs (position.x) % 2 == 0 && Mathf.Abs (position.y) % 2 == 0) { return true; } //both even
                if (Mathf.Abs (position.x) % 2 != 0 && Mathf.Abs (position.y) % 2 != 0) { return true; } //both odd
            }
            // Debug.Log("position "+ position.ToString()+" invalid");
            return false;
        }
        public static bool PointsUp(Vector2Int from)
        {
            return from.x % 2 == 0;
        }
        public static Vector2Int MarchHorizontal(Vector2Int from,int dir = 1)
        {
            if(dir == 1){
                if(PointsUp(from)){
                    return from+new Vector2Int(1,1);
                }else{
                    return from+new Vector2Int(1,-1);
                }
            }else{
                if(PointsUp(from)){
                    return from+new Vector2Int(-1,1);
                }else{
                    return from+new Vector2Int(-1,-1);
                }
            }
        }
        public static Vector2Int MarchPositiveSlope(Vector2Int from,int dir = 1)
        {
            if(dir == 1)
            {
                if(PointsUp(from)){
                    return from+new Vector2Int(1,1);
                }else{
                    return from+new Vector2Int(-1,1);
                }
            }else
            {
                if(PointsUp(from)){
                    return from+new Vector2Int(1,-1);
                }else{
                    return from+new Vector2Int(-1,-1);
                }
            }
        }
        public static Vector2Int MarchNegativeSlope(Vector2Int from,int dir = 1)
        {
            if(dir == 1)
            {
                if(PointsUp(from)){
                    return from+new Vector2Int(1,-1);
                }else{
                    return from+new Vector2Int(1,-1);
                }
            }else
            {
                if(PointsUp(from)){
                    return from+new Vector2Int(-1,1);
                }else{
                    return from+new Vector2Int(-1,1);
                }
            }
        }
        public static bool IsValid (int x, int y) {
            if (Mathf.Abs (x) % 2 == 0 && Mathf.Abs (y) % 2 == 0) { return true; } //both even
            if (Mathf.Abs (x) % 2 != 0 && Mathf.Abs (y) % 2 != 0) { return true; } //both odd
            return false;
        }
        public static bool IsValid (Vector2Int k) {
            return IsValid (k.x, k.y);
        }
        public static float MinimumMovesAtoB (Triangle a, Triangle b) {
            //a = x1,y1,z1; b = x2,y2,z2
            //maximum (|x2-x1|, |y2-y1|, |z2-z1|)
            return Mathf.Max (Mathf.Abs (b.position.x - a.position.x), Mathf.Abs (b.position.y - a.position.y), Mathf.Abs (b.position3.z - a.position3.z));
        }
        public Vector2[] GetVertsInWorldSpace () {
            Vector2[] verts = new Vector2[3];
            if (pointsUp) {
                //bottom left
                verts[0] = new Vector2 ((float) position.x / 2, (float) position.y / 2) * edgeLength;
                verts[0] = verts[0] + new Vector2 (verts[0].y / 2, 0);
                //bottom right
                verts[1] = verts[0] + new Vector2 (edgeLength, 0);
                //top
                verts[2] = new Vector2 ((float) position.x / 2 + edgeLength / 2, (float) position.y / 2 + 1) * edgeLength;
                verts[2] = verts[2] + new Vector2 (verts[0].y / 2, 0);
            } else {
                //we define them as 2,1,0 so our mesh generator doesnt have flipped normals.

                //top left
                verts[2] = new Vector2 ((float) position.x / 2, ((float) (position.y - 1) / 2) + 1) * edgeLength;
                verts[2] = verts[2] + new Vector2 (verts[2].y / 2 - edgeLength / 2, 0);

                //top right
                verts[1] = verts[2] + new Vector2 (edgeLength, 0);
                //bottom
                verts[0] = new Vector2 ((float) position.x / 2 + edgeLength / 2, (float) (position.y - 1) / 2) * edgeLength;
                verts[0] = verts[0] + new Vector2 (verts[2].y / 2 - edgeLength / 2, 0);

            }
            return verts;
        }
        public Vector2[][] GetEdgesInWorldSpace () {
            Vector2[][] edges = new Vector2[3][];
            Vector2[] verts = GetVertsInWorldSpace ();
            edges[0] = new Vector2[2];
            edges[0][0] = verts[0];
            edges[0][1] = verts[1];

            edges[1] = new Vector2[2];
            edges[1][0] = verts[0];
            edges[1][1] = verts[2];

            edges[2] = new Vector2[2];
            edges[2][0] = verts[1];
            edges[2][1] = verts[2];

            return edges;
        }
        public Vector2 GetCentroidInWorldSpace () {
            Vector2[] verts = GetVertsInWorldSpace ();
            //one third the way from a vertex to the midpoint of the other vertices
            return Vector2.Lerp (Vector2.Lerp (verts[0], verts[1], 0.5f), verts[2], 1 / 3);

        }
    }
}