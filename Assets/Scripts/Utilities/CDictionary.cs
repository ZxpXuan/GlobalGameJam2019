using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTUtility
{
	/// <summary>
	/// A dictionary that caches the key and value collections (which mono 2.0 does not...)
	/// </summary>
	public class CDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{
		KeyCollection keys;
		ValueCollection values;

		new public KeyCollection Keys => keys ?? (keys = new KeyCollection(this));

		new public ValueCollection Values => values ?? (values = new ValueCollection(this));

		public CDictionary() : base(0, null)
		{
		}

		public CDictionary(int capacity) : base(capacity, null)
		{
		}

		public CDictionary(IEqualityComparer<TKey> comparer) : base(0, comparer)
		{
		}

		public CDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
		{
		}

		public CDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary, null)
		{
		}

		public CDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
		{
		}
	}
}
