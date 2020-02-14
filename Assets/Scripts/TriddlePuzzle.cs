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
        negativeSlope_left,
        none//should never happen, but keeps us from having to deal with nulls on a non-nullable type. 
    }
    public struct HintItem{
        public HintItem(int a,int c){q = a;color = c;}
        public int q;
        public int color;
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

        public HintItem[][] t_solution_hint;

        public HintItem[] GetHintFromSolution(int[] solution){
            //give a solution, return the appropriatley concatenated hint, as a string. So 1,1,0,0,0,3,3,3 becomes 2,2.
            //For now, lets consider monochromatic hints. Otherwise some non-int form may be needed, like a bit shift trick or just a fucking string lol.
            int prev = 0;
            List<HintItem> hint = new List<HintItem>();
            //make a list of hintItems with each color and Amount.
            for(int i = 0;i<solution.Length;i++){
                int j = solution[i];
                if(j != prev){//
                    hint.Add(new HintItem(1,j));//start with one.
                }else{
                    //all this does is increase q(uantity) by one.
                    hint[hint.Count-1] = new HintItem(hint[hint.Count-1].q+1,hint[hint.Count-1].color);
                }
                prev = i;
            }
            //But get rid of the ones that aren't actually colors. (color, state, same thing basically.)
            for(int i = hint.Count-1;i>0;i--){
                if(hint[i].color == 0){
                    hint.RemoveAt(i);
                }
            }
            return hint.ToArray();
        }
        
        public static MarchDirections OppositeMarchDirection(MarchDirections indir)
        {
                if(indir == MarchDirections.horizontal_left){return MarchDirections.horizontal_right;}
            else if(indir == MarchDirections.horizontal_right){return MarchDirections.horizontal_left;}
            else if(indir == MarchDirections.negativeSlope_left){return MarchDirections.negativeSlope_right;}
            else if(indir == MarchDirections.negativeSlope_right){return MarchDirections.negativeSlope_left;}
            else if(indir == MarchDirections.positiveSlope_left){return MarchDirections.positiveSlope_right;}
            else if(indir == MarchDirections.positiveSlope_right){return MarchDirections.positiveSlope_left;}
            else{return MarchDirections.none;}
        }
    }
}