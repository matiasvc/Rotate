using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShuffleBag<T>
{
	public ShuffleBag(T[] startArray)
	{
		this.startArray = startArray;
	}
	
	private T[] startArray;
	private Stack<T> shuffleStack = null;
	
	public T Grab()
	{
		if (shuffleStack == null || shuffleStack.Count == 0)
		{
			List<T> shuffleList = new List<T>(startArray);
			
			foreach (T item in startArray)
				shuffleList.Add(item);
			
			int n = shuffleList.Count;
			
			while (n > 1)
			{
				n--;
				int k = Random.Range(0, n+1);
				T value = shuffleList[k];
				shuffleList[k] = shuffleList[n];
				shuffleList[n] = value;
			}
			
			shuffleStack = new Stack<T>(shuffleList);
		}
		
		return shuffleStack.Pop();
	}
}
