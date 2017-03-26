// hcq 2016/12/7
using System;
using UnityEngine;

namespace Fragments
{
    [DisallowMultipleComponent]
    public class FragmentAnimator : BaseBehaviour, IFragmentAnimator
    {
        //应用退出动画
        public void DoApplicationQuitAnimation(Action onAnimationEnd = null)
        {
        }

        //fragment切换动画
        public void DoAnimation(Fragment enterFragment, Fragment exitFragment, Action onAnimationEnd = null)
        {
            float exitDuration = 0, enterDuration = 0;

            if (exitFragment != null)
            {
                exitDuration = exitFragment.DoExitAnimation();
            }
            if (enterFragment != null)
            {
                enterDuration = enterFragment.DoEnterAnimation();
            }

            float animationDuration = Math.Max(exitDuration, enterDuration);

            if (animationDuration > 0)
            {
                GetRayCastManager().Block(animationDuration, () =>
                {
                    if (onAnimationEnd != null)
                    {
                        onAnimationEnd();
                    }
                });
            }
            else
            {
                if (onAnimationEnd != null)
                {
                    onAnimationEnd();
                }
            }
        }
    }
}
