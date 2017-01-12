using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DBFilesClient.NET;
using DBFilesClient.NET.Types;
using DBFilesViewer.Data.Structures;
using DBFilesViewer.Data.Structures.Types;
using DBFilesViewer.UI.Forms;
using DBFilesViewer.Utils;

namespace DBFilesViewer.Data.IO.Files
{
    internal static class DBC
    {
        /// <summary>
        /// Fires when all DB2s have been loaded.
        /// </summary>
        public static event Action OnFilesLoaded;

        /// <summary>
        /// Fires for each DB2 loaded.
        /// </summary>
        public static event Action<string> OnLoadProgress;

        public static List<IStorage> Files { get; } = new List<IStorage>();
        private static Dictionary<Type, List<FieldInfo>> _referenceTree = new Dictionary<Type, List<FieldInfo>>();

        private static Dictionary<uint, List<NpcModelItemSlotDisplayInfoEntry>>
            _cdieXnmsdie = new Dictionary<uint, List<NpcModelItemSlotDisplayInfoEntry>>();

        private static Dictionary<uint, List<ModelFileDataInfo>> _modelXfdi = new Dictionary<uint, List<ModelFileDataInfo>>(); 

        static DBC()
        {
            OnFilesLoaded += () =>
            {
                foreach (var kv in Get<NpcModelItemSlotDisplayInfoEntry>())
                {
                    if (!_cdieXnmsdie.ContainsKey(kv.Value.CreatureDisplayInfoExtra.Key))
                        _cdieXnmsdie[kv.Value.CreatureDisplayInfoExtra.Key] =
                            new List<NpcModelItemSlotDisplayInfoEntry>(12);
                    _cdieXnmsdie[kv.Value.CreatureDisplayInfoExtra.Key].Add(kv.Value);
                }

                // Create a metadata type mapping ModelID -> List<(Component)ModelFileDataEntry>
                Parallel.ForEach(Get<ModelFileDataEntry>(), modelPair =>
                {
                    List<ModelFileDataInfo> store;
                    lock (_modelXfdi)
                    {
                        if (!_modelXfdi.TryGetValue(modelPair.Value.ModelID, out store))
                            _modelXfdi[modelPair.Value.ModelID] = store = new List<ModelFileDataInfo>();
                    }

                    store.AddRange(from componentPair in Get<ComponentModelFileDataEntry>()
                        where componentPair.Key == modelPair.Key
                        select new ModelFileDataInfo
                        {
                            Filters = componentPair.Value,
                            FileDataID = modelPair.Value.Model.Key
                        });
                });
            };
        }

        /// <summary>
        /// Loads local files instead of looking up in CASC system
        /// </summary>
        public static async void InitializeLocal()
        {
            await Task.Factory.StartNew(() =>
            {
                Files.Clear();

                // Find all structures, load files accordingly.
                foreach (
                    var structureType in Assembly.GetExecutingAssembly().GetTypes().Where(typeInfo => typeInfo.IsClass))
                {
                    var attrInfo = structureType.GetCustomAttribute<DBFileNameAttribute>();
                    if (attrInfo == null)
                        continue;
                    var fileName = $@"./dbc/{attrInfo.FileName}.db2";

                    var instanceType = typeof (Storage<>).MakeGenericType(structureType);
                    try
                    {
                        Files.Add((IStorage) Activator.CreateInstance(instanceType, fileName));
                        OnLoadProgress?.Invoke(attrInfo.FileName);

                        foreach (var structureField in structureType.GetFields()
                            .Where(structureField => structureField.FieldType.IsGenericType && structureField.FieldType.GetGenericTypeDefinition() == typeof (ForeignKey<>)))
                        {
                            if (!_referenceTree.ContainsKey(structureType))
                                _referenceTree[structureType] = new List<FieldInfo>(5);

                            _referenceTree[structureType].Add(structureField);
                        }
                    }
                    catch (FileNotFoundException)
                    {
                    }
                }

                OnFilesLoaded?.Invoke();
            });
        }

        /// <summary>
        /// Returns a list of objects describing a tree of foreign keys.
        /// </summary>
        /// <typeparam name="T">The type of the record you are looking for references to.</typeparam>
        /// <param name="key">The ID key of the record you are looking for references to.</param>
        /// <returns></returns>
        public static List<ReferenceInfo> FindReferences<T>(uint key) where T : class, new()
        {
            var foreignKeyInfo = new List<ReferenceInfo>();

            foreach (var refInfo in _referenceTree)
            {
                var referringStore = (IDictionary) Get(refInfo.Key);
                if (referringStore == null)
                    throw new InvalidOperationException();
                var fileName = refInfo.Key.GetCustomAttribute<DBFileNameAttribute>();
                var referringFields = refInfo.Value.Where(fieldInfo => fieldInfo.FieldType.IsConstructedGenericType &&
                    fieldInfo.FieldType.GetGenericArguments()[0] == typeof (T)).ToList();

                if (referringFields.Count == 0)
                    continue;

                var refEnumerator = referringStore.GetEnumerator();
                while (refEnumerator.MoveNext())
                {
                    var refFieldsEnumerator = referringFields.GetEnumerator();
                    while (refFieldsEnumerator.MoveNext())
                    {
                        var referringValue = refFieldsEnumerator.Current.GetValue(refEnumerator.Value) as IObjectType<int>;
                        if (referringValue?.Key != key)
                            continue;

                        foreignKeyInfo.Add(new ReferenceInfo
                        {
                            ReferencingFile = fileName.FileName,
                            ReferencerEntry = (uint)refEnumerator.Key,
                            StoreType = refInfo.Key,
                            ReferencingField = refFieldsEnumerator.Current.Name
                        });
                    }
                }
            }

            return foreignKeyInfo;
        } 

        /// <summary>
        /// Loads DB2 files from the indicated CASC filesystem.
        /// </summary>
        public static async void InitializeFromCASC()
        {
            await Task.Factory.StartNew(() =>
            {
                Files.Clear();

                foreach (
                    var structureType in Assembly.GetExecutingAssembly().GetTypes().Where(typeInfo => typeInfo.IsClass))
                {
                    var attrInfo = structureType.GetCustomAttribute<DBFileNameAttribute>();
                    if (attrInfo == null)
                        continue;

                    var instanceType = typeof (Storage<>).MakeGenericType(structureType);
                    try
                    {
                        using (var blteStream = CASC.Manager.OpenFile($@"DBFilesClient\{attrInfo.FileName}.db2"))
                        {
                            if (blteStream != null)
                            {
                                Files.Add((IStorage) Activator.CreateInstance(instanceType, blteStream));
                                OnLoadProgress?.Invoke(attrInfo.FileName);

                                foreach (var structureField in structureType.GetFields()
                                    .Where(structureField => structureField.FieldType.IsGenericType && structureField.FieldType.GetGenericTypeDefinition() == typeof(ForeignKey<>)))
                                {
                                    if (!_referenceTree.ContainsKey(structureType))
                                        _referenceTree[structureType] = new List<FieldInfo>(5);

                                    _referenceTree[structureType].Add(structureField);
                                }
                            }
                            else
                                Console.WriteLine(@"Unable to find {0} in CASC!", attrInfo.FileName);
                        }
                    }
                    catch (FileNotFoundException)
                    {
                    }
                }

                OnFilesLoaded?.Invoke();
            });
        }

        public static IStorage Get(Type recordType) => Files.First(f => f.RecordType == recordType);
        public static Storage<T> Get<T>() where T : class, new() => (Storage<T>) Get(typeof(T));

        /// <summary>
        /// This helper function returns a list of all equipped
        /// items for a given CreatureDisplayInfoExtra.
        /// </summary>
        /// <param name="creatureDisplayInfoExtraID"></param>
        /// <returns></returns>
        public static List<NpcModelItemSlotDisplayInfoEntry> GetEquippedItems(uint creatureDisplayInfoExtraID)
        {
            List<NpcModelItemSlotDisplayInfoEntry> l;
            _cdieXnmsdie.TryGetValue(creatureDisplayInfoExtraID, out l);
            return l;
        }

        /// <summary>
        /// A utility function to return the proper model file data ID based on filters.
        /// 
        /// ModelFileData.db2 is keyed on file data id, and maps it to a model ID.
        /// ComponentModelFileData.db2 is keyed on file data id as well, and provides filtering mechanism for the client to be
        /// able to select the proper model based off gender, race and class.
        /// </summary>
        /// 
        /// <remarks>
        /// Quoting Simca:
        /// 
        /// ModelFileDataID
        /// * Used internally to reference model files in almost all cases (spell visuals seem
        ///   to go directly to FileDataID in a few places because being consistent is hard)
        /// * Used to bunch many different FileDataIDs together who have the same purpose but
        ///   slightly different characteristics.For example, a helmet item will reference 1
        ///   ModelFileDataID, which in turn will resolve into a different FileDataID for
        ///   every race/gender combination. The characteristics supported in this grouping
        ///   process are: RaceID, SexID, SpecID, and 'Order' (0 = left shoulder, 1 = right
        ///   shoulder, -1 = everything else).
        /// * Implemented by ComponentModelFileData.db2 (details the characteristics) and
        ///   ModelFileData.db2(groups the FileDataIDs by ModelFileDataID).
        /// </remarks>
        /// <param name="modelID"></param>
        /// <param name="gender">Gender of the creature.</param>
        /// <param name="classID">Class of the creature</param>
        /// <param name="raceID">Race of the creature.</param>
        /// <param name="order">Texture order; 0 for the left shoulder, 1 for the right one, and -1 otherwise.</param>
        /// <returns></returns>
        public static uint GetModelFile(uint modelID, Genders gender, Classes classID, Races raceID, int order = -1)
        {
            if (!_modelXfdi.ContainsKey(modelID))
                return 0;

            var possibleEntries = _modelXfdi[modelID]
                .Where(kv => kv.Filters.ClassID == classID
                    && (kv.Filters.RaceID == raceID || kv.Filters.RaceID == Races.None)
                    && (kv.Filters.GenderID == gender || kv.Filters.GenderID == Genders.Neutral)
                    && kv.Filters.Order == order).ToArray();

            return possibleEntries.Length != 1 ? 0 : possibleEntries[0].FileDataID;
        }

        /// <summary>
        /// See <see cref="GetModelFile(uint, Genders, Classes, Races, int)"/>.
        /// </summary>
        /// <param name="modelID"></param>
        /// <param name="displayInfo"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static uint GetModelFile(uint modelID, CreatureDisplayInfoEntry displayInfo, int order = -1)
        {
            if (!_modelXfdi.ContainsKey(modelID) || displayInfo.ExtendedDisplayInfo.Value == null)
                return 0;

            var possibleEntries = _modelXfdi[modelID]
                .Where(kv => kv.Filters.ClassID == displayInfo.ExtendedDisplayInfo.Value.DisplayClassID
                    && (kv.Filters.RaceID == displayInfo.ExtendedDisplayInfo.Value.DisplayRaceID || kv.Filters.RaceID == Races.None)
                    && (kv.Filters.GenderID == displayInfo.Gender || kv.Filters.GenderID == Genders.Neutral)
                    && kv.Filters.Order == order).ToArray();

            return possibleEntries.Length != 1 ? 0 : possibleEntries[0].FileDataID;
        }
    }
}
