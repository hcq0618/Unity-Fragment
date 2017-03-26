// Created by hcq
using UnityEngine;

namespace UnityFragment
{
    [DisallowMultipleComponent]
    public abstract class Fragment : BaseBehaviour
    {
        #region life cycle

        //子类重载 进栈时回调 为了区分OnEnable时不同逻辑
        public virtual void OnEnterStack()
        {
        }

        //子类重载 出栈时回调 为了区分OnDisable时不同逻辑
        public virtual void OnExitStack()
        {
        }

        //子类重载 通过intent启动fragment时回调
        public virtual void OnIntent(string extra)
        {
        }

        #region animation

        //返回动画时长
        protected internal virtual float DoExitAnimation()
        {
            return 0f;
        }

        //返回动画时长
        protected internal virtual float DoEnterAnimation()
        {
            return 0f;
        }

        //按返回键是否执行动画
        protected internal virtual bool IsDoAnimationOnBackPressed()
        {
            return true;
        }

        #endregion

        protected internal virtual void OnDynamicInstantiated()
        {

        }

        #endregion

        #region input event

        //子类重载
        public virtual bool OnBackPressed()
        {
            return false;
        }

        //子类重载
        public virtual bool OnMenuPressed()
        {
            return false;
        }

        //子类重载
        public virtual bool OnHomePressed()
        {
            return false;
        }

        //子类需要处理长按事件时重载
        public virtual bool OnLongBackPressed()
        {
            return false;
        }

        #endregion

    }
}

