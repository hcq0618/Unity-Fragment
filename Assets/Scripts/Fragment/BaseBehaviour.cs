// hcq 2017/3/26
using UnityEngine;

namespace UnityFragment
{
    public class BaseBehaviour : MonoBehaviour
    {
        FragmentRayCastManager raycastManager;
        GameObject canvas;

        protected internal virtual GameObject GetCanvas()
        {
            if (canvas == null)
            {
                canvas = Utils.GetCanvas();
            }

            return canvas;
        }

        protected internal FragmentRayCastManager GetRayCastManager()
        {
            if (raycastManager == null)
            {
                raycastManager = FragmentRayCastManager.GetInstance(GetCanvas());
            }

            return raycastManager;
        }

    }
}
