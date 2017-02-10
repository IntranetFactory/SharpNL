using System.Collections.Generic;

namespace ModelGeneratorW.Models
{
    class IntentFileInfo
    {
        /// <summary>
        /// Id - Guid that will be passed to IntentOutputObject.Id object
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// IntentName - When reading files with simple intents IntentName = Filename
        /// When reading files with InlineId intents IntentName doesn't make sense
        /// </summary>
        public string IntentName { get; set; }

        /// <summary>
        /// Lines - all lines in the intent file
        /// </summary>
        public List<string> Lines { get; set; }

        public IntentFileInfo()
        {
            Lines = new List<string>();
        }
    }
}
