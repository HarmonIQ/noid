﻿/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2016 Kevin Spinar (Alipha)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
namespace Liphsoft.Crypto.Argon2
{
    /// <summary>
    /// An enumeration of the possible error codes which are returned from Daniel Dinu and
    /// Dmitry Khovratovich's Argon2 library.
    /// 
    /// Some of these error conditions cannot be reached while using the C# PasswordHasher wrapper
    /// </summary>
    public enum Argon2Error 
    {
        /// <summary>
        /// The operation was successful
        /// </summary>
        OK = 0,

        /// <summary>
        /// The output hash length is less than 4 bytes
        /// </summary>
        OUTPUT_TOO_SHORT = -2,

        /// <summary>
        /// The salt is less than 8 bytes
        /// </summary>
        SALT_TOO_SHORT = -6,
        
        /// <summary>
        /// The time cost is less than 1
        /// </summary>
        TIME_TOO_SMALL = -12,
        
        /// <summary>
        /// The memory cost is less than 8 (KiB)
        /// </summary>
        MEMORY_TOO_LITTLE = -14,
        /// <summary>
        /// The memory cost is greater than 2^21 (KiB) (2 GiB)
        /// </summary>
        MEMORY_TOO_MUCH = -15,

        /// <summary>
        /// The parallelism is less than 1
        /// </summary>
        LANES_TOO_FEW = -16,
        /// <summary>
        /// The parallelism is greater than 16,777,215
        /// </summary>
        LANES_TOO_MANY = -17,
        
        /// <summary>
        /// Memory allocation failed
        /// </summary>
        MEMORY_ALLOCATION_ERROR = -22,
        
        /// <summary>
        /// The parallelism is less than 1
        /// </summary>
        THREADS_TOO_FEW = -28,
        /// <summary>
        /// The parallelism is greater than 16,777,215
        /// </summary>
        THREADS_TOO_MANY = -29,

        /// <summary>
        /// This will not be returned from the C# PasswordHasher wrapper
        /// </summary>
        DECODING_FAIL = -32,
        /// <summary>
        /// Unable to create the number of threads requested
        /// </summary>
        THREAD_FAIL = -33,

        /// <summary>
        /// This will not be returned from the C# PasswordHasher wrapper
        /// </summary>
        VERIFY_MISMATCH = -35
    }
}
