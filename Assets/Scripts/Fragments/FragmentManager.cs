// Created by hcq
//这个类中的函数保证一个页面中只会被调一次 所以应该与根物体绑定
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Fragments
{
    [DisallowMultipleComponent]
    public sealed class FragmentManager : BaseBehaviour
    {
        const string UNINSTANTIATE_FRAGMENT_TAG = "UnInstantiateFragment";

        public delegate bool OnBeforeApplicationQuitDelegate(Action ApplicationQuitAction);

        //动态加载的fragment prefab
        public GameObject[] dynamicFragmentPrefabs;

        public IFragmentAnimator fragmentAnimator;

        //fragment显示栈
        readonly List<Fragment> fragmentStack = new List<Fragment>();
        //所有的fragment
        List<Fragment> allFragments;

        InputEventHandler inputEventHandler;

        //应用退出前的回调 可以用于满足应用退出时的特殊需求
        OnBeforeApplicationQuitDelegate OnBeforeApplicationQuit;

        #region 生命周期

        public void Awake()
        {
            //已经挂在scene中的fragment
            Fragment[] fragmentsInScene = GetComponentsInChildren<Fragment>(true);

            FillAllFragments(fragmentsInScene);
            FirstFragmentEnterStack(fragmentsInScene);

            InitInputEvent();

        }

        public void Update()
        {
            inputEventHandler.DispatchEvent();
        }

        public void OnDestroy()
        {
            ClearFragments();
        }

        #endregion

        public static FragmentManager GetInstance(GameObject canvas)
        {
            return Utils.GetComponent<FragmentManager>(canvas);
        }

        public static FragmentManager GetInstance()
        {
            return GetInstance(Utils.GetCanvas());
        }

        //初始化触控
        void InitInputEvent()
        {
            inputEventHandler = new InputEventHandler();
            inputEventHandler.SetBackPressedListener(HandleBackPressed);
            inputEventHandler.SetMenuPressedListener(HandleMenuPressed);
            inputEventHandler.SetHomePressedListener(HandleHomePressed);
            inputEventHandler.SetLongBackPressListener(HandleLongBackPress);
        }

        void FillAllFragments(Fragment[] fragmentsInScene)
        {
            allFragments = new List<Fragment>(fragmentsInScene);

            if (!Utils.IsCollectionEmpty(dynamicFragmentPrefabs))
            {
                for (int i = 0; i < dynamicFragmentPrefabs.Length; i++)
                {
                    GameObject prefab = dynamicFragmentPrefabs[i];
                    Fragment fragment = prefab.GetComponent<Fragment>();
                    if (fragment != null)
                    {
                        if (!allFragments.Contains(fragment))
                        {
                            fragment.tag = UNINSTANTIATE_FRAGMENT_TAG;
                            allFragments.Add(fragment);
                        }
                    }
                }
            }

            //Debug.Log("hcq fragment " + allFragments.Count);
            //foreach (Fragment fragment in allFragments)
            //{
            //    Debug.Log("hcq fragment " + fragment.name);
            //}
        }

        //把第一个界面放入栈里
        void FirstFragmentEnterStack(Fragment[] fragmentsInScene)
        {
            for (int i = 0; i < fragmentsInScene.Length; i++)
            {
                Fragment fragment = fragmentsInScene[i];
                //找第一个物体可见的fragment
                if (fragment.gameObject.activeSelf && fragment.enabled)
                {
                    //没有指定某个需要启动的fragment
                    FragmentEnterStack(fragment);
                    return;
                }
            }
        }

        void FragmentEnterStack(Fragment fragment)
        {
            fragmentStack.Add(fragment);
            fragment.OnEnterStack();
        }

        void FragmentExitStack(Fragment deleteFragment, bool isCallBack = true)
        {
            //从后面删 避免存在相同引用的fragment删除了前面的
            for (int i = fragmentStack.Count - 1; i >= 0; i--)
            {
                Fragment fragment = fragmentStack[i];
                if (fragment.Equals(deleteFragment))
                {
                    fragmentStack.RemoveAt(i);
                    if (isCallBack)
                    {
                        fragment.OnExitStack();
                    }
                    break;
                }
            }
        }

        void ClearTopFragments(Fragment moveToTopFragment)
        {
            for (int i = fragmentStack.Count - 1; i >= 0; i--)
            {
                Fragment fragment = fragmentStack[i];
                if (!fragment.Equals(moveToTopFragment))
                {
                    fragmentStack.RemoveAt(i);
                    fragment.OnExitStack();
                }
                else
                {
                    break;
                }
            }
        }

        void ClearFragments()
        {
            //在foreach循环中修改元素会抛异常 所以必须用for循环 但正序for循环会导致只删除一般元素  所以这里必须用倒序循环
            for (int i = fragmentStack.Count - 1; i >= 0; i--)
            {
                Fragment fragment = fragmentStack[i];
                fragmentStack.RemoveAt(i);
                fragment.OnExitStack();
            }
        }

        public Fragment GetTopFragment()
        {
            //foreach (Fragment fragment in fragmentStack)
            //{
            //    Debug.Log("hcq fragmentStack " + fragment.name);
            //}
            //Debug.Log("hcq fragmentStack end -------");
            return !Utils.IsCollectionEmpty(fragmentStack) ? fragmentStack[fragmentStack.Count - 1] : null;
        }

        public Fragment GetPreviousFragment()
        {
            Fragment preFragment = null;
            int preIndex = fragmentStack.Count - 2;
            if (preIndex >= 0)
            {
                preFragment = fragmentStack[preIndex];
            }

            return preFragment;
        }

        public int GetFragmentCountInStack()
        {
            return Utils.IsCollectionEmpty(fragmentStack) ? 0 : fragmentStack.Count;
        }

        #region 处理按键操作

        public bool HandleBackPressed()
        {
            Fragment activeFragment = GetTopFragment();
            bool isConsumed = activeFragment != null && activeFragment.OnBackPressed();

            if (!isConsumed)
            {
                Fragment previousFragment = GetPreviousFragment();

                if (previousFragment == null)
                {
                    //应用退出
                    ApplicationQuit(activeFragment);
                }
                else
                {
                    //fragment返回
                    Hide(activeFragment.IsDoAnimationOnBackPressed());
                }
            }

            return isConsumed;
        }

        public FragmentManager SetOnBeforeApplicationQuit(OnBeforeApplicationQuitDelegate callback)
        {
            OnBeforeApplicationQuit = callback;
            return this;
        }

        void ApplicationQuit(Fragment activeFragment)
        {
            Action ApplicationQuitAction = delegate
            {
                if (activeFragment != null)
                {
                    activeFragment.isPendingApplicationQuit = true;
                }

                GetFragmentAnimator().DoApplicationQuitAnimation(Application.Quit);
            };

            bool isConsumed = false;

            if (OnBeforeApplicationQuit != null)
            {
                isConsumed = OnBeforeApplicationQuit(ApplicationQuitAction);
            }

            if (!isConsumed)
            {
                ApplicationQuitAction();
            }
        }

        bool HandleLongBackPress()
        {
            Fragment activeFragment = GetTopFragment();
            return activeFragment != null && activeFragment.OnLongBackPressed();
        }

        bool HandleMenuPressed()
        {
            Fragment activeFragment = GetTopFragment();
            return activeFragment != null && activeFragment.OnMenuPressed();

        }

        bool HandleHomePressed()
        {
            Fragment activeFragment = GetTopFragment();
            return activeFragment != null && activeFragment.OnHomePressed();

        }

        #endregion

        public Fragment FindFragmentByName(string gameObjectName)
        {
            for (int i = 0; i < allFragments.Count; i++)
            {
                Fragment fragment = allFragments[i];
                if (string.Equals(fragment.gameObject.name, gameObjectName))
                {
                    return fragment;
                }
            }

            return null;
        }

        #region 显示

        public void Show(ref Fragment nextFragment)
        {
            Show(ref nextFragment, true);
        }

        public void Show(ref Fragment nextFragment, bool isDoAnimation)
        {
            FragmentIntent intent = new FragmentIntent();
            intent.isDoAnimation = isDoAnimation;

            Show(ref nextFragment, intent);
        }

        public void Show(ref Fragment nextFragment, FragmentIntent intent)
        {
            Fragment activeFragment = GetTopFragment();

            //先要显示下一个 让界面加载出来 再显示动画
            switch (intent.launchMode)
            {
                case FragmentIntent.FLAG_CLEAR_TOP:

                    bool isInStack = fragmentStack.Contains(nextFragment);
                    if (isInStack)
                    {
                        ClearTopFragments(nextFragment);
                    }

                    //不在显示栈中要进栈
                    ShowSpecificFragment(ref nextFragment, !isInStack);

                    break;
                case FragmentIntent.FLAG_NEW_INSTANCE:

                    ShowSpecificFragment(ref nextFragment, true);

                    break;
                case FragmentIntent.FLAG_SINGLE_INSTANCE:

                    isInStack = fragmentStack.Contains(nextFragment);
                    if (isInStack)
                    {
                        FragmentExitStack(nextFragment, false);
                    }

                    ShowSpecificFragment(ref nextFragment, true);

                    break;
            }

            nextFragment.OnIntent(intent.extra);

            if (intent.isDoAnimation)
            {
                GetFragmentAnimator().DoAnimation(nextFragment, activeFragment, () =>
                {
                    HideSpecificFragment(activeFragment, false);
                });
            }
            else
            {
                HideSpecificFragment(activeFragment, false);
            }
        }

        //显示某一个
        void ShowSpecificFragment(ref Fragment specificFragment, bool addToStack)
        {
            if (specificFragment != null)
            {
                if (specificFragment.transform.parent == null)
                {
                    if (specificFragment.CompareTag(UNINSTANTIATE_FRAGMENT_TAG))
                    {
                        InstantiateFragment(ref specificFragment);
                    }

                    specificFragment.transform.SetParent(transform, false);
                }

                if (addToStack)
                {
                    FragmentEnterStack(specificFragment);
                }

                if (!specificFragment.transform.parent.gameObject.activeInHierarchy)
                {
                    specificFragment.transform.parent.gameObject.SetActive(true);
                }

                if (!specificFragment.gameObject.activeInHierarchy)
                {
                    specificFragment.gameObject.SetActive(true);
                }

                if (!specificFragment.isActiveAndEnabled)
                {
                    specificFragment.enabled = true;
                }
            }
        }

        void InstantiateFragment(ref Fragment specificFragment)
        {
            GameObject obj = Instantiate(specificFragment.gameObject);
            obj.name = specificFragment.gameObject.name;

            //销毁prefab
            if (allFragments.Contains(specificFragment))
            {
                allFragments.Remove(specificFragment);
            }
            Destroy(specificFragment.gameObject);

            //实例化
            specificFragment = obj.GetComponent<Fragment>();
            specificFragment.tag = "Untagged";//unity默认自带的tag
            if (!allFragments.Contains(specificFragment))
            {
                allFragments.Add(specificFragment);
            }

            Fragment[] childFragments = specificFragment.GetComponentsInChildren<Fragment>(true);
            for (int i = 0; i < childFragments.Length; i++)
            {
                Fragment childFragment = childFragments[i];
                if (!allFragments.Contains(childFragment))
                {
                    allFragments.Add(childFragment);
                }
            }

            specificFragment.OnDynamicInstantiated();

            //Debug.Log("hcq InstantiateSpecificFragment " + allFragments.Count);
            //foreach (Fragment fragment in allFragments)
            //{
            //    Debug.Log("hcq InstantiateSpecificFragment " + fragment.name);
            //}
        }

        public Fragment Show(string gameObjectName)
        {
            return Show(gameObjectName, true);
        }

        public Fragment Show(string gameObjectName, bool isDoAnimation)
        {
            FragmentIntent intent = new FragmentIntent();
            intent.isDoAnimation = isDoAnimation;

            return Show(gameObjectName, intent);
        }

        public Fragment Show(string gameObjectName, FragmentIntent intent)
        {
            Fragment nextFragment = FindFragmentByName(gameObjectName);
            Show(ref nextFragment, intent);

            return nextFragment;
        }

        #endregion

        #region 隐藏

        public void Hide(Fragment specificFragment)
        {
            Hide(specificFragment, true);
        }

        public void Hide(Fragment specificFragment, bool isDoAnimation)
        {
            Fragment previousFragment = GetPreviousFragment();

            //先要显示上一个 让界面加载出来 再显示动画
            ShowSpecificFragment(ref previousFragment, false);

            if (isDoAnimation)
            {
                GetFragmentAnimator().DoAnimation(previousFragment, specificFragment, () =>
                {
                    HideSpecificFragment(specificFragment, true);
                });
            }
            else
            {
                HideSpecificFragment(specificFragment, true);
            }

        }

        //隐藏某一个
        void HideSpecificFragment(Fragment specificFragment, bool removeFromStack)
        {
            if (specificFragment != null)
            {
                if (specificFragment.gameObject.activeInHierarchy)
                {
                    specificFragment.gameObject.SetActive(false);
                }

                if (specificFragment.isActiveAndEnabled)
                {
                    specificFragment.enabled = false;
                }

                if (removeFromStack)
                {
                    FragmentExitStack(specificFragment);
                }
            }
        }

        public Fragment Hide(string gameObjectName)
        {
            return Hide(gameObjectName, true);
        }

        public Fragment Hide(string gameObjectName, bool isDoAnimation)
        {
            Fragment oldFragment = FindFragmentByName(gameObjectName);
            Hide(oldFragment, isDoAnimation);

            return oldFragment;
        }

        public Fragment Hide()
        {
            return Hide(true);
        }

        public Fragment Hide(bool isDoAnimation)
        {
            //隐藏当前的
            Fragment activeFragment = GetTopFragment();
            Hide(activeFragment, isDoAnimation);

            return activeFragment;
        }

        #endregion

        public IFragmentAnimator GetFragmentAnimator()
        {
            if (fragmentAnimator == null)
            {
                GameObject canvas = GetCanvas();
                fragmentAnimator = canvas.GetComponent<FragmentAnimator>();

                if (fragmentAnimator == null)
                {
                    fragmentAnimator = canvas.AddComponent<FragmentAnimator>();
                }
            }

            return fragmentAnimator;
        }

    }
}

namespace Fragments
{
    public class FragmentIntent
    {
        public const int FLAG_NEW_INSTANCE = 1;
        public const int FLAG_CLEAR_TOP = 2;
        public const int FLAG_SINGLE_INSTANCE = 3;

        public string fragmentName;
        public string extra;

        public int launchMode = FLAG_NEW_INSTANCE;
        public bool isDoAnimation;
    }
}