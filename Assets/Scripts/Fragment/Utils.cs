// hcq 2017/3/26
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace UnityFragment
{
    public static class Utils
    {
        public static bool IsCollectionEmpty(ICollection collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static GameObject GetCanvas()
        {
            GameObject canvasGo = GameObject.Find("Canvas");

            if (canvasGo == null)
            {
                GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
                for (int i = 0; i < rootGameObjects.Length; i++)
                {
                    GameObject rootGameObject = rootGameObjects[i];
                    Canvas canvas = rootGameObject.GetComponent<Canvas>();
                    if (canvas != null)
                    {
                        canvasGo = canvas.gameObject;
                        break;
                    }
                }
            }

            return canvasGo;
        }

        public static T GetComponent<T>(GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();

            if (component == null)
            {
                component = obj.AddComponent<T>();
            }

            return component;
        }

        //获取当前时间戳
        public static long GetTimeStamp()
        {
            DateTime now = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now);
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan elapsedTime = now - dtStart;
            return (long)elapsedTime.TotalMilliseconds;
        }
    }
}
