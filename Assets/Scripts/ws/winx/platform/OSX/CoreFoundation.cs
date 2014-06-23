#region License
//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2010 the Open Toolkit library.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ws.winx.platform.osx.Carbon
{
    using CFRunLoop = System.IntPtr;
	using CFAllocatorRef=System.IntPtr;
	using CFDictionaryRef=System.IntPtr;
	using CFDictionaryKeyCallBacks=System.IntPtr;
	using CFDictionaryValueCallBacks=System.IntPtr;
	using CFIndex=System.Int32;
	using CFStringRef=System.IntPtr;
	using CFNumberRef=System.IntPtr;
	using CFArrayCallBacks=System.IntPtr;

    struct CFArray
    {
        IntPtr arrayRef;
        public IntPtr Ref { get { return arrayRef; } set { arrayRef = value; } }

        public CFArray(IntPtr reference)
        {
            arrayRef = reference;
        }

        public int Count
        {
            get { return CF.CFArrayGetCount(arrayRef); }
        }
        public IntPtr this[int index]
        {
            get
            {
                if (index >= Count || index < 0)
                    throw new IndexOutOfRangeException();

                return CF.CFArrayGetValueAtIndex(arrayRef, index);
            }
        }
    }
    struct CFDictionary
    {
        public CFDictionary(IntPtr reference)
        {
            dictionaryRef = reference;
        }

        IntPtr dictionaryRef;
        public IntPtr Ref { get { return dictionaryRef; } set { dictionaryRef = value; } }

        public int Count
        {
            get
            {
                return CF.CFDictionaryGetCount(dictionaryRef);
            }
        }
        public double GetNumberValue(string key)
        {
            unsafe
            {
                double retval;
                IntPtr cfnum = CF.CFDictionaryGetValue(dictionaryRef,
                    CF.CFSTR(key));

                CF.CFNumberGetValue(cfnum, CF.CFNumberType.kCFNumberDoubleType, &retval);

                return retval;
            }

        }
    }
    class CF
    {
        const string appServices = "/System/Library/Frameworks/ApplicationServices.framework/Versions/Current/ApplicationServices";
		const string coreFoundationLibrary = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
        
		[DllImport(coreFoundationLibrary)]
        internal static extern int CFArrayGetCount(IntPtr theArray);

		[DllImport(coreFoundationLibrary)]
        internal static extern IntPtr CFArrayGetValueAtIndex(IntPtr theArray, int idx);

		[DllImport(coreFoundationLibrary)]
        internal static extern int CFDictionaryGetCount(IntPtr theDictionary);

		[DllImport(coreFoundationLibrary)]
        internal static extern IntPtr CFDictionaryGetValue(IntPtr theDictionary, IntPtr theKey);

		[DllImport(coreFoundationLibrary)]
		//extern static IntPtr CFArrayCreate (IntPtr allocator, IntPtr values, CFIndex numValues, IntPtr callbacks);
		internal static extern IntPtr CFArrayCreate (CFAllocatorRef allocator, IntPtr[] values, CFIndex numValues, CFArrayCallBacks callbacks);


		[DllImport(coreFoundationLibrary)]
		//extern static IntPtr CFDictionaryCreate (IntPtr allocator, IntPtr[] keys, IntPtr[] vals, int len, IntPtr keyCallbacks, IntPtr valCallbacks);

		internal static extern CFDictionaryRef CFDictionaryCreate (
			CFAllocatorRef allocator,
			CFStringRef[] keys,
			CFNumberRef[] values,
			CFIndex numValues,
			CFDictionaryKeyCallBacks keyCallBacks,
			CFDictionaryValueCallBacks valueCallBacks
			);

        // this mirrors the definition in CFString.h.
        // I don't know why, but __CFStringMakeConstantString is marked as "private and should not be used directly"
        // even though the CFSTR macro just calls it.
        [DllImport(appServices)]
        static extern IntPtr __CFStringMakeConstantString(string cStr);
        internal static IntPtr CFSTR(string cStr)
        {
            return __CFStringMakeConstantString(cStr);
        }

		[DllImport(appServices)]
        internal unsafe static extern bool CFNumberGetValue (IntPtr number, CFNumberType theType, int* valuePtr);
//		internal static extern bool CFNumberGetValue (IntPtr number, CFNumberType theType, int* valuePtr);

		[DllImport(appServices)]
// internal static extern bool CFNumberGetValue(IntPtr number, CFNumberType theType, double* valuePtr);
        internal unsafe static extern bool CFNumberGetValue(IntPtr number, CFNumberType theType, double* valuePtr);

        internal enum CFNumberType
        {
            kCFNumberSInt8Type = 1,
            kCFNumberSInt16Type = 2,
            kCFNumberSInt32Type = 3,
            kCFNumberSInt64Type = 4,
            kCFNumberFloat32Type = 5,
            kCFNumberFloat64Type = 6,
            kCFNumberCharType = 7,
            kCFNumberShortType = 8,
            kCFNumberIntType = 9,
            kCFNumberLongType = 10,
            kCFNumberLongLongType = 11,
            kCFNumberFloatType = 12,
            kCFNumberDoubleType = 13,
            kCFNumberCFIndexType = 14,
            kCFNumberNSIntegerType = 15,
            kCFNumberCGFloatType = 16,
            kCFNumberMaxType = 16
        };

        public enum CFRunLoopExitReason
        {
            Finished = 1,
            Stopped = 2,
            TimedOut = 3,
            HandledSource = 4
        }

        public static readonly IntPtr RunLoopModeDefault = CF.CFSTR("kCFRunLoopDefaultMode");

        [DllImport(appServices)]
        internal static extern CFRunLoop CFRunLoopGetCurrent();

        [DllImport(appServices)]
        internal static extern CFRunLoop CFRunLoopGetMain();

        [DllImport(appServices)]
        internal static extern CFRunLoopExitReason CFRunLoopRunInMode(
            IntPtr cfstrMode, double interval, bool returnAfterSourceHandled);
    }
}