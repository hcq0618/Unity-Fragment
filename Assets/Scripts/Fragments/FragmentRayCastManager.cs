// hcq 2017/1/16
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Fragments
{
    [DisallowMultipleComponent]
    public class FragmentRayCastManager : BaseBehaviour
    {
        public bool isBlocking { get { return !Utils.IsCollectionEmpty(blockingCoroutines); } }

        // 用来屏蔽全部事件, 挂载到 Canvas 上
        CanvasGroup blockRaycastsCanvasGroup;
        readonly Dictionary<long, Coroutine> blockingCoroutines = new Dictionary<long, Coroutine>();

        public static FragmentRayCastManager GetInstance()
        {
            return GetInstance(Utils.GetCanvas());
        }

        public static FragmentRayCastManager GetInstance(GameObject canvas)
        {
            return Utils.GetComponent<FragmentRayCastManager>(canvas);
        }

        //动画期间需要屏蔽焦点事件 防止用户误点
        public void Block(float blockDurationForSeconds, Action onBlockEnd = null)
        {
            if (gameObject.activeInHierarchy)
            {
                long timeStamp = Utils.GetTimeStamp();

                Coroutine blockRaycastsCoroutine = StartCoroutine(BlockCoroutine(blockDurationForSeconds, onBlockEnd, timeStamp));
                blockingCoroutines.Add(timeStamp, blockRaycastsCoroutine);
            }
        }

        IEnumerator BlockCoroutine(float blockDurationForSeconds, Action onBlockEnd, long timeStamp)
        {
            // init fullScreenCanvasGroup
            if (blockRaycastsCanvasGroup == null)
            {
                GameObject canvas = GetCanvas();
                if (canvas != null)
                {
                    blockRaycastsCanvasGroup = canvas.GetComponent<CanvasGroup>();
                    if (blockRaycastsCanvasGroup == null)
                    {
                        blockRaycastsCanvasGroup = canvas.AddComponent<CanvasGroup>();
                    }
                }
            }

            // blockRaycastsCanvasGroup 仍然有可能为空, 比如界面的 Canvas 不存在, 被更名 等
            if (blockRaycastsCanvasGroup != null)
            {
                blockRaycastsCanvasGroup.blocksRaycasts = false;
            }

            //异步等待
            yield return new WaitForSeconds(blockDurationForSeconds);

            if (blockRaycastsCanvasGroup != null)
            {
                blockRaycastsCanvasGroup.blocksRaycasts = true;
            }

            if (blockingCoroutines.ContainsKey(timeStamp))
            {
                blockingCoroutines.Remove(timeStamp);
            }

            if (onBlockEnd != null)
            {
                onBlockEnd();
            }
        }

        protected void OnDisable()
        {
            if (!Utils.IsCollectionEmpty(blockingCoroutines))
            {
                foreach (Coroutine co in blockingCoroutines.Values)
                {
                    StopCoroutine(co);
                }

                blockingCoroutines.Clear();

            }

            //恢复状态
            if (blockRaycastsCanvasGroup != null)
            {
                blockRaycastsCanvasGroup.blocksRaycasts = true;
            }

        }
    }
}
