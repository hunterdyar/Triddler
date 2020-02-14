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
        public void DrawPuzzleHint(PuzzleEdges puzzleEdges){
            // foreach(
        }
        public void Draw(HintItem[] hint, MarchDirections dir, Triangle initialEdge){
            //could this be static? Or manage... all of them as a single manager?

        }
        public void DrawOneHint(int numberFromAxis,int number, Color c, MarchDirections drawDir, Triangle initialEdge)
        {
            //make a rhombus!
            //It should be numberFromAxis units 
        }

        public void Reset(){
            foreach(Transform child in transform)
            {
                Destroy(child);
            }
        }

    }
}