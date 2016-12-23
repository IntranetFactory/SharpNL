using ModelGenerator.Models;
using ModelGenerator.Readers;
using ModelGenerator.Writers;
using System.Collections.Generic;

namespace ModelGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var intentsRelativePath = "model/input/intents";
            IList<Intent> intents = IntentReader.Read(intentsRelativePath);

            var intentOutputFolderRelativePath = "model/output/intents";
            IntentWriter.Write(intents, intentOutputFolderRelativePath);

            var entitiesRelativePath = "model/input/entities";
            IList<Entity> entities = EntityReader.Read(entitiesRelativePath);

            var entitiesOutputFolderRelativePath = "model/output/entities";
            EntityWriter.Write(entities, entitiesOutputFolderRelativePath);
        }
    }
}
