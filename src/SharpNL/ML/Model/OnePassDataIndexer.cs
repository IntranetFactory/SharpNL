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

using System;
using System.Collections.Generic;
using SharpNL.Utility;

namespace SharpNL.ML.Model {
    /// <summary>
    /// An indexer for maxent model data which handles cutoffs for uncommon contextual predicates and provides a unique integer index for each of the predicates.
    /// </summary>
    public class OnePassDataIndexer : AbstractDataIndexer {

        protected readonly IObjectStream<Event> eventStream;
        protected readonly int cutoff;
        protected readonly bool sort;

        public OnePassDataIndexer(IObjectStream<Event> eventStream, int cutoff) : this(eventStream, cutoff, true) { }
        public OnePassDataIndexer(IObjectStream<Event> eventStream, int cutoff, bool sort) {
            this.eventStream = eventStream;
            this.cutoff = cutoff;
            this.sort = sort;
        }

        protected override void PerformIndexing() {

            var predicateIndex = new Dictionary<string, int>();

            Console.Out.WriteLine("Indexing events using cutoff of " + cutoff);
            Console.Out.WriteLine("\tComputing event counts...");

            LinkedList<Event> events = ComputeEventCounts(predicateIndex);

            Console.Out.WriteLine("done. " + events.Count + " events");


            Console.Out.WriteLine("\tIndexing...");

            List<ComparableEvent> eventsToCompare = Index(events, predicateIndex);

            events.Clear();
            predicateIndex.Clear();

            Console.Out.WriteLine("done.");

            Console.Out.WriteLine("Sorting and merging events...");

            SortAndMerge(eventsToCompare, sort);

            Console.Out.WriteLine("Done indexing.");
        }

        #region . ComputeEventCounts .

        /// <summary>
        /// Reads events into a linked list. The predicates associated with each event are
        /// counted and any which occur at least cutoff times are added to the 
        /// <see cref="predicatesInOut"/> map along with a unique integer index.
        /// </summary>
        /// <param name="predicatesInOut">The predicates.</param>
        /// <returns>The events</returns>
        private LinkedList<Event> ComputeEventCounts(Dictionary<string, int> predicatesInOut) {

            var predicateSet = new HashSet<string>();
            var counter = new Dictionary<string, int>();
            var events = new LinkedList<Event>();

            Event ev;
            while ((ev = eventStream.Read()) != null) {
                events.AddLast(ev);
                Update(ev.Context, predicateSet, counter, cutoff);
            }
            predCounts = new int[predicateSet.Count];

            int index = 0;
            for (var e = predicateSet.GetEnumerator(); e.MoveNext(); index++) {
                if (e.Current != null) {
                    predCounts[index] = counter[e.Current];
                    predicatesInOut[e.Current] = index;
                }
            }

            return events;
        }

        #endregion

        #region . Index .

        protected virtual List<ComparableEvent> Index(LinkedList<Event> events, Dictionary<string, int> predicateIndex) {
            var map = new Dictionary<string, int>();

            int numEvents = events.Count;
            int outcomeCount = 0;
           
            var eventsToCompare = new List<ComparableEvent>();
            var indexedContext = new List<int>();

            for (int eventIndex = 0; eventIndex < numEvents; eventIndex++) {
                var ev = events.First.Value;
                events.RemoveFirst();

                int ocID;

                if (map.ContainsKey(ev.Outcome)) {
                    ocID = map[ev.Outcome];
                } else {
                    ocID = outcomeCount++;
                    map[ev.Outcome] = ocID;
                }

                foreach (var pred in ev.Context) {
                    if (predicateIndex.ContainsKey(pred)) {
                        indexedContext.Add(predicateIndex[pred]);
                    }
                }

                // drop events with no active features
                if (indexedContext.Count > 0) {
                    eventsToCompare.Add(new ComparableEvent(ocID, indexedContext.ToArray()));
                } else {
                    Console.Error.WriteLine("Dropped event {0}:{1}", ev.Outcome, ev.Context.ToDisplay());
                }

                indexedContext.Clear();
            }

            outcomeLabels = ToIndexedStringArray(map);
            predLabels = ToIndexedStringArray(predicateIndex);
            return eventsToCompare;
        }

        #endregion

    }
}