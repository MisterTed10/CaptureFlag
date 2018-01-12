using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Core
{
    public class CharacterDistanceComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (((CharacterDistance)x).distance < ((CharacterDistance)y).distance)
                return -1;  // compare user names
            if (((CharacterDistance)x).distance > ((CharacterDistance)y).distance)
                return 1;  // compare user names
            return 0;
        }



    }

    public struct CharacterDistance : System.IComparable
    {
        public float distance;
        public Character character;

        public int CompareTo(object obj)
        {
            if (((CharacterDistance)this).distance < ((CharacterDistance)obj).distance)
                return -1;  // compare user names
            if (((CharacterDistance)this).distance > ((CharacterDistance)obj).distance)
                return 1;  // compare user names
            return 0;
        }
    };
}