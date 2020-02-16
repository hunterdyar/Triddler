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
        [Header("Config")]
        public GameObject filledMeshMaker;
        public FastLineRenderer lineRenderer;
        public HintDisplay hintDisplay;
        [Header("Settings")]
        public TriddlePuzzle puzzle;
        Dictionary<Vector2Int,Triangle> trid;
        Dictionary<Vector2Int,int> levelData;
        Dictionary<Vector2,Triangle> trisByCentroid;
        PuzzleEdges puzzleEdges;
        Coroutine validation;
        void Start()
        {
            //Load our levelData from a file into the levelData dictionary.
            LoadLevel();
        }
        public Dictionary<Vector2Int,int> LevelDataFromCurrentState()
        {
            Dictionary<Vector2Int,int> data = new Dictionary<Vector2Int, int>();
            foreach(Triangle t in trid.Values)
            {
                Vector2Int key = t.position;
                int value = t.status;
                data.Add(key,value);
            }
            return data;
        }
        [ContextMenu("Save level to Text")]
        void SetPuzzleTextFromData()
        {
            puzzle.level = LevelDataFromCurrentState();
            puzzle.SetLevelAsTextFromLevel();
        }
        [ContextMenu("Load Level")]
        public void LoadLevel()
        {
            //Clear current level...
            //destroy children...
            puzzle.SetLevelFromLevelAsText();
            //copy puzzle data to here
            levelData = new Dictionary<Vector2Int, int>(puzzle.level);
            //wait we want to start with a blank slate.
            foreach(Vector2Int key in puzzle.level.Keys){
                levelData[key] = 0;
            }
            //
            ClearData();
            SpawnGrid(puzzle.tridSize);
            DrawAllTriangles();
            DrawTriangleGrid();
            Camera.main.transform.position = (Vector3)GetVisualCenter()+Vector3.back*10;
            SetPuzzleSolution(puzzle);
            hintDisplay.DrawPuzzleHint(puzzleEdges);
        }
        public void SetPuzzleSolution(TriddlePuzzle p){
            Dictionary<Vector2Int, int> level = new Dictionary<Vector2Int,int>();
            foreach(KeyValuePair<Vector2Int,Triangle> vt in trid)
            {  
                level.Add(vt.Key,vt.Value.status);
            }
            //
            p.t_solution = puzzleEdges.GetSolutionsForEdge(LevelEdge.top,level);
            p.tl_solution = puzzleEdges.GetSolutionsForEdge(LevelEdge.topLeft,level);
            p.tr_solution = puzzleEdges.GetSolutionsForEdge(LevelEdge.topRight,level);
            p.b_solution = puzzleEdges.GetSolutionsForEdge(LevelEdge.bottom,level);
            p.br_solution = puzzleEdges.GetSolutionsForEdge(LevelEdge.bottomRight,level);
            p.bl_solution = puzzleEdges.GetSolutionsForEdge(LevelEdge.bottomLeft,level);
            //
            p.level = level;//not sure about this one.
            //
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
            lineRenderer.Reset();
            if(trid !=null)
            {
                //destroy all triangle objects
                foreach(Triangle t in trid.Values)
                {
                    Destroy(t.drawingObject);
                }
            }
            //destroy all hint drawing objects
            hintDisplay.Reset();
            //reset our data
            puzzleEdges = new PuzzleEdges();
            trid = new Dictionary<Vector2Int, Triangle>();
            trisByCentroid = new Dictionary<Vector2, Triangle>();
            //
            //garbage collection should do the rest.
        }
        public void TriangleUpdated(Triangle t)
        {
            //Update our storage.
            levelData[t.position] = t.selectedStatus;
            StopCoroutine(ValidatePuzzle());
            validation = StartCoroutine(ValidatePuzzle());
        }
        [ContextMenu("Validate")]
        public IEnumerator ValidatePuzzle()
        {
            //update levelData
            bool won = true;
            //compare puzzle.level to levelData
            foreach(Vector2Int key in puzzle.level.Keys)
            {
                if(!levelData.ContainsKey(key)){won = false;break;}
                if(levelData[key] != puzzle.level[key]){won = false; break;}
                yield return null;
            }
            if(won){
                YouWon();
            }
        }
        void YouWon()
        {
            Debug.Log("o.K.");
            hintDisplay.Reset();
            GameObject.FindObjectOfType<Selector>().enabled = false;
        }
        bool AddTriangle(Triangle o)
        {
            if(!trid.ContainsKey(o.position))
            {
                //set value from level data.
                if(puzzle.level.ContainsKey(o.position))
                {
                    o.status = puzzle.level[o.position];
                }else{
                    o.status = 0;
                }


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
            for(int i = 0;i<gs.bottomTris;i++)
            {
                t = new Triangle(Triangle.MarchHorizontal(t.position,1));
                if(AddTriangle(t)){
                    lastValid = t.position;
           
                };
            }
            //bottom right edge
            for(int i = 0;i<gs.bottomRightTris;i++)
            {
                t = new Triangle(Triangle.MarchPositiveSlope(t.position,1));
                if(AddTriangle(t)){
                    lastValid = t.position;
              
                };

            }
            //topRight edge
            for(int i = 0;i<gs.topRightTris;i++)
            {
                t = new Triangle(Triangle.MarchNegativeSlope(t.position,-1));
                if(AddTriangle(t)){
                    lastValid = t.position;
 
                }

            }
            //top edge
            for(int i = 0;i<gs.topTris;i++)
            {
                t = new Triangle(Triangle.MarchHorizontal(t.position,-1));
                if(AddTriangle(t)){
                    lastValid = t.position;
                    //

                };
            }
            //top left edge
            for(int i = 0;i<gs.topLeftTris;i++)
            {
                t = new Triangle(Triangle.MarchPositiveSlope(t.position,-1));
                if(AddTriangle(t)){
                    lastValid = t.position;

                };
            }
            //bottlmLeft edge
            for(int i = 0;i<gs.bottomLeftTris;i++)
            {
                t = new Triangle(Triangle.MarchNegativeSlope(t.position,1));
                if(AddTriangle(t)){
                    lastValid = t.position;
        
                };
            }
         

            //Alright so we have the outline.
            SpawnFill();

            IdentifyPuzzleEdges();
        }

        void IdentifyPuzzleEdges(){
            foreach(Triangle t in trid.Values)
            {
                Vector2Int pos = t.position;
                Vector2Int right = Triangle.MarchHorizontal(pos,1);
                Vector2Int left = Triangle.MarchHorizontal(pos,-1);
                Vector2Int posRight = Triangle.MarchPositiveSlope(pos,1);
                Vector2Int posLeft = Triangle.MarchPositiveSlope(pos,-1);
                Vector2Int negRight = Triangle.MarchNegativeSlope(pos,1);
                Vector2Int negLeft = Triangle.MarchNegativeSlope(pos,-1);

                if(!t.pointsUp){//top, bottom left, bottom right
                    //top
                    if(!trid.ContainsKey(posRight) && !trid.ContainsKey(negLeft)){
                        puzzleEdges.topEdgeTriangles.Add(t);
                    }else if(trid.ContainsKey(right) && !trid.ContainsKey(left)){
                        if(!puzzleEdges.bottomLeftEdgeTriangles.Contains(t)){
                            puzzleEdges.bottomLeftEdgeTriangles.Add(t);
                        }
                    }else if(!trid.ContainsKey(right) && trid.ContainsKey(left)){
                        if(!puzzleEdges.bottomRightEdgeTriangles.Contains(t)){
                            puzzleEdges.bottomRightEdgeTriangles.Add(t);
                        }
                    }

                }else{//bottom, top right, top left
                    if(!trid.ContainsKey(negRight) && !trid.ContainsKey(posLeft)){
                        puzzleEdges.bottomEdgeTriangles.Add(t);
                    }else if(trid.ContainsKey(right) && !trid.ContainsKey(left)){
                        if(!puzzleEdges.topLeftEdgeTriangles.Contains(t)){
                            puzzleEdges.topLeftEdgeTriangles.Add(t);
                        }
                    }else if(!trid.ContainsKey(right) && trid.ContainsKey(left)){
                        if(!puzzleEdges.topRightEdgeTriangles.Contains(t)){
                            puzzleEdges.topRightEdgeTriangles.Add(t);
                        }
                    }
                }
            }
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
            filledMesh.material.color = Color.white;
            filledMesh.enabled = true;
            filledMesh.GetComponent<TriangleInteractor>().triangle = t;
        }
    }
}