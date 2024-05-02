using UnityEngine;

namespace SpacemarketSimulator
{
    public class TransformDefiner : MonoBehaviour
    {
        public Vector3 position;
        public Vector3 localPosition;
        public Vector3 localScale;
        public Vector3 eulerAngles;

        public void Update()
        {
            transform.localPosition = localPosition;
            transform.localPosition = position + Plugin.Player.transform.position;
            transform.localScale = localScale;
            transform.eulerAngles = eulerAngles;
        }
        public static TransformDefiner AddToGameObject(GameObject go, Vector3 position, Vector3 localPosition, Vector3 localScale, Vector3 eulerAngles)
        {
            var td = go.AddComponent<TransformDefiner>();
            td.position = position;
            td.localPosition = localPosition;
            td.localScale = localScale;
            td.eulerAngles = eulerAngles;
            return td;
        }
        public static TransformDefiner AddToGameObject(GameObject go)
        {
            var td = go.AddComponent<TransformDefiner>();
            td.position = go.transform.position;
            td.localPosition = go.transform.localPosition;
            td.localScale = go.transform.localScale;
            td.eulerAngles = go.transform.eulerAngles;
            return td;
        }
    }
}
