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

        [Multiline]
        public string levelAsTextData; 
        public int[][] t_solution;
        public int[][] tl_solution;
        public int[][] tr_solution;
        public int[][] b_solution;
        public int[][] bl_solution;
        public int[][] br_solution;


        
        public static HintItem[] GetHintFromSolution(int[] solution){
            //give a solution, return the appropriatley concatenated hint. So 1,1,0,0,0,3,3,3 becomes 2,2.
            //For now, lets consider monochromatic hints. Otherwise some non-int form may be needed, like a bit shift trick or just a fucking string lol.
            int prev = 0;
            List<HintItem> hint = new List<HintItem>();
            //make a list of hintItems with each color and Amount.
            for(int i = 0;i<solution.Length;i++){
                int j = solution[i];
                if(j != prev){//
                    hint.Add(new HintItem(1,j));//start with one.
                }else{
                    if(hint.Count>0){
                        //all this does is increase q(uantity) by one.
                        hint[hint.Count-1] = new HintItem(hint[hint.Count-1].q+1,hint[hint.Count-1].color);
                    }
                }
                prev = j;
            }
            //But get rid of the ones that aren't actually colors. (color, state, same thing basically.)
            for(int i = hint.Count-1;i>0;i--){
                if(hint[i].color == 0){
                    hint.RemoveAt(i);
                }
            }
            return hint.ToArray();
        }
        public void SetLevelAsTextFromLevel()
        {
            levelAsTextData = "";
            levelAsTextData = levelAsTextData+tridSize._top.ToString()+",";
            levelAsTextData = levelAsTextData+tridSize._topRight.ToString()+",";
            levelAsTextData = levelAsTextData+tridSize._topLeft.ToString()+",";
            levelAsTextData = levelAsTextData+tridSize._bottomLeft.ToString()+",";
            levelAsTextData = levelAsTextData+tridSize.colors.ToString()+",";
            //0-4
            //5+
            foreach(KeyValuePair<Vector2Int,int> kvp in level)
            {
                levelAsTextData = levelAsTextData+kvp.Key.x.ToString()+","+kvp.Key.y.ToString()+","+kvp.Value.ToString()+",";
            }
        }
        public void SetLevelFromLevelAsText()
        {
            level = new Dictionary<Vector2Int, int>();
            string[] lar= levelAsTextData.Split(',');
            tridSize = new TridSize(int.Parse(lar[0]),int.Parse(lar[1]),int.Parse(lar[2]),int.Parse(lar[3]),int.Parse(lar[4]));
            for(int i = 5;i<lar.Length;i = i+3)
            {
                if(lar[i] != "" && lar[i+1] != "" && lar[i+2] != ""){
                    Vector2Int p = new Vector2Int(int.Parse(lar[i]),int.Parse(lar[i+1]));
                    level[p] = int.Parse(lar[i+2]);
                }
            }
        }
    }

}