// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-20-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-20-2016
// ***********************************************************************
// <copyright file="Tuple.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
/// <summary>
/// Class Tuple.
/// </summary>
/// <typeparam name="T1">The type of the t1.</typeparam>
/// <typeparam name="T2">The type of the t2.</typeparam>
public class Tuple<T1, T2>
{
    /// <summary>
    /// Gets the first.
    /// </summary>
    /// <value>The first.</value>
    public T1 First { get; private set; }
    /// <summary>
    /// Gets the second.
    /// </summary>
    /// <value>The second.</value>
    public T2 Second { get; private set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="Tuple{T1, T2}"/> class.
    /// </summary>
    /// <param name="first">The first.</param>
    /// <param name="second">The second.</param>
    internal Tuple(T1 first, T2 second)
	{
		First = first;
		Second = second;
	}
}