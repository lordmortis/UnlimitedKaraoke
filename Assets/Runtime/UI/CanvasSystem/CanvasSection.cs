using Mono.CompilerServices.SymbolWriter;
using UnityEngine;

namespace UnlimitedKaraoke.Runtime.UI.CanvasSystem
{
    [CreateAssetMenu(fileName = "NewCanvas", menuName = "UI/Canvas Section", order = 0)]
    public class CanvasSection : ScriptableObject
    {
        public string Name => name;
        [SerializeField] private string name;
    }
}