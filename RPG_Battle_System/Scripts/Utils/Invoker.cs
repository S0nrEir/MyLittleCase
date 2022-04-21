
/*
Copyright (c) 2015 Funonium (Jade Skaggs)
	
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public delegate void Invokable();

/// <summary>
/// Enables invokation of functions without regard to timeScale
/// To use this class, Call Invoker.InvokeDelayed(MyFunc, 5);
/// 
/// Written by Jade Skaggs - Funonium.com
/// </summary>
public class Invoker : MonoBehaviour {
	private struct InvokableItem
	{
		public Invokable Func;
		public float ExecuteAtTime;
		public static Invoker Instance = null;
		
		public InvokableItem(Invokable func, float delaySeconds)
		{
			this.Func = func;
			
			// realtimeSinceStartup is never 0, and Time.time is affected by timescale (though it is 0 on startup).  Use a combination 
			// http://forum.unity3d.com/threads/205773-realtimeSinceStartup-is-not-0-in-first-Awake()-call
			if(Time.time == 0) 
				this.ExecuteAtTime = delaySeconds;
			else
				this.ExecuteAtTime = Time.realtimeSinceStartup + delaySeconds;
			
		}
	}
	
	private static Invoker _instance = null;
	public static Invoker Instance
	{
		get
		{
			if( _instance == null )
			{
				GameObject go = new GameObject();
				go.AddComponent<Invoker>();
				go.name = "_FunoniumInvoker";
				_instance = go.GetComponent<Invoker>();
			}
			return _instance;
		}
	}
	
	float fRealTimeLastFrame = 0;
	float fRealDeltaTime = 0;
	
	List<InvokableItem> invokeList = new List<InvokableItem>();
	List<InvokableItem> invokeListPendingAddition = new List<InvokableItem>();
	List<InvokableItem> invokeListExecuted = new List<InvokableItem>();
	
	public float RealDeltaTime()
	{
		return fRealDeltaTime;	
	}
	/// <summary>
	/// Invokes the function with a time delay.  This is NOT 
	/// affected by timeScale like the Invoke function in Unity.
	/// </summary>
	/// <param name='func'>
	/// Function to invoke
	/// </param>
	/// <param name='delaySeconds'>
	/// Delay in seconds.
	/// </param>
	public static void InvokeDelayed(Invokable func, float delaySeconds)
	{
		Instance.invokeListPendingAddition.Add(new InvokableItem(func, delaySeconds));
	}
	
	// must be maanually called from a game controller or something similar every frame
	public void Update()
	{
		fRealDeltaTime = Time.realtimeSinceStartup - fRealTimeLastFrame;
		fRealTimeLastFrame = Time.realtimeSinceStartup;
		
		// Copy pending additions into the list (Pending addition list 
		// is used because some invokes add a recurring invoke, and
		// this would modify the collection in the next loop, 
		// generating errors)
		foreach(InvokableItem item in invokeListPendingAddition)
		{
			invokeList.Add(item);	
		}
		invokeListPendingAddition.Clear();
		
		
		// Invoke all items whose time is up
		foreach(InvokableItem item in invokeList)
		{
			if(item.ExecuteAtTime <= Time.realtimeSinceStartup)	
			{
				if(item.Func != null)
					item.Func();
				
				invokeListExecuted.Add(item);
			}
		}
		
		// Remove invoked items from the list.
		foreach(InvokableItem item in invokeListExecuted)
		{
			invokeList.Remove(item);
		}
		invokeListExecuted.Clear();
	}
}