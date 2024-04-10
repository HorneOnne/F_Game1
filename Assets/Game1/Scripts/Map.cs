using UnityEngine;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    public List<Transform> Waypoints;
    public List<int> GreenPointIndices;
    public List<int> YellowPointIndices;
    public List<int> RedPointIndices;
    public List<int> PurplePointIndices;

    public Effect GetEffect(int index)
    {
        for(int i = 0; i <GreenPointIndices.Count; i++)
        {
            if (GreenPointIndices[i] == index)
            {
                return Effect.OpportunityReroll;
            }
        }
        for (int i = 0; i < YellowPointIndices.Count; i++)
        {
            if (YellowPointIndices[i] == index)
            {
                return Effect.SkipTurn;
            }
        }


        for (int i = 0; i < RedPointIndices.Count; i++)
        {
            if (RedPointIndices[i] == index)
            {
                return Effect.ReturnStartPoint;
            }
        }

        for (int i = 0; i < PurplePointIndices.Count; i++)
        {
            if (PurplePointIndices[i] == index)
            {
                return Effect.BonusGame;
            }
        }


        return Effect.Default;
    }

}
