﻿// 
//  Copyright 2014 Gustavo J Knuppe (https://github.com/knuppe)
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// 
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//   - May you do good and not evil.                                         -
//   - May you find forgiveness for yourself and forgive others.             -
//   - May you share freely, never taking more than you give.                -
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//  

namespace SharpNL.Utility {
    /// <summary>
    /// Represents a text position.
    /// </summary>
    public struct TextPosition {

        /// <summary>
        /// The start text index;
        /// </summary>
        public int Start;

        /// <summary>
        /// The end text index.
        /// </summary>
        public int End;

        /// <summary>
        /// Gets the text length.
        /// </summary>
        /// <value>The text length.</value>
        public int Length {
            get { return End - Start; }
        }

        /// <summary>
        /// Gets the covered text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The covered text.</returns>
        public string GetCoveredText(string text) {
            return text.Substring(Start, Length);
        }

    }
}