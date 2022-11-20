using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;


namespace SynNotSoSalty
{
    public class Program
    {
        public static FormKey SaltPile;
        public static ConditionGlobal SaltCondition = new ConditionGlobal();

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "SynNotSoSalty.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            FormKey.TryFactory("034CDF:Skyrim.esm", out SaltPile);
            //IEnumerable<IConstructibleObjectGetter> constructibleObjects = state.LoadOrder.PriorityOrder.ConstructibleObject().WinningOverrides();

            BuildSaltCondition();

            foreach (IConstructibleObjectGetter constructibleObject in state.LoadOrder.PriorityOrder.ConstructibleObject().WinningOverrides())
            {
                int constructibleObjectItemCount = constructibleObject.Items?.Count ?? 0;
                if (constructibleObjectItemCount > 1)
                {
                    for (int i = 0; i < constructibleObjectItemCount; i++)
                    {
                        IContainerEntryGetter item = constructibleObject.Items![i];
                        if (item.Item.Item.FormKey.Equals(SaltPile))
                        {
                            ConstructibleObject newConstructibleObjectOverride = constructibleObject.DeepCopy();

                            newConstructibleObjectOverride.Items!.Remove(newConstructibleObjectOverride.Items[i]);

                            newConstructibleObjectOverride.Conditions.Insert(0, SaltCondition);
                            state.PatchMod.ConstructibleObjects.GetOrAddAsOverride(newConstructibleObjectOverride);

                            break;
                        }
                    }
                }
            }
        }

        public static void BuildSaltCondition()
        {
            FunctionConditionData functionConditionData = new FunctionConditionData();
            functionConditionData.Function = Condition.Function.GetItemCount;
            functionConditionData.ParameterOneRecord.FormKey = SaltPile;
            //functionConditionData.ParameterTwoRecord.FormKey = SaltPile;
            functionConditionData.RunOnType = Condition.RunOnType.Subject;
            functionConditionData.Unknown3 = -1;
            
            SaltCondition.Data = functionConditionData;
            SaltCondition.CompareOperator = CompareOperator.GreaterThan;
        }
    }
}
