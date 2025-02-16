using System.Collections;
using UnityEngine;

namespace RobbieWagnerGames.Zombinos
{
    public class SequenceEvent : MonoBehaviour
    {
        public virtual IEnumerator InvokeSequenceEvent()
        {
            yield return null;
        }
    }
}