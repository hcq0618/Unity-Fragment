// Created by hcq
using UnityEngine;

namespace UnityFragment
{
    public class InputEventHandler
    {
        #region delegate

        public delegate bool OnBackPressedDelegate();

        public delegate bool OnMenuPressedDelegate();

        public delegate bool OnHomePressedDelegate();

        public delegate bool OnLongBackPressedDelegate();

        #endregion

        #region _listener

        OnLongBackPressedDelegate longBackPressListener;
        OnBackPressedDelegate backPressedListener;
        OnMenuPressedDelegate menuPressedListener;
        OnHomePressedDelegate homePressedListener;

        #endregion

        readonly int LONG_PRESS_DURATION = 1;
        // unit is second
        float keydownTime;

        #region set listeners
        public void SetLongBackPressListener(OnLongBackPressedDelegate listener)
        {
            longBackPressListener = listener;
        }

        public void SetBackPressedListener(OnBackPressedDelegate listener)
        {
            backPressedListener = listener;
        }

        public void SetMenuPressedListener(OnMenuPressedDelegate listener)
        {
            menuPressedListener = listener;
        }

        public void SetHomePressedListener(OnHomePressedDelegate listener)
        {
            homePressedListener = listener;
        }

        #endregion

        public void DispatchEvent()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                // 松开返回键
                if (Time.timeSinceLevelLoad - keydownTime >= LONG_PRESS_DURATION)
                {
                    if (longBackPressListener != null)
                    {
                        longBackPressListener();
                    }
                }
                else
                {
                    if (null != backPressedListener)
                    {
                        backPressedListener();
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                //按了返回键
                keydownTime = Time.timeSinceLevelLoad;
            }
            else if (Input.GetKeyDown(KeyCode.Menu))
            {
                //按了菜单键
                if (menuPressedListener != null)
                {
                    menuPressedListener();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Home))
            {
                //按了home键
                if (homePressedListener != null)
                {
                    homePressedListener();
                }
            }
        }
    }
}

