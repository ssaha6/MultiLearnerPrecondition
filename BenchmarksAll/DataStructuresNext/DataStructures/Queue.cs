﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataStructures.Utility;
using System.Diagnostics;
namespace DataStructures
{
    
   /*
     * 
     * Source Code adapted from: https://github.com/dotnet/corefx/blob/master/src/System.Collections/src/System/Collections/Generic/Queue.cs
     * 
     */
    public class Queue<T> : IEnumerable<T>,
        System.Collections.ICollection/*,
        IReadOnlyCollection<T>*/
   { 
        private T[] _array;
        private int _head;       // The index from which to dequeue if the queue isn't empty.
        private int _tail;       // The index at which to enqueue if the queue isn't full.
        private int _size;       // Number of elements.
        private int _version;
        [NonSerialized]
        private object _syncRoot;

        private const int MinimumGrow = 4;
        private const int GrowFactor = 200;  // double each time

        // Creates a queue with room for capacity objects. The default initial
        // capacity and grow factor are used.
        public Queue()
        {
            //_array = Array.Empty<T>();
            _array = EmptyArray<T>.Value;
        }

        // Creates a queue with room for capacity objects. The default grow factor
        // is used.
        public Queue(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("Creating invalid object: capacity < 0 ");
            _array = new T[capacity];
        }

        // Fills a Queue with the elements of an ICollection.  Uses the enumerator
        // to get each of the elements.
        public Queue(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("nameof(collection)");

            _array = ToArray<T>(collection, out _size);
            if (_size != _array.Length) _tail = _size;
        }
        
        public int Count
        {
            get { return _size; }
        }

               bool System.Collections.ICollection.IsSynchronized
               {
                   get { return false; }
               }

               object System.Collections.ICollection.SyncRoot
               {
                   get
                   {
                       if (_syncRoot == null)
                       {
                           System.Threading.Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                       }
                       return _syncRoot;
                   }
               }
                
        // Removes all Objects from the queue.
        public void Clear()
        {
            if (_size != 0)
            {
                //if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                //{
                if (_head < _tail)
                {
                    Array.Clear(_array, _head, _size);
                }
                else
                {
                    Array.Clear(_array, _head, _array.Length - _head);
                    Array.Clear(_array, 0, _tail);
                }
                //}

                _size = 0;
            }

            _head = 0;
            _tail = 0;
            _version++;
        }

        // CopyTo copies a collection into an Array, starting at a particular
        // index into the array.
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("Invalid array: Array==null");
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException("Invalid: arrayIndex < 0 || arrayIndex > array.Length");
            }

            int arrayLen = array.Length;
            if (arrayLen - arrayIndex < _size)
            {
                throw new ArgumentException("Invalid: arrayLen - arrayIndex < _size");
            }

            int numToCopy = _size;
            if (numToCopy == 0) return;

            int firstPart = Math.Min(_array.Length - _head, numToCopy);
            Array.Copy(_array, _head, array, arrayIndex, firstPart);
            numToCopy -= firstPart;
            if (numToCopy > 0)
            {
                Array.Copy(_array, 0, array, arrayIndex + _array.Length - _head, numToCopy);
            }
        }
        
       void System.Collections.ICollection.CopyTo(Array array, int index)
       {
           if (array == null)
           {
               throw new ArgumentNullException("nameof(array)");
           }

           if (array.Rank != 1)
           {
               throw new ArgumentException("SR.Arg_RankMultiDimNotSupported, nameof(array)");
           }

           if (array.GetLowerBound(0) != 0)
           {
               throw new ArgumentException("SR.Arg_NonZeroLowerBound, nameof(array)");
           }

           int arrayLen = array.Length;
           if (index < 0 || index > arrayLen)
           {
               throw new ArgumentOutOfRangeException("nameof(index), index, SR.ArgumentOutOfRange_Index");
           }

           if (arrayLen - index < _size)
           {
               throw new ArgumentException("SR.Argument_InvalidOffLen");
           }

           int numToCopy = _size;
           if (numToCopy == 0) return;

           try
           {
               int firstPart = (_array.Length - _head < numToCopy) ? _array.Length - _head : numToCopy;
               Array.Copy(_array, _head, array, index, firstPart);
               numToCopy -= firstPart;

               if (numToCopy > 0)
               {
                   Array.Copy(_array, 0, array, index + _array.Length - _head, numToCopy);
               }
           }
           catch (ArrayTypeMismatchException)
           {
               throw new ArgumentException("SR.Argument_InvalidArrayType, nameof(array)");
           }
       }
        
        // Adds item to the tail of the queue.
        public void Enqueue(T item) /*Originally named Push*/
        {
            if (_size == _array.Length)
            {
                int newcapacity = (int)((long)_array.Length * (long)GrowFactor / 100);
                if (newcapacity < _array.Length + MinimumGrow)
                {
                    newcapacity = _array.Length + MinimumGrow;
                }
                SetCapacity(newcapacity);
            }

            _array[_tail] = item;
            MoveNext(ref _tail);
            _size++;
            _version++;
        }

        // GetEnumerator returns an IEnumerator over this Queue.  This
        // Enumerator will support removing.
        public Enumerator GetEnumerator()
         {
             return new Enumerator(this);
         }

         /// <internalonly/>
         IEnumerator<T> IEnumerable<T>.GetEnumerator()
         {
             return new Enumerator(this);
         }

         System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
         {
             return new Enumerator(this);
         }
          
        // Removes the object at the head of the queue and returns it. If the queue
        // is empty, this method throws an 
        // InvalidOperationException.
        public T Dequeue() /*Originally named Pop*/
        {
            if (_size == 0)
            {
                ThrowForEmptyQueue();
            }

            T removed = _array[_head];
            //if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            //{
            _array[_head] = default(T);
            //}
            MoveNext(ref _head);
            _size--;
            _version++;
            return removed;
        }

        public bool TryDequeue(out T result)
        {
            if (_size == 0)
            {
                result = default(T);
                return false;
            }

            result = _array[_head];
            //if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            //{
            _array[_head] = default(T);
            //}
            MoveNext(ref _head);
            _size--;
            _version++;
            return true;
        }

        // Returns the object at the head of the queue. The object remains in the
        // queue. If the queue is empty, this method throws an 
        // InvalidOperationException.
        public T Peek()
        {
            if (_size == 0)
            {
                ThrowForEmptyQueue();
            }

            return _array[_head];
        }

        public bool TryPeek(out T result)
        {
            if (_size == 0)
            {
                result = default(T);
                return false;
            }

            result = _array[_head];
            return true;
        }

        // Returns true if the queue contains at least one object equal to item.
        // Equality is determined using EqualityComparer<T>.Default.Equals().
        public bool Contains(T item)
        {
            if (_size == 0)
            {
                return false;
            }

            if (_head < _tail)
            {
                return Array.IndexOf(_array, item, _head, _size) >= 0;
            }

            // We've wrapped around. Check both partitions, the least recently enqueued first.
            return
                Array.IndexOf(_array, item, _head, _array.Length - _head) >= 0 ||
                Array.IndexOf(_array, item, 0, _tail) >= 0;
        }

        // Iterates over the objects in the queue, returning an array of the
        // objects in the Queue, or an empty array if the queue is empty.
        // The order of elements in the array is first in to last in, the same
        // order produced by successive calls to Dequeue.
        public T[] ToArray()
        {
            if (_size == 0)
            {
                return EmptyArray<T>.Value;
            }

            T[] arr = new T[_size];

            if (_head < _tail)
            {
                Array.Copy(_array, _head, arr, 0, _size);
            }
            else
            {
                Array.Copy(_array, _head, arr, 0, _array.Length - _head);
                Array.Copy(_array, 0, arr, _array.Length - _head, _tail);
            }

            return arr;
        }

        // PRIVATE Grows or shrinks the buffer to hold capacity objects. Capacity
        // must be >= _size.
        private void SetCapacity(int capacity)
        {
            T[] newarray = new T[capacity];
            if (_size > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_array, _head, newarray, 0, _size);
                }
                else
                {
                    Array.Copy(_array, _head, newarray, 0, _array.Length - _head);
                    Array.Copy(_array, 0, newarray, _array.Length - _head, _tail);
                }
            }

            _array = newarray;
            _head = 0;
            _tail = (_size == capacity) ? 0 : _size;
            _version++;
        }

        // Increments the index wrapping it if necessary.
        private void MoveNext(ref int index)
        {
            // It is tempting to use the remainder operator here but it is actually much slower 
            // than a simple comparison and a rarely taken branch.   
            int tmp = index + 1;
            index = (tmp == _array.Length) ? 0 : tmp;
        }

        private void ThrowForEmptyQueue()
        {
            Debug.Assert(_size == 0);
            throw new InvalidOperationException("Invalid operation: empty queue");
        }

        /*Zhenglun'code*/
          public string ToStringForInts()
        {
            //This is adapted from the same method in stack.c
            string ret = "{";
            

            var en = this.GetEnumerator();
            while (en.MoveNext())
            {
                ret += en.Current.ToString() + " ";
            }
            
            return ret + "}";
            
        }

         public virtual Object Clone()
        {
            //a slight modification
            Queue<int> s = new Queue<int>(_size);
            s._head = _head;
            s._tail = _tail;
            s._size = _size;
            s._version = _version;
            Array.Copy(_array, 0, s._array, 0, _size);
            
            return s;
        }
        /* end of Zhenglun's Code*/
        /*
       public void TrimExcess()
       {
           int threshold = (int)(((double)_array.Length) * 0.9);
           if (_size < threshold)
           {
               SetCapacity(_size);
           }
       }*/

         internal static T[] ToArray<T>(IEnumerable<T> source, out int length)
        {
            if (source is ICollection<T> )
            {
                ICollection<T> ic = (ICollection<T>)source;
                int count = ic.Count;
                if (count != 0)
                {
                    // Allocate an array of the desired size, then copy the elements into it. Note that this has the same
                    // issue regarding concurrency as other existing collections like List<T>. If the collection size
                    // concurrently changes between the array allocation and the CopyTo, we could end up either getting an
                    // exception from overrunning the array (if the size went up) or we could end up not filling as many
                    // items as 'count' suggests (if the size went down).  This is only an issue for concurrent collections
                    // that implement ICollection<T>, which as of .NET 4.6 is just ConcurrentDictionary<TKey, TValue>.
                    T[] arr = new T[count];
                    ic.CopyTo(arr, 0);
                    length = count;
                    return arr;
                }
            }
            else
            {
                using (var en = source.GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        const int DefaultCapacity = 4;
                        T[] arr = new T[DefaultCapacity];
                        arr[0] = en.Current;
                        int count = 1;

                        while (en.MoveNext())
                        {
                            if (count == arr.Length)
                            {
                                // MaxArrayLength is defined in Array.MaxArrayLength and in gchelpers in CoreCLR.
                                // It represents the maximum number of elements that can be in an array where
                                // the size of the element is greater than one byte; a separate, slightly larger constant,
                                // is used when the size of the element is one.
                                const int MaxArrayLength = 0x7FEFFFFF;

                                // This is the same growth logic as in List<T>:
                                // If the array is currently empty, we make it a default size.  Otherwise, we attempt to
                                // double the size of the array.  Doubling will overflow once the size of the array reaches
                                // 2^30, since doubling to 2^31 is 1 larger than Int32.MaxValue.  In that case, we instead
                                // constrain the length to be MaxArrayLength (this overflow check works because of the
                                // cast to uint).  Because a slightly larger constant is used when T is one byte in size, we
                                // could then end up in a situation where arr.Length is MaxArrayLength or slightly larger, such
                                // that we constrain newLength to be MaxArrayLength but the needed number of elements is actually
                                // larger than that.  For that case, we then ensure that the newLength is large enough to hold
                                // the desired capacity.  This does mean that in the very rare case where we've grown to such a
                                // large size, each new element added after MaxArrayLength will end up doing a resize.
                                int newLength = count << 1;
                                if ((uint)newLength > MaxArrayLength)
                                {
                                    newLength = MaxArrayLength <= count ? count + 1 : MaxArrayLength;
                                }

                                Array.Resize(ref arr, newLength);
                            }

                            arr[count++] = en.Current;
                        }

                        length = count;
                        return arr;
                    }
                }
            }

            length = 0;
            return DataStructures.Utility.EmptyArray<T>.Value;
        }

        // Implements an enumerator for a Queue.  The enumerator uses the
        // internal version number of the list to ensure that no modifications are
        // made to the list while an enumeration is in progress.
        // [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "not an expected scenario")]
         public struct Enumerator : IEnumerator<T>,
             System.Collections.IEnumerator
         {
             private readonly Queue<T> _q;
             private readonly int _version;
             private int _index;   // -1 = not started, -2 = ended/disposed
             private T _currentElement;

             internal Enumerator(Queue<T> q)
             {
                 _q = q;
                 _version = q._version;
                 _index = -1;
                 _currentElement = default(T);
             }

             public void Dispose()
             {
                 _index = -2;
                 _currentElement = default(T);
             }

             public bool MoveNext()
             {
                 if (_version != _q._version) throw new InvalidOperationException("SR.InvalidOperation_EnumFailedVersion");

                 if (_index == -2)
                     return false;

                 _index++;

                 if (_index == _q._size)
                 {
                     // We've run past the last element
                     _index = -2;
                     _currentElement = default(T);
                     return false;
                 }

                 // Cache some fields in locals to decrease code size
                 T[] array = _q._array;
                 int capacity = array.Length;

                 // _index represents the 0-based index into the queue, however the queue
                 // doesn't have to start from 0 and it may not even be stored contiguously in memory.

                 int arrayIndex = _q._head + _index; // this is the actual index into the queue's backing array
                 if (arrayIndex >= capacity)
                 {
                     // NOTE: Originally we were using the modulo operator here, however
                     // on Intel processors it has a very high instruction latency which
                     // was slowing down the loop quite a bit.
                     // Replacing it with simple comparison/subtraction operations sped up
                     // the average foreach loop by 2x.

                     arrayIndex -= capacity; // wrap around if needed
                 }
                
                 _currentElement = array[arrayIndex];
                 return true;
             }

             public T Current
             {
                 get
                 {
                     if (_index < 0)
                         ThrowEnumerationNotStartedOrEnded();
                     return _currentElement;
                 }
             }

             private void ThrowEnumerationNotStartedOrEnded()
             {
                 Debug.Assert(_index == -1 || _index == -2);
                 throw new InvalidOperationException("_index == -1 ? SR.InvalidOperation_EnumNotStarted : SR.InvalidOperation_EnumEnded");
             }

             object System.Collections.IEnumerator.Current
             {
                 get { return Current; }
             }

             void System.Collections.IEnumerator.Reset()
             {
                 if (_version != _q._version) throw new InvalidOperationException("SR.InvalidOperation_EnumFailedVersion");
                 _index = -1;
                 _currentElement = default(T);
             }
             
         }
    }
}
