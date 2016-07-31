/*
 * Phonetic
 * Copyright (c) 2006, Triple Software
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are 
 * permitted provided that the following conditions are met:
 * 
 * - Redistributions of source code must retain the above copyright notice, this list 
 *   of conditions and the following disclaimer.
 * 
 * - Redistributions in binary form must reproduce the above copyright notice, this list
 *   of conditions and the following disclaimer in the documentation and/or other materials 
 *   provided with the distribution.
 * 
 * - Neither the name of the Triple Software nor the names of its contributors may be used to 
 *   endorse or promote products derived from this software without specific prior written 
 *   permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS 
 * OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
 * AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER 
 * IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
 * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
// Copyright (c) 2016 NoID Developers
// Distributed under the MIT/X11 software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;

namespace NoID.Base.Algorithms
{
    /// <summary>
    /// interface for phonetic functions
    /// </summary>
    internal interface IPhonetic
    {
        /// <summary>
        /// Generate a key accourding to the used algoritm
        /// </summary>
        /// <param name="strInput">String from whihch to calculate the key</param>
        /// <returns>Calculated key</returns>
        string GenerateKey( string strInput );

        /// <summary>
        /// Generate a key with a differnt key length than the deafult for the use algoritm
        /// </summary>
        /// <param name="strInput">String from whihch to calculate the key</param>
        /// <param name="keyLength">Length of the key to generate</param>
        /// <returns>Calculated key</returns>
        string GenerateKey( string strInput, int keyLength );

        /// <summary>
        /// Get the primary key
        /// </summary>
        string PrimaryKey{
            get;
        }

        /// <summary>
        /// Get the alternate key, if not present return the primary key
        /// </summary>
        string AlternateKey{
            get;
        }
    }

    /// <summary>
    /// Abstarct class for use with phonetics classes
    /// Also used bij Daitch-Mokotoff soundex class.
    /// </summary>
    internal abstract class Phonetics : IPhonetic
    {
        /// <summary>
        /// The input string
        /// </summary>
        protected string input = "";
        /// <summary>
        /// Total length of the input string
        /// </summary>
        protected int inputLength = 0;
        /// <summary>
        /// Primary key buffer
        /// </summary>
        protected string primaryKey = "";
        /// <summary>
        /// Alternate Key buffer
        /// </summary>
        protected string alternateKey = "";

        /// <summary>
        /// Get the primary key
        /// </summary>
        public virtual string PrimaryKey {
            get {
                return primaryKey;
            }
        }

        /// <summary>
        /// Get the alternate key
        /// If no alternate key is present the primary key is returned
        /// </summary>
        public virtual string AlternateKey
        {
            get {
                return alternateKey;
            }
        }

        /// <summary>
        /// Generate the keys accourding to the used algorithm
        /// </summary>
        /// <param name="strInput">string to encrypt</param>
        /// <returns>primary keyt</returns>
        public abstract string GenerateKey(string strInput);

        /// <summary>
        /// Generate the keys accourding to the used algorithm
        /// with a fixed key length
        /// </summary>
        /// <param name="strInput">string to encrypt</param>
        /// <param name="keyLength">Key length</param>
        /// <returns>primary keyt</returns>
        public abstract string GenerateKey(string strInput, int keyLength);

        /// <summary>
        /// Reset the primaryKey, alternateKey and input;
        /// </summary>
        protected void Reset(){
            input = "";
            primaryKey = "";
            alternateKey = "";
            inputLength = 0;    
        }

        /// <summary>
        /// Do a simple check if the input can be computed
        /// </summary>
        /// <param name="strInput"></param>
        protected string CheckInput(string strInput){

            strInput = strInput.Trim();
            // Cant work wih empty string
            if( strInput.Equals( "" ) )
                return null;
            return strInput.ToUpper();

        }

        /// <summary>
        /// Add a string to the key
        /// </summary>
        /// <param name="primary">string to add</param>
        protected void AddToKey( string primary )
        {
            AddToKey( primary, null );
        }

        /// <summary>
        /// Add a string to the primaryKey en Alsternate key
        /// </summary>
        /// <param name="primary">string to add to the primary key</param>
        /// <param name="alternate">string to add to the alternate key</param>
        protected void AddToKey( string primary, string alternate)
        {
            primaryKey += primary;
            if( null == alternate )
                alternateKey += primary;
            else
                alternateKey += alternate;
        }

        /// <summary>
        /// Check wheter the char at the given posirion is char given
        /// </summary>
        /// <param name="pos">Position of the char</param>
        /// <param name="c">Char wich to check</param>
        /// <returns>match</returns>
        protected bool IsChar(int pos, Char c)
        {
            bool result = false;
            if( pos >= 0 && pos < inputLength )
            if( input[pos] == c)
                result = true;

            return result;
        }

        /// <summary>
        /// Check wheter the char matched any of the chars given
        /// </summary>
        /// <param name="pos">position od the char in th input string</param>
        /// <param name="charList">array of char to match</param>
        /// <returns>match</returns>
        protected bool IsChar(int pos, params char[] charList){
            bool result = false;
            if( pos >= 0 && pos < inputLength ){
                char inputChar = input[pos];
                foreach(char c in charList)
                    if( c == inputChar )
                        result = true;
            }
            return result;
        }

        /// <summary>
        /// Check id the a string te macth is present at the given position
        /// </summary>
        /// <param name="pos">position in the input string</param>
        /// <param name="toMatch">String to match on the given position</param>
        /// <returns></returns>
        protected bool IsMatch(int pos, string toMatch){
            bool result = false;
            int length = toMatch.Length;
            if( (pos+length) <= inputLength && pos >= 0 )
            if( input.Substring( pos, length) == toMatch)
                result = true;
            return result;
        }

        /// <summary>
        /// Check if any of the given matches match the string form the given position in the input string
        /// </summary>
        /// <param name="pos">position in the input string</param>
        /// <param name="toMatch">Strings to match on the given position</param>
        /// <returns></returns>
        protected bool IsMatch(int pos,params string[] toMatch){
            bool result = false;
            foreach(string match in toMatch)
                if(IsMatch(pos,match))
                    result = true;
            return result;
        }
    }
}