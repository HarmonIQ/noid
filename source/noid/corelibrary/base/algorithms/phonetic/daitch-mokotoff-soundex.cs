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
    /// DAITCH-MOKOTOFF SOUNDEX SYSTEM WAS CREATED BY RANDY DAITCH AND GARY MOKOTOFF OF THE JEWISH GENEALOGICAL SOCIETY.
    /// 
    /// Extends AMetphone because it uses all the same functions
    /// Is not complete version of Daitch-Mokotoff, sometimes the algortime will produce more than two keys
    /// Our implementation will only produce 2
    /// </summary>
    internal class DaitchMokotoff : Phonetics
    {
        int keyLength = 6;

        /// <summary>
        /// generate a Daitch-Mokotoff Soundex key for the given string 
        /// with the default length of 6
        /// </summary>
        /// <param name="strInput">String for which to compute the key</param>
        /// <returns>primary key</returns>
        public override string GenerateKey(string strInput)
        {
            return GenerateKey(strInput,6);
        }

        /// <summary>
        /// generate a Daitch-Mokotoff Soundex key for the given string
        /// </summary>
        /// <param name="strInput">String for which to compute the key</param>
        /// <param name="keyLength">Length of the key to generate</param>
        /// <returns>primary key</returns>
        public override string GenerateKey(string strInput, int keyLength)
        {
            // reset the internal input string
            this.Reset();

            this.keyLength = keyLength;

            strInput = CheckInput(strInput);
            if(strInput == null)
                return primaryKey;
            input = strInput;

            //just to speed things up
            inputLength = input.Length;
            char lastChar = ' ';

            for(int i=0; i<inputLength;i++){
                if( lastChar == input[i])
                    continue;

                switch(input[i]) {
                case 'A':
                    i = HandleA(i);
                    break;
                case 'B':
                    AddToKey("7");
                    break;
                case 'C':
                    i = HandleC(i);
                    break;
                case 'D':
                    i = HandleD(i);
                    break;
                case 'E':
                    i = HandleE(i);
                    break;
                case 'F':
                    i = HandleF(i);
                    break;
                case 'G':
                    AddToKey("5");
                    break;
                case 'H':
                    i = HandleH(i);
                    break;
                case 'I':
                    i = HandleI(i);
                    break;
                case 'J':
                    AddToKey("1","4");
                    break;
                case 'K':
                    i = HandleK(i);
                    break;
                case 'L':
                    AddToKey("8");
                    break;
                case 'M':
                    i = HandleM(i);
                    break;
                case 'N':
                    i = HandleN(i);
                    break;
                case 'O':
                    i = HandleO(i);
                    break;
                case 'P':
                    i = HandleP(i);
                    break;
                case 'Q':
                    AddToKey("5");
                    break;
                case 'R':
                    i = HandleR(i);
                    break;
                case 'S':
                    i = HandleS(i);
                    break;
                case 'T':
                    i = HandleT(i);
                    break;
                case 'U':
                    i = HandleU(i);
                    break;
                case 'V':
                    AddToKey("7");
                    break;
                case 'W':
                    AddToKey("7");
                    break;
                case 'X':
                    i = HandleX(i);
                    break;
                case 'Y':
                    i = HandleY(i);
                    break;
                case 'Z':
                    i = HandleZ(i);
                    break;
                }
                lastChar = input[i];
            }
            if( primaryKey.Length > keyLength)
                return primaryKey.Substring(0, keyLength);
            return primaryKey.PadRight(keyLength,'0');
        }

        /// <summary>
        /// Handle the Z
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleZ(int i)
        {
            if(IsMatch(i,"ZDZ","ZDZH","ZHDZH")) {
                if(i == 0)
                    AddToKey("2");
                else
                    AddToKey("4");
                if(IsMatch(i,"ZHDZH"))
                    return i + 4;
                else if(IsMatch(i,"ZDZH"))
                    return i + 3;
                else
                    return i + 2;
            }// end ZDZ,ZDZH, ZHDZH

            if(IsMatch(i,"ZD","ZHD")) {
                if(i == 0)
                    AddToKey("2");
                else
                    AddToKey("43");
                if(IsMatch(i,"ZHD"))
                    return i + 2;
                else
                    return i + 1;
            }// End ZD, ZHD

            AddToKey("4");
            if(IsMatch(i,"ZSCH"))
                return i + 3;
            else if(IsMatch(i,"ZSH"))
                return i + 2;
            else if(IsMatch(i,"ZS","ZH"))
                return i + 1;
            else
                return i;
        }

        /// <summary>
        /// Handle the Y
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleY(int i)
        {
            if(i == 0)
                AddToKey("0");
            return i;
        }

        /// <summary>
        /// Handle the X
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleX(int i)
        {
            if(i == 0)
                AddToKey("5");
            else
                AddToKey("54");
            return i;
        }

        /// <summary>
        /// Handle the U
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleU(int i)
        {
            if(IsChar(i + 1,'I','J','Y')) {
                if(i == 0)
                    AddToKey("0");
                else if(IsVowel(i + 2))
                    AddToKey("1");
                return i + 1;
            }

            if(i == 0)
                AddToKey("0");

            if(IsChar(i,'E'))
                return i + 1;
            else
                return i;
        }

        /// <summary>
        /// Handle the T
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleT(int i)
        {
            if(IsMatch(i,"TCH","TTCH","TTSCH")) {
                AddToKey("4");
                if( IsMatch(i, "TTSCH" ) )
                    return i+4;
                else if( IsMatch( i, "TTCH" ) )
                    return i+3;
                else
                    return i+2;
            } // end TCH, TTCH, TTSCH

            if( IsMatch( i,"TH") ){
                AddToKey("3");
                return i+1;
            } // end TH

            if( IsMatch(i, "TRZ", "TRS") ){
                AddToKey("4");
                return i+2;
            }// end TRZ,TRS

            if( IsMatch(i, "TSCH", "TSH")){
                AddToKey("4");
                if( IsMatch(i,"TSCH"))
                    return i+3;
                else
                    return i+2;
            }// end TSCH, TSH

            if(IsMatch(i,"TS", "TTS", "TTSZ","TC" )){
                AddToKey("4");
                if(IsMatch(i,"TTSZ"))
                    return i+3;
                else if( IsMatch(i, "TTS") )
                    return i+2;
                else
                    return i+1;
            }// end TS, TTS, TTSZ, TC

            if(IsMatch(i,"TZ","TTZ","TZS","TSZ" )){
                AddToKey("4");
                if( IsMatch( i,"TTZ","TZS","TSZ"))
                    return i+2;
                else
                    return i+1;
            }// end TZ, TTZ, TZS, TSZ

            AddToKey("3");
            return i;
        }

        /// <summary>
        /// Handle the S
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleS(int i)
        {
            if(IsMatch(i,"SCH")) {
                if(IsMatch(i,"SCHTSCH","SCHTSH","SCHTCH") && i== 0) 
                    AddToKey("2");
                else if( IsMatch( i, "SCHT", "SCHD" ) )
                if( i== 0)
                    AddToKey("2");
                else
                    AddToKey("43");
                else
                    AddToKey("4");

                if(IsMatch(i,"SCHTSCH"))
                    return i + 6;
                else if(IsMatch(i,"SCHTSH","SCHTCH"))
                    return i + 5;
                else if(IsMatch(i,"SCHT","SCHD"))
                    return i + 3;
                else
                    return i + 2;
            }// end SCH

            if(IsMatch(i,"SHTCH","SHCH","SHTSH")) {
                if(i == 0)
                    AddToKey("2");
                else
                    AddToKey("4");
                if(IsMatch(i,"SHCH"))
                    return i + 3;
                else
                    return i + 4;
            } // end SHTCH SHCH SHTSH

            if(IsMatch(i,"SHT")) {
                if(i == 0)
                    AddToKey("2");
                else
                    AddToKey("43");

                return i + 2;
            } //end SHT

            if(IsMatch(i,"SH")) {
                AddToKey("4");
                return i + 1;
            } // end SH

            if(IsMatch(i,"STCH","STSCH","SC")) {
                if(i == 0)
                    AddToKey("2");
                else
                    AddToKey("4");
                if(IsMatch(i,"STSCH"))
                    return i + 4;
                else if(IsMatch(i,"STCH"))
                    return i + 3;
                else
                    return i + 1;
            }// end STCH, STSCH, SC


            if(IsMatch(i,"STRZ","STRS","STSH")) {
                if(i == 0)
                    AddToKey("2");
                else
                    AddToKey("4");

                return i + 3;
            }//End STRZ, STRS, STSH

            if( IsMatch(i, "ST") ){
                if(i == 0)
                    AddToKey("2");
                else
                    AddToKey("43");
                return i+1;
            }// end ST

            if( IsMatch(i,"SZCZ", "SZCS" ) ){
                if( i==0)
                    AddToKey("2");
                else
                    AddToKey("4");
                return i+3;
            } // end SZCZ, SZCS

            if( IsMatch(i,"SZT","SHD","SZD") ){
                if( i ==0)
                    AddToKey("2");
                else
                    AddToKey("43");
                return i+2;
            } // end SZT, SHD, SZD

            if( IsMatch(i, "SD") ){
                if( i ==0)
                    AddToKey("2");
                else
                    AddToKey("43");
                return i+1;
            }// end SD

            AddToKey( "4");
            if( IsMatch(i, "SZ") )
                return i+1;
            else
                return i;
        }

        /// <summary>
        /// Handle the R
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleR(int i)
        {
            if(IsChar(i + 1,'S','Z')) {
                AddToKey("94","4");
                return i + 1;
            }

            AddToKey("9");
            return i;
        }

        /// <summary>
        /// Handle the P
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleP(int i)
        {
            AddToKey("7");

            if(IsChar(i + 1,'F','H'))
                return i + 1;
            else
                return i;
        }

        /// <summary>
        /// Handle the O
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleO(int i)
        {
            if( IsChar(i+1,'I','J','Y') ){
                if(i == 0)
                    AddToKey("0");
                else if(IsVowel(i + 2))
                    AddToKey("1");
                return i + 1;
            }

            if(i == 0)
                AddToKey("0");
            return i;
        }

        /// <summary>
        /// Handle the N
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleN(int i)
        {
            if(IsChar(i + 1,'M') && i !=0 ) {
                AddToKey("66");
                return i+1;
            }

            AddToKey("6");
            return i;     
        }

        /// <summary>
        /// Handle the M
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleM(int i)
        {
            if(IsChar(i + 1,'N') && i !=0 ) {
                AddToKey("66");
                return i+1;
            }

            AddToKey("6");
            return i;
        }

        /// <summary>
        /// Handle the K
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleK(int i)
        {
            if( IsChar( i+1, 'S')){
                if(i == 0)
                    AddToKey("5");
                else
                    AddToKey("54");
                return i + 1;
            }

            AddToKey("5");
            if(IsChar(i + 1,'H'))
                return i + 1;
            else
                return i;
        }

        /// <summary>
        /// Handle the I
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleI(int i)
        {
            if(IsChar(i + 1,'A','E','O','U') && i == 0) {
                AddToKey("1");
                return i + 1;
            }

            if(i == 0) 
                AddToKey("0");
            return i;

        }

        /// <summary>
        /// Handle the H
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleH(int i)
        {
            if(i == 0 || IsVowel(i + 1))
                AddToKey("5");
            return i;

        }

        /// <summary>
        /// Handle the F
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleF(int i)
        {
            AddToKey("7");
            if(IsChar(i + 1,'B'))
                return i + 1;
            else
                return i;
        }

        /// <summary>
        /// Handle the E
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleE(int i)
        {
            if(IsChar(i + 1,'I','J','Y')) {
                if(i == 0) 
                    AddToKey("0");
                else if(IsVowel( i+2 ) )
                    AddToKey("1");

                return i+1;
            }

            if( IsChar(i+1,'U')){
                if( i==0 || IsVowel( i+2 ) )
                    AddToKey("1");

                return i+1;
            }

            if( i == 0) 
                AddToKey("0");
            return i;
        }

        /// <summary>
        /// Handle the D
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleD(int i)
        {
            if(IsMatch(i,"DRZ","DRS", "DSH","DSZ","DZH","DZS")) {
                AddToKey("4");
                return i + 2;
            }

            if(IsMatch(i,"DS","DZ")) {
                AddToKey("4");
                return i + 1;
            }

            AddToKey("3");
            if(IsChar(i + 1,'T'))
                return i + 1;
            else
                return i;
        }

        /// <summary>
        /// Handle the C
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleC(int i)
        {
            if( IsChar( i+1, 'H') ){
                AddToKey( "5", "4");
                return i+1;
            } 

            if( IsChar( i+1, 'K') ){
                AddToKey( "5", "45");
                return i+1;
            }

            if( IsMatch( i, "CS","CSZ","CZS") ) {
                AddToKey( "4" );
                if(IsMatch(i,"CSZ","CZS"))
                    return i + 2;
                else
                    return i + 1;
            }

            AddToKey("5","4");
            return i;

        }

        /// <summary>
        /// Handle the A
        /// </summary>
        /// <param name="i">Currunt postion in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleA(int i)
        {
            if( i == 0 )
                AddToKey("0");
            else if( IsChar( i+1, 'I','J','Y') && IsVowel(i+2) )
                AddToKey("1");
            else if( IsChar( i+1, 'u') && IsVowel(i+2) )
                AddToKey("7");

            if( IsChar( i+1, 'I','J','Y','U'))
                return i+1;
            else
                return i;
        }

        /// <summary>
        /// Check wheter the letter at the given position in the input string is a vowel
        /// Vowels are A, E, I, O, U and Y
        /// </summary>
        /// <param name="pos">Position in the input string</param>
        /// <returns>Vowel or not</returns>
        private bool IsVowel(int pos){
            return IsChar(pos, 'E','A','I','O','U','Y');
        }

        /// <summary>
        /// Get the primary key
        /// </summary>
        public override string PrimaryKey
        {
            get
            {
                return primaryKey.PadRight(keyLength,'0');
            }
        }

        /// <summary>
        /// Get the alternate key
        /// If no alternate key is present the primary key is returned
        /// </summary>
        public override string AlternateKey
        {
            get
            {
                return alternateKey.PadRight(keyLength,'0');
            }
        }
    }
}