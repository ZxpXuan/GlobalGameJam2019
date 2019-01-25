using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JTUtility
{
	[Serializable]
	public abstract class SDictionary
	{
		public abstract void Init();
	}

	/// <summary>
	/// Like SDictionary, but duplicate keys are preserved
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	[Serializable]
	public class SPairList<TKey, TValue> : SDictionary
	{
		/// <summary>
		/// The runtime dictionary
		/// </summary>
		[NonSerialized]
		protected CDictionary<TKey, TValue> dict;

		/// <summary>
		/// The list of keys
		/// </summary>
		[SerializeField]
		protected List<TKey> keys = new List<TKey>();

		/// <summary>
		/// The list of values
		/// </summary>
		[SerializeField]
		protected List<TValue> values = new List<TValue>();

		public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
				yield return new KeyValuePair<TKey, TValue>(keys[i], values[i]);
		}

		public override void Init()
		{
		}
	}


	/// <summary>
	/// An <see cref="IDictionary{TKey, TValue}"/>
	/// implementation that Unity can serialize
	/// </summary>
	/// <typeparam name="TKey">The type of the keys</typeparam>
	/// <typeparam name="TValue">The type of the values</typeparam>
	[Serializable]
	public class SDictionary<TKey, TValue> : SPairList<TKey, TValue>, IDictionary<TKey, TValue>
	{
		public SDictionary()
		{
		}

		public SDictionary(IEqualityComparer<TKey> comparer)
		{
			KeyComparer = comparer;
		}


		/// <summary>
		/// The number of elements in the dictionary
		/// </summary>
		public int Count => dict?.Count ?? keys.Count;

		/// <summary>
		/// Gets a collection of all the dictionary's keys
		/// </summary>
		public virtual ICollection<TKey> Keys
		{
			get
			{
				Init();
				return dict.Keys;
			}
		}

		/// <summary>
		/// Gets a collection of all the dictionary's values
		/// </summary>
		public virtual ICollection<TValue> Values
		{
			get
			{
				Init();
				return dict.Values;
			}
		}

		/// <summary>
		/// Interface implementation. Always returns <c>false</c>
		/// </summary>
		public virtual bool IsReadOnly => false;

		IEqualityComparer<TKey> keyComparer;
		IEqualityComparer<TValue> valueComparer;

		/// <summary>
		/// Gets and sets the equality comparer for keys
		/// </summary>
		public IEqualityComparer<TKey> KeyComparer
		{
			get { return keyComparer ?? (keyComparer = EqualityComparer<TKey>.Default); }
			set
			{
				keyComparer = value;
				if (dict != null)
					dict = new CDictionary<TKey, TValue>(dict, KeyComparer);
			}
		}

		/// <summary>
		/// Gets and sets the equality comparer for values
		/// </summary>
		public IEqualityComparer<TValue> ValueComparer
		{
			get { return valueComparer ?? (valueComparer = EqualityComparer<TValue>.Default); }
			set { valueComparer = value; }
		}

		/// <summary>
		/// Initialises the runtime dictionary using the serialized values
		/// </summary>
		public override void Init()
		{
			// In the editor, it's possible to edit the values directly after 
			// initialization so we need to re-do that if we detect something's up
			if (dict == null || (Application.isEditor &&
				(keys.Count != values.Count || keys.Count != dict.Count)))
			{
				// We should make sure that keys.Count == values.Count
				// It should do 99% of the time, but just in case
				while (keys.Count > values.Count)
					values.Add(default(TValue));
				if (values.Count > keys.Count)
					values.RemoveRange(keys.Count, values.Count - keys.Count);
				if (dict == null)
					dict = new CDictionary<TKey, TValue>(Count, KeyComparer);
				else
					dict.Clear();
				bool error = false;
				for (int i = 0; i < keys.Count; i++)
				{
					// If we have duplicate elements, warn the user after removing them
					// disable once CompareNonConstrainedGenericWithNull
					if (keys[i] == null || !dict.TryAdd(keys[i], values[i]))
					{
						keys.RemoveAt(i);
						values.RemoveAt(i);
						i--;
						if (!error)
						{
							Debug.LogWarning(GetType() +
								": Duplicate or null keys in data store. Removing");
							error = true;
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the value associated with the specified key
		/// </summary>
		/// <param name="key"></param>
		public virtual TValue this[TKey key]
		{
			get
			{
				Init();
				return dict[key];
			}

			set
			{
				Init();
				dict[key] = value;
				if (Application.isEditor)
				{
					for (int i = 0; i < keys.Count; i++)
					{
						if (KeyComparer.Equals(key, keys[i]))
						{
							values[i] = value;
							return;
						}
					}
					keys.Add(key);
					values.Add(value);
				}
			}
		}

		public virtual string GetPropertyPath(TKey key)
		{
			if (!Application.isEditor)
				throw new InvalidOperationException("Can only be called in the editor");
			for (int i = 0; i < keys.Count; i++)
			{
				if (KeyComparer.Equals(key, keys[i]))
					return nameof(values) + $".Array.data[{i}]";
			}
			throw new KeyNotFoundException();
		}

		/// <summary>
		/// Adds a new item to the dictionary
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="value">The value</param>
		public virtual void Add(TKey key, TValue value)
		{
			Init();
			dict.Add(key, value);
			if (Application.isEditor)
			{
				keys.Add(key);
				values.Add(value);
			}
		}

		/// <summary>
		/// Checks that the dictionary contains the specified key
		/// </summary>
		/// <param name="key"></param>
		/// <returns><c>true</c> if the key exists, otherwise
		/// <c>false</c></returns>
		public virtual bool ContainsKey(TKey key)
		{
			Init();
			return dict.ContainsKey(key);
		}

		/// <summary>
		/// Removes the specified key from the dictionary
		/// </summary>
		/// <param name="key"></param>
		/// <returns><c>true</c> if the value exists and was removed,
		/// otherwise <c>false</c></returns>
		public virtual bool Remove(TKey key)
		{
			Init();
			var ret = dict.Remove(key);
			if (ret && Application.isEditor)
			{
				int i;
				for (i = 0; i <= Count; i++)
				{
					if (KeyComparer.Equals(key, keys[i]))
					{
						keys.RemoveAt(i);
						values.RemoveAt(i);
						break;
					}
				}
			}
			return ret;
		}

		/// <summary>
		/// Gets the value associated with the specified key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns><c>true</c> if the key exists and the value was found,
		/// otherwise <c>false</c></returns>
		public virtual bool TryGetValue(TKey key, out TValue value)
		{
			Init();
			return dict.TryGetValue(key, out value);
		}

		/// <summary>
		/// Removes all elements from the dictionary
		/// </summary>
		public virtual void Clear()
		{
			dict?.Clear();
			keys.Clear();
			values.Clear();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the dictionary
		/// </summary>
		/// <returns></returns>
		public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			Init();
			return dict.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the dictionary
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			Init();
			return ((IEnumerable)dict).GetEnumerator();
		}

		/// <summary>
		/// Adds the specified key and value pair to the dictionary
		/// </summary>
		/// <param name="pair"></param>
		public void Add(KeyValuePair<TKey, TValue> pair)
		{
			Add(pair.Key, pair.Value);
		}

		/// <summary>
		/// Removes the specified key and its associated value from the dictionary
		/// </summary>
		/// <param name="pair"></param>
		/// <returns></returns>
		public bool Remove(KeyValuePair<TKey, TValue> pair)
		{
			return Remove(pair.Key);
		}

		/// <summary>
		/// Checks if the dictionary contains the specified key and value pair
		/// </summary>
		/// <param name="pair"></param>
		/// <returns></returns>
		public bool Contains(KeyValuePair<TKey, TValue> pair)
		{
			Init();
			return (dict.ContainsKey(pair.Key) &&
				ValueComparer.Equals(pair.Value, dict[pair.Key]));
		}

		/// <summary>
		/// Copies the elements of the collection to the specified array
		/// </summary>
		/// <param name="array">The array to copy to</param>
		/// <param name="index">The index in <paramref name="array"/> in which
		/// to begin copying</param>
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			((ICollection<KeyValuePair<TKey, TValue>>)dict).CopyTo(array, index);
		}
	}
}
