// hcq 2017/3/26
using System;

namespace Fragments
{
    public interface IFragmentAnimator
    {
        void DoApplicationQuitAnimation(Action onAnimationEnd = null);

        void DoAnimation(Fragment enterFragment, Fragment exitFragment, Action onAnimationEnd = null);
    }
}
