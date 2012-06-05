using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LinqToEdmx.Common
{
  public static class EnumerableExtensions
  {
    /// <summary>
    /// Returns a sequence with <paramref name="head"/> as its first element and <paramref name="tail"/> as its subsequent elements.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the sequence.</typeparam>
    /// <param name="head">The element that will be first in the sequence.</param>
    /// <param name="tail">The elements that will be subsequent to the head element.</param>
    [DebuggerHidden]
    public static IEnumerable<TSource> Concat<TSource>(this TSource head, IEnumerable<TSource> tail)
    {
      yield return head;

      if (tail.Any())
      {
        foreach (var item in tail)
        {
          yield return item;
        }
      }
    }

    /// <summary>
    /// Returns a sequence with <paramref name="first"/> as its first element and <paramref name="second"/> as the second element.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the sequence.</typeparam>
    /// <param name="first">The element that will be first in the sequence.</param>
    /// <param name="second">The elements that will be second in the sequence.</param>
    [DebuggerHidden]
    public static IEnumerable<TSource> Concat<TSource>(this TSource first, TSource second)
    {
      yield return first;
      yield return second;
    }

    /// <summary>
    /// Returns a sequence with <paramref name="first"/> as its first elements and <paramref name="second"/> as the last element.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the sequence.</typeparam>
    /// <param name="first">The first.</param>
    /// <param name="second">The second.</param>
    /// <returns></returns>
    [DebuggerHidden]
    public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, TSource second)
    {
      foreach (var item in first)
      {
        yield return item;
      }

      yield return second;
    }

    /// <summary>
    /// Returns a sequence with <paramref name="item"/> as its only item.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <param name="item">The item to return in a sequence.</param>
    [DebuggerHidden]
    public static IEnumerable<TElement> AsEnumerable<TElement>(this TElement item)
    {
      yield return item;
    }

    /// <summary>
    /// Determines whether the second sequence is a subset of the first sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <param name="first">The first sequence.</param>
    /// <param name="second">The second.</param>
    /// <returns>
    /// 	<c>true</c> if all elements in the <paramref name="second"/> sequence exist in the <paramref name="first"/> sequence; otherwise, <c>false</c>.
    /// </returns>
    [DebuggerHidden]
    public static bool Contains<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
    {
      return !second.Except(first).Any();
    }

    /// <summary>
    /// Determines whether the map contains all of the specified keys.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="map">The map.</param>
    /// <param name="keys">The keys.</param>
    /// <returns>
    /// 	<c>true</c> if all elements in the <paramref name="keys"/> sequence exist in the <paramref name="map"/>; otherwise, <c>false</c>.
    /// </returns>
    [DebuggerHidden]
    public static bool ContainsKeys<TKey, TValue>(this IDictionary<TKey, TValue> map, IEnumerable<TKey> keys)
    {
      return keys.All(map.ContainsKey);
    }

    /// <summary>
    /// Returns the result of applying an accumulator function over a sequence until the specified condition is true, 
    /// or the final accumulator value if no accumulator value satisfied the condition. 
    /// The specified seed value is used as the initial accumulator value.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to aggregate over.</param>
    /// <param name="seed">The initial accumulator value.</param>
    /// <param name="func">An accumulator function to be invoked on each element.</param>
    /// <param name="predicate">A function to test each accumulator value for a condition.</param>
    /// <returns>The final accumulator value that satisfied the condition or the final accumulator value if no accumulator value satisfied the condition.</returns>
    [DebuggerHidden]
    public static TAccumulate AggregateUntil<TSource, TAccumulate>(this IEnumerable<TSource> source, 
      TAccumulate seed, 
      Func<TAccumulate, TSource, TAccumulate> func, 
      Func<TAccumulate, bool> predicate)
    {
      var currentAggregate = seed;
      foreach (var element in source)
      {
        currentAggregate = func(currentAggregate, element);
        if(predicate(currentAggregate))
        {
          return currentAggregate;
        }
      }
      return currentAggregate;
    }
  }
}