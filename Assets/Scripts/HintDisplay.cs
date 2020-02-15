using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            // foreach(
        }
        public void Draw(HintItem[] hint, MarchDirections dir, Triangle initialEdge){
            //could this be static? Or manage... all of them as a single manager?

        }
        public void DrawOneHint(int numberFromAxis,int number, Color c, MarchDirections drawDir, Triangle initialEdge)
        {
            
            MeshRenderer filledMesh = GameObject.Instantiate(hintMeshMaker,Vector3.zero,Quaternion.identity,transform).GetComponent<MeshRenderer>();
            Mesh mesh = new Mesh();

            numberFromAxis--;//
            Vector2Int drawPosition = new Triangle(initialEdge.position.x,initialEdge.position.y,initialEdge.edgeLength).position;
            for(int i = 0;i<numberFromAxis;i++)
            {
                //Skip over x times, to get to the right triangle (rhombus) place to darw
                drawPosition = Triangle.March(drawPosition,drawDir);
                drawPosition = Triangle.March(drawPosition,drawDir);
            }
            //NOT TO FUTURE ME, I AM THIS FAR, GOING DOWN THROUGH THE COPY PASTED CODE.

            Vector2[] verts2 = initialEdge.GetVertsInWorldSpace();
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

        public void Reset(){
            foreach(Transform child in transform)
            {
                Destroy(child);
            }
        }

    }
}