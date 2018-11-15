# Unity-Fragment
用于UGUI界面间的跳转，类似Android中的Fragment
Use for switching UGUI pages, just like Fragment in Android.

build in Unity 5.3.4f



## 用法
1.FragmentManager挂在Canvas上

  FragmentManager script bind on your Canvas.

2.每个界面的根物体上挂继承Fragment的实现脚本

  The root object's script in every page should implement Fragment script.



## Fragment
**Fragment中包含生命周期的回调：**

**OnEnterStack：**

fragment进栈时回调, 可以用于实现与OnEnable不同的逻辑

The method will callback when a fragment enter the stack, this use for implement logic that different with OnEnable method.

**OnExitStack：**

fragment出栈时回调, 可以用于实现与OnDisable不同的逻辑

The method will callback when a fragment exit the stack, this use for implement logic that different with OnDisable method.

**OnIntent：**

通过FragmentIntent启动fragment时回调, 可用于界面跳转时传些数据

The method will callback when start a fragment by FragmentIntent, this use for pass data when pages switch.

**DoExitAnimation：**

可以在这里实现界面退出动画

You can override this method to implement animation when a fragment exit.

**DoEnterAnimation：**

可以在这里实现界面进入动画

You can override this method to implement animation when a fragment enter.

**OnDynamicInstantiated：**

fragment动态加载时回调

The method will callback when dynamic loading a fragment.


**Fragment中包含按键时的回调**

**OnBackPressed：**

按返回键时回调

The method will callback when Back key pressed.

**OnMenuPressed：**

按菜单键时回调

The method will callback when Menu key pressed.

**OnHomePressed：**

按Home键时回调

The method will callback when Home key pressed.

**OnLongBackPressed：**

长按返回键时回调

The method will callback when Back key long pressed.


## FragmentManager

**支持两种形式的Fragment**

**静态Fragment：**

在Scene中已经存在的物体上继承Fragment的实现脚本，第一个可见物体上的Fragment被认为是默认显示的界面。

Static Fragment: A script implement Fragment script on a exist object in a scene, and the default page which the first visible fragment will show.


**动态Fragment：**

在编辑器中的FragmentManager脚本上配置界面的任意个Prefab，每个Prefab认为是一个界面，初始化时需要手动显示默认显示的界面。需要在TagManager中添加名为“UnInstantiateFragment”的tag

Dynamic Fragment: The FragmentManager in the editor should setup page's any Prefab, every Prefab is know as a page,
The default display page needs to be manually displayed during initialization.
A tag called "UnInstantiateFragment" needs to be added to the TagManager.



**支持多种Fragment启动模式**

效果与安卓中activity启动模式一样

The effect is the same as in android's activity startup mode

**FLAG_NEW_INSTANCE：**

以一个新的Fragment实例启动

Start with a new Fragment instance

**FLAG_CLEAR_TOP：**

若栈中存在该Fragment实例, 则将栈顶的其他Fragment实例都清空, 并将该Fragment实例置顶并显示

If the Fragment instance exists on the stack, then all the other Fragment instances on the top of the stack are cleared and the Fragment instance is placed on the top and displayed.

**FLAG_SINGLE_INSTANCE：**

若栈中存在该Fragment实例, 则将该Fragment实例置顶并显示

If the Fragment instance exists on the stack, the Fragment instance is placed on top and displayed.


## License

MIT License

Copyright (c) 2017 Hcq

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
