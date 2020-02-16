using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Blooper.Triangles{

    public struct Hint{
        public HintItem[] hintItems;
        public MarchDirections solutionDirection;
        public Triangle edgeTriangle;
    }
    //Is given a new HintItem[] and draws it to the scene.
    //Draws the specific hints as children gameObjects.
    public class HintDisplay : MonoBehaviour
    {
        public GameObject hintMeshMaker;
        public void DrawPuzzleHint(PuzzleEdges puzzleEdges){
            foreach(Triangle t in puzzleEdges.edgeTriangleToSolutionMap.Keys){
                int[] solution = puzzleEdges.edgeTriangleToSolutionMap[t];
                HintItem[] hint = TriddlePuzzle.GetHintFromSolution(solution);
                Draw(hint,PuzzleEdges.EdgeToSolutionDir(puzzleEdges.EdgeTriangleIsOn(t)),t);
            }
        }
        
        public void Draw(HintItem[] hint, MarchDirections dir, Triangle initialEdge){
            for(int i = 1;i<=hint.Length;i++){
                //color from int to color? 
                //Where do we store the palette?
                //Debug.Log("Drawing a single hint set");
                DrawOneHint(i,hint[i-1].q,hint[i-1].color,Triangle.GetHintDrawDir(dir),initialEdge);
            }

        }
        public void DrawOneHint(int numberFromAxis,int number, Color c, MarchDirections drawDir, Triangle initialEdge)
        {
            
            MeshRenderer filledMesh = GameObject.Instantiate(hintMeshMaker,Vector3.zero,Quaternion.identity,transform).GetComponent<MeshRenderer>();
            Mesh mesh = new Mesh();

            numberFromAxis--;//
            Vector2Int drawPosition = new Triangle(initialEdge.position.x,initialEdge.position.y,initialEdge.edgeLength).position;
            drawPosition = Triangle.March(drawPosition,drawDir);
            for(int i = 0;i<numberFromAxis;i++)
            {
                //Skip over x times, to get to the right triangle (rhombus) place to darw
                drawPosition = Triangle.March(drawPosition,drawDir);
                drawPosition = Triangle.March(drawPosition,drawDir);
            }

            Vector2Int drawPosition2 = Triangle.March(drawPosition,drawDir);

            //A rhombus is made of two triangles that share  an edge. These are those two triangles.
            Triangle rh1 = new Triangle(drawPosition);
            Triangle rh2 = new Triangle(drawPosition2);

            //Get verts of our triangles.
            Vector2[] vertsrh1 = rh1.GetVertsInWorldSpace();
            Vector2[] vertsrh2 = rh2.GetVertsInWorldSpace();


            //Ah, a nice neat array of our verts.
            Vector2[] verts2 = new Vector2[vertsrh1.Length+vertsrh2.Length];
            vertsrh1.CopyTo(verts2,0);
            vertsrh2.CopyTo(verts2,vertsrh1.Length);

            //Great! Verts 2 is our nice neat array of two triangles now.
            ///It has duplicate vertices buuuuut is that actually a problem?

            Vector2[] verts2Local = new Vector2[verts2.Length];//4
            Vector3[] verts3 = new Vector3[verts2.Length];//4

            Vector3 offset = Vector3.Lerp(rh1.GetCentroidInWorldSpace(),rh2.GetCentroidInWorldSpace(),0.5f);

            for(int i = 0;i<verts2.Length;i++)
            {
                verts2Local[i] = verts2[i] - (Vector2)offset;//same but Vector2 cus why bother casting a whole array later for the polygon collider2d.
                verts3[i] = (Vector3)verts2[i]-offset;//offset by vertex 0...
                
            }
            
            int[] tris = new int[]{2,1,0,5,4,3};//simple enough. 
        
            mesh.vertices = verts3;
            mesh.triangles = tris;
            //
            filledMesh.transform.position = transform.position+offset;//un-offset. By this offset so the position is the centroid, and by the parent, for moving the grid around. Now our triangles are positioned sensibly. 
            
            //and we can do sorting/filtering by world positions or transforms, which is easier than just naked data because unity has a lot of features for it
            //already built it. I THINK.
            //
            filledMesh.GetComponent<MeshFilter>().mesh = mesh;
            
            filledMesh.material.color = c;
            filledMesh.enabled = true;
            //Set our text to be the right hint.
            filledMesh.GetComponentInChildren<TMPro.TextMeshPro>().text = number.ToString();
            //Center the textMesh
            //offset set in the chid object of prefab. IDK, its fine.
            // filledMesh.transform.GetChild(0).transform.position = filledMesh.transform.position;
        }

        public void Reset(){
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

    }
}