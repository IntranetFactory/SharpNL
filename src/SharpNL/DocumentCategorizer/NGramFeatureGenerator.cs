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

using System.Collections.Generic;
using SharpNL.Utility;

namespace SharpNL.DocumentCategorizer {
    /// <summary>
    /// Represents a nGram feature generator.
    /// </summary>   
    [TypeClass("opennlp.tools.doccat.NGramFeatureGenerator")]
    public class NGramFeatureGenerator : IFeatureGenerator {

        /// <summary>
        /// Extracts the features from the given text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="extraInformation">The extra information.</param>
        /// <returns>The list of features.</returns>
        public List<string> ExtractFeatures(string[] text, Dictionary<string, object> extraInformation) {
            var features = new List<string>(text.Length * 8);
            for (int i = 0; i < text.Length - 1; i++) {
                features.Add(string.Format("ng={0}:{1}", text[i], text[i + 1]));
            }
            return features;
        }
    }
}