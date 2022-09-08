using System.Collections;

namespace Berry.Spider.Core;

/// <summary>
/// Bloom过滤器。https://www.cnblogs.com/heaad/archive/2011/01/02/1924195.html
/// </summary>
/// <typeparam name="T"></typeparam>
public class BloomFilterHelper<T>
{
    #region Fields

    /// <summary>
    /// The get hash secondary.
    /// </summary>
    private readonly HashFunction? _getHashSecondary;

    /// <summary>
    /// The hash bits.
    /// </summary>
    private readonly BitArray _hashBits;

    /// <summary>
    /// The hash function count.
    /// </summary>
    private readonly int _hashFunctionCount;

    #endregion Fields

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BloomFilterHelper{T}"/> class.
    /// </summary>
    /// <param name="capacity">
    /// The capacity.
    /// </param>
    /// <param name="errorRate">
    /// The error rate.
    /// </param>
    public BloomFilterHelper(int capacity, int errorRate)
        : this(capacity, errorRate, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BloomFilterHelper{T}"/> class.
    /// </summary>
    /// <param name="capacity">
    /// The capacity.
    /// </param>
    /// <param name="hashFunction">
    /// The hash function.
    /// </param>
    public BloomFilterHelper(int capacity, HashFunction? hashFunction = null)
        : this(capacity, BestErrorRate(capacity), hashFunction)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BloomFilterHelper{T}"/> class.
    /// </summary>
    /// <param name="capacity">
    /// The capacity.
    /// </param>
    /// <param name="errorRate">
    /// The error rate.
    /// </param>
    /// <param name="hashFunction">
    /// The hash function.
    /// </param>
    public BloomFilterHelper(int capacity, float errorRate, HashFunction? hashFunction)
        : this(capacity, errorRate, hashFunction, BestM(capacity, errorRate), BestK(capacity, errorRate))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BloomFilterHelper{T}"/> class.
    /// </summary>
    /// <param name="capacity">
    /// The capacity.
    /// </param>
    /// <param name="errorRate">
    /// The error rate.
    /// </param>
    /// <param name="hashFunction">
    /// The hash function.
    /// </param>
    /// <param name="m">
    /// The m.
    /// </param>
    /// <param name="k">
    /// The k.
    /// </param>
    public BloomFilterHelper(int capacity, float errorRate, HashFunction? hashFunction, int m, int k)
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "capacity must be > 0");
        }

        if (errorRate >= 1 || errorRate <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(errorRate),
                errorRate,
                string.Format("errorRate must be between 0 and 1, exclusive. Was {0}", errorRate));
        }

        if (m < 1)
        {
            throw new ArgumentOutOfRangeException(
                string.Format(
                    "The provided capacity and errorRate values would result in an array of length > int.MaxValue. Please reduce either of these values. Capacity: {0}, Error rate: {1}",
                    capacity,
                    errorRate));
        }

        if (hashFunction == null)
        {
            if (typeof(T) == typeof(string))
            {
                this._getHashSecondary = HashString;
            }
            else if (typeof(T) == typeof(int))
            {
                this._getHashSecondary = HashInt32;
            }
            else
            {
                throw new ArgumentNullException(
                    nameof(hashFunction),
                    "Please provide a hash function for your type T, when T is not a string or int.");
            }
        }
        else
        {
            this._getHashSecondary = hashFunction;
        }

        this._hashFunctionCount = k;
        this._hashBits = new BitArray(m);
    }

    #endregion Constructors and Destructors

    #region Delegates

    /// <summary>
    /// The hash function.
    /// </summary>
    /// <param name="input">
    /// The input.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public delegate int HashFunction(T input);

    #endregion Delegates

    #region Public Properties

    /// <summary>
    /// Gets the truthiness.
    /// </summary>
    public double Truthiness
    {
        get { return (double)this.TrueBits() / this._hashBits.Count; }
    }

    #endregion Public Properties

    #region Public Methods and Operators

    /// <summary>
    /// The add.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    public void Add(T item)
    {
        if (item != null)
        {
            int primaryHash = item.GetHashCode();
            if (this._getHashSecondary != null)
            {
                int secondaryHash = this._getHashSecondary(item);

                for (int i = 0; i < this._hashFunctionCount; i++)
                {
                    int hash = this.ComputeHash(primaryHash, secondaryHash, i);
                    this._hashBits[hash] = true;
                }
            }
        }
    }

    /// <summary>
    /// The contains.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public bool Contains(T item)
    {
        if (item != null)
        {
            int primaryHash = item.GetHashCode();
            if (this._getHashSecondary != null)
            {
                int secondaryHash = this._getHashSecondary(item);

                for (int i = 0; i < this._hashFunctionCount; i++)
                {
                    int hash = this.ComputeHash(primaryHash, secondaryHash, i);
                    if (this._hashBits[hash] == false)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    #endregion Public Methods and Operators

    #region Methods

    /// <summary>
    /// The best error rate.
    /// </summary>
    /// <param name="capacity">
    /// The capacity.
    /// </param>
    /// <returns>
    /// The <see cref="float"/>.
    /// </returns>
    private static float BestErrorRate(int capacity)
    {
        var c = (float)(1.0 / capacity);
        if (Math.Abs(c) > 0)
        {
            return c;
        }

        double y = int.MaxValue / (double)capacity;
        return (float)Math.Pow(0.6185, y);
    }

    /// <summary>
    /// The best k.
    /// </summary>
    /// <param name="capacity">
    /// The capacity.
    /// </param>
    /// <param name="errorRate">
    /// The error rate.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    private static int BestK(int capacity, float errorRate)
    {
        return (int)Math.Round(Math.Log(2.0) * BestM(capacity, errorRate) / capacity);
    }

    /// <summary>
    /// The best m.
    /// </summary>
    /// <param name="capacity">
    /// The capacity.
    /// </param>
    /// <param name="errorRate">
    /// The error rate.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    private static int BestM(int capacity, float errorRate)
    {
        return (int)Math.Ceiling(capacity * Math.Log(errorRate, 1.0 / Math.Pow(2, Math.Log(2.0))));
    }

    /// <summary>
    /// The hash int 32.
    /// </summary>
    /// <param name="input">
    /// The input.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    private static int HashInt32(T input)
    {
        var x = input as uint?;
        unchecked
        {
            x = ~x + (x << 15);
            x = x ^ (x >> 12);
            x = x + (x << 2);
            x = x ^ (x >> 4);
            x = x * 2057;
            x = x ^ (x >> 16);

            return (int)x;
        }
    }

    /// <summary>
    /// The hash string.
    /// </summary>
    /// <param name="input">
    /// The input.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    private static int HashString(T input)
    {
        var str = input as string;
        int hash = 0;

        if (str != null)
        {
            for (int i = 0; i < str.Length; i++)
            {
                hash += str[i];
                hash += hash << 10;
                hash ^= hash >> 6;
            }

            hash += hash << 3;
            hash ^= hash >> 11;
            hash += hash << 15;
        }

        return hash;
    }

    /// <summary>
    /// The compute hash.
    /// </summary>
    /// <param name="primaryHash">
    /// The primary hash.
    /// </param>
    /// <param name="secondaryHash">
    /// The secondary hash.
    /// </param>
    /// <param name="i">
    /// The i.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    private int ComputeHash(int primaryHash, int secondaryHash, int i)
    {
        int resultingHash = (primaryHash + (i * secondaryHash)) % this._hashBits.Count;
        return Math.Abs(resultingHash);
    }

    /// <summary>
    /// The true bits.
    /// </summary>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    private int TrueBits()
    {
        int output = 0;

        foreach (bool bit in this._hashBits)
        {
            if (bit)
            {
                output++;
            }
        }

        return output;
    }

    #endregion Methods
}