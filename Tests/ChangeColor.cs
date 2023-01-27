using UnityEngine;
using RuntimeDebugger.Commands;
namespace RuntimeDebugger.Demo
{
    public class ChangeColor : MonoBehaviour
    {
        Color _randomColor;
        static readonly int shPropertyColor = Shader.PropertyToID("_Color");
        MaterialPropertyBlock propertyBlock;
        MeshRenderer meshRenderer;

        public MaterialPropertyBlock PropertyBlock
        {
            get
            {
                if (propertyBlock == null)
                    propertyBlock = new MaterialPropertyBlock();
                return propertyBlock;
            }
        }

        void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        [AddCommand("changecolor", "changes to random color")]
        void ChangeMeshColor()
        {
            _randomColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            PropertyBlock.SetColor(shPropertyColor, _randomColor);
            meshRenderer.SetPropertyBlock(PropertyBlock);
        }

        void ChangeColorTo(float r, float g, float b)
        {
            PropertyBlock.SetColor(shPropertyColor, new Color(r,g,b,1));
            meshRenderer.SetPropertyBlock(PropertyBlock);
        }
    }
}
