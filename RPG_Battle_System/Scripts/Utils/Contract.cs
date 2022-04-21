// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-06-2016
// ***********************************************************************
// <copyright file="Contract.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Contract class
/// </summary>
internal static class Contract
	{
    /// <summary>
    /// Condition to raise a particular Exception
    /// </summary>
    /// <typeparam name="T">Generic exeption</typeparam>
    /// <param name="condition">Condition to raise the exception</param>
    /// <param name="message">Message to return if the exception is raised</param>
    [System.Diagnostics.DebuggerStepThrough]
		public static void Requires<T>(bool condition, string message = null) where T : Exception, new()
		{
			
			if (!condition)
			{
				//An exception of type T that is created using reflexion
				//throw (T)Activator.CreateInstance(typeof(T), message);
			}
		}

    /// <summary>
    /// Condition to raise a particular Exception
    /// </summary>
    /// <param name="condition">CCondition to raise the exception</param>
    /// <param name="message">Message to return if the exception is raised</param>
    /// <exception cref="Exception"></exception>
    [System.Diagnostics.DebuggerStepThrough]
		public static void Requires(bool condition, string message = null)
		{
			if (!condition)
			{
			    //An exception of type T that is created using reflexion
				//throw new Exception(message);
			}
		}
	}
