using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.FastLineRenderer;

namespace Blooper.Triangles{
    [System.Serializable]
    public struct TridSize{
        public TridSize(int top,int topRight,int topLeft,int bottmLeft,int numberOfColors){
            _top = top;
            _topRight = topRight;
            _topLeft = topLeft;
            _bottomLeft = bottmLeft;
            colors = numberOfColors;
        }
        //doodling and thinking reveals to me that:
        //opposing 2-sides pairs of a hexagon need to total the same length
        //wrap a hexagon with sides a,b,c,d,e,f
        //a+b = d+f
        //b+c = e+f
        //c+d = a+e
        //d+f = b+a
        //From here, we can calculate our bottom right and bottom left. if the abcdef started at the bottom left and went clockwise
        //e = (c+d)-a and f = (a+b)-d. 
        //thats how i get the bottom right and bottom left.
        //isnt math FUN.
        public int _top;
        public int _topRight;
        [Space]
        public int _topLeft;
        public int _bottomLeft;
        public int _bottomRight {get{return (_topLeft+_bottomLeft)-_topRight;}}
        public int _bottom {get{return (_top+_topRight)-_bottomLeft;}}
        [Space]
        public int colors;
        public Vector2Int top_topRight {get{return new Vector2Int(_top,_topRight);}}
        public Vector2Int topLeft_bottomLeft{get{return new Vector2Int(_topLeft,_bottomLeft);}}
        public override string ToString(){
            return "("+top_topRight.x+"+"+top_topRight.y+")x("+topLeft_bottomLeft.x+"+"+topLeft_bottomLeft.y+")x"+colors;
        }
        public static int TrisFromSideLength(int sideLength)
        {
            if(sideLength>1)
            {return (2*sideLength - 1);}
            else
            {return sideLength;}
        }
        public int topTris {get{return TrisFromSideLength(_top);}}
        public int topRightTris {get{return TrisFromSideLength(_topRight);}}
        public int topLeftTris {get{return TrisFromSideLength(_topLeft);}}
        public int bottomTris {get{return TrisFromSideLength(_bottom);}}
        public int bottomLeftTris {get{return TrisFromSideLength(_bottomLeft);}}
        public int bottomRightTris {get{return TrisFromSideLength(_bottomRight);}}
    }
    public class TriangleGridSystem: MonoBehaviour
    {
        public GameObject filledMeshMaker;
        public FastLineRenderer lineRenderer;
        public TriddlePuzzle puzzle;
        Dictionary<Vector2Int,Triangle> trid;
        Dictionary<Vector2,Triangle> trisByCentroid;
        PuzzleEdges puzzleEdges;
        void Start()
        {
            //Create our metadata
            SpawnGrid(puzzle.tridSize);
            //Create our gameObjects
            DrawAllTriangles();
            //Create our background of lines.
            DrawTriangleGrid();
            //Set the camera to the center.
            Camera.main.transform.position = (Vector3)GetVisualCenter()+Vector3.back*10;
            SetPuzzleSolution(puzzle);
            //Maybe we should store the vector2int/int version in this class?
            TriddlePuzzle.GetSolutionsForEdge(puzzleEdges.topEdgeTriangles,puzzle.level,MarchDirections.negativeSlope_right);
        }

        //Turns the big moma data into a list of positiosn and integers, which is how the data a) gets serialized and b) gets processed.
        //This is for, like, saving scene data into a puzzle. Usually the data will probably go the other way round.
        public void SetPuzzleSolution(TriddlePuzzle p){
            p.level = new Dictionary<Vector2Int,int>();
            foreach(KeyValuePair<Vector2Int,Triangle> vt in trid)
            {  
                p.level.Add(vt.Key,vt.Value.status);
            }
        }
        public Vector2 GetVisualCenter()
        {
            float xt=0;
            float yt=0;
            float length = trisByCentroid.Keys.Count;
            foreach(Vector2 center in trisByCentroid.Keys)
            {
                xt = xt+center.x;
                yt = yt+center.y;
            }
            return new Vector2(xt/length,yt/length);
        }
        void ClearData()
        {
            puzzleEdges = new PuzzleEdges();
            trid = new Dictionary<Vector2Int, Triangle>();
            trisByCentroid = new Dictionary<Vector2, Triangle>();
        }
        bool AddTriangle(Triangle o)
        {
            if(!trid.ContainsKey(o.position))
            {
                trid.Add(o.position,o);
                trisByCentroid.Add(o.GetCentroidInWorldSpace(),o);
                return true;
            }else{
                return false;
            }
        }
        void SpawnGrid(TridSize gs)
        {
            ClearData();
            //our origin will be if we extended the hexagon into a triangle alllll the way out to the bottom left.
            //so the bottom row is y = 0.
            //For the actual bottom row, it starts at some offset that we have to figure out, and that is the bottomLeft size.
            Vector2Int origin = Vector2Int.zero;
            //idfk guys something about the triangles being points up or down or where the origin should be. it works.
            if(gs._bottomLeft != 0){
                origin.x = gs._bottomLeft;
                if(gs._bottomLeft%2==0)
                {
                    origin.x = gs._bottomLeft+gs._bottom+1;
                }
            }
            Triangle o = new Triangle(origin);
            Vector2Int lastValid = origin;
            AddTriangle(o);//lets start somewhere and march around.
            Triangle t = o;//stamping triangle to march.
            //lets make the bottom row.
            //triangles on a side with n triangle faces at the side = 2n-2
            int w = 0;//eh
            for(int i = 0;i<gs.bottomTris-w;i++)
            {
                t = new Triangle(Triangle.MarchHorizontal(t.position,1));
                if(AddTriangle(t)){
                    lastValid = t.position;
                    if(t.pointsUp){
                        puzzleEdges.bottomEdgeTriangles.Add(t);
                    }
                };
            }
            //bottom right edge
            for(int i = 0;i<gs.bottomRightTris-w;i++)
            {
                t = new Triangle(Triangle.MarchPositiveSlope(t.position,1));
                if(AddTriangle(t)){
                    lastValid = t.position;
                    if(!t.pointsUp){
                        puzzleEdges.bottomRightEdgeTriangles.Add(t);
                    }    
                };

            }
            //topRight edge
            for(int i = 0;i<(gs.topRightTris-w);i++)
            {
                t = new Triangle(Triangle.MarchNegativeSlope(t.position,-1));
                if(AddTriangle(t)){
                    lastValid = t.position;
                    if(t.pointsUp){
                        puzzleEdges.topRightEdgeTriangles.Add(t);
                    }    
                };

            }
            //top edge
            for(int i = 0;i<gs.topTris-w;i++)
            {
                t = new Triangle(Triangle.MarchHorizontal(t.position,-1));
                if(AddTriangle(t)){
                    lastValid = t.position;
                    //
                    if(!t.pointsUp){
                        puzzleEdges.topEdgeTriangles.Add(t);
                    }
                };
            }
            //top left edge
            for(int i = 0;i<gs.topLeftTris-w;i++)
            {
                t = new Triangle(Triangle.MarchPositiveSlope(t.position,-1));
                if(AddTriangle(t)){
                    lastValid = t.position;
                    if(t.pointsUp){
                        puzzleEdges.topLeftEdgeTriangles.Add(t);
                    }
                };
            }
            //bottlmLeft edge
            for(int i = 0;i<gs.bottomLeftTris-w;i++)
            {
                t = new Triangle(Triangle.MarchNegativeSlope(t.position,1));
                if(AddTriangle(t)){
                    lastValid = t.position;
                    if(!t.pointsUp){
                        puzzleEdges.bottomLeftEdgeTriangles.Add(t);
                    }    
                };
            }
            //Alright so we have the outline.
            SpawnFill();
        }
        //Goes bottom to top, left to right, and fills in each row with triangles.
        void SpawnFill(){
            int min = int.MaxValue;
            int max = int.MinValue;
            foreach(Vector2Int o in trid.Keys)
            {
                    if(o.y < min){
                        min = o.y;
                    }
                    if(o.y > max){
                        max = o.y;
                    }
            }
            if(min == max){ 
                Debug.Log("only one row?");
                return;//Only found one item in this row.
            }
            if(min == int.MaxValue)
            {
                Debug.Log("something is wrong. empty data?");
                return;//found nothing. 
            }
            if(min%2!=0){
                min++;
            }
            for(int i = min;i<=max;i=i+2){
                SpawnFillHorizontalRow(i);
            }

        }
        //used by SpawnFill();
        void SpawnFillHorizontalRow(int row){
            if(row%2 != 0){
                // Debug.Log("should use even rows only.");
                return;
            }
            int min = int.MaxValue;
            int max = int.MinValue;
            foreach(Vector2Int o in trid.Keys)
            {
                if(o.y == row){
                    if(o.x < min){
                        min = o.x;
                    }
                    if(o.x > max){
                        max = o.x;
                    }
                }
            }
            if(min == max){ 
                return;//Only found one item in this row.
            }
            if(min == int.MaxValue)
            {
                return;//found nothing in this row.
            }
            Vector2Int left = new Vector2Int(min,row);
            Vector2Int right = new Vector2Int(max,row);
            Triangle hf = new Triangle(min,row);
            
            for(int i = min;i<max;i++){
                hf = new Triangle(Triangle.MarchHorizontal(hf.position,1));
                AddTriangle(hf);
            }

        }
        
        void DrawTriangleGrid()
        {
            foreach(Triangle t in trid.Values){
                DrawTriangleOutline(t);
            }
        }
        void DrawAllTriangles()
        {
            foreach(Triangle t in trid.Values){
                DrawTriangle(t);
            }
        }
        void DrawTriangleOutline(Triangle t)
        {
            Vector2[][] edges = t.GetEdgesInWorldSpace();
            foreach(Vector2[] edge in edges)
            {
                FastLineRendererProperties props = new FastLineRendererProperties();
                props.Start = edge[0]+(Vector2)transform.position;
                props.End = edge[1]+(Vector2)transform.position;
                props.Radius = 0.02f;
                lineRenderer.AddLine(props);
            }
            lineRenderer.Apply();
        }
        void DrawTriangle(int x,int y)
        {
            DrawTriangle(trid[new Vector2Int(x,y)]);
        }
        void DrawTriangle(Triangle t)
        {
            MeshRenderer filledMesh;
            if(t.drawingObject == null){
                filledMesh = GameObject.Instantiate(filledMeshMaker,Vector3.zero,Quaternion.identity,transform).GetComponent<MeshRenderer>();
                t.drawingObject = filledMesh.gameObject;
            }else{
                filledMesh = t.drawingObject.GetComponent<MeshRenderer>();
            }

            Mesh mesh = new Mesh();
            Vector2[] verts2 = t.GetVertsInWorldSpace();
            Vector2[] verts2Local = new Vector2[verts2.Length];//3
            Vector3[] verts3 = new Vector3[verts2.Length];//3
            Vector3 offset = t.GetCentroidInWorldSpace();//the mesh object will be positioned AT the centroid. NEAT.
            for(int i = 0;i<verts2.Length;i++)
            {
                verts2Local[i] = verts2[i] - (Vector2)offset;//same but Vector2 cus why bother casting a whole array later for the polygon collider2d.
                verts3[i] = (Vector3)verts2[i]-offset;//offset by vertex 0...
                
            }
            
            int[] tris = new int[]{2,1,0};//simple enough lol.
        
            mesh.vertices = verts3;
            mesh.triangles = tris;
            //
            filledMesh.transform.position = transform.position+offset;//un-offset. By this offset so the position is the centroid, and by the parent, for moving the grid around. Now our triangles are positioned sensibly. 
            
            //and we can do sorting/filtering by world positions or transforms, which is easier than just naked data because unity has a lot of features for it
            //already built it. I THINK.
            //
            filledMesh.GetComponent<MeshFilter>().mesh = mesh;
            filledMesh.GetComponent<PolygonCollider2D>().points = verts2Local;
            
            filledMesh.material.color = Color.black;
            filledMesh.enabled = false;
            filledMesh.GetComponent<TriangleInteractor>().triangle = t;
        }
    }
}