using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Menus;
using StardewValley.Inventories;
using System.Collections.Generic;
using System.Linq;

namespace CraftFromContainersMod
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Constructor(
                    typeof(CraftingPage),
                    new[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(bool), typeof(bool), typeof(List<IInventory>) }
                ),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(CraftingPage_Postfix))
            );
        }

        private static void CraftingPage_Postfix(CraftingPage __instance, List<IInventory> materialContainers)
        {
            // Get the current location's chests
            var chests = Game1.currentLocation.Objects.Values.OfType<Chest>();
            List<IInventory> chestInventories = chests.Select(chest => (IInventory)chest.Items).ToList();

            // Combine original containers (if any) with chest inventories
            List<IInventory> combinedContainers = materialContainers?.ToList() ?? new List<IInventory>();
            combinedContainers.AddRange(chestInventories);

            // Update the _materialContainers field using reflection
            var materialContainersField = AccessTools.Field(typeof(CraftingPage), "_materialContainers");
            materialContainersField.SetValue(__instance, combinedContainers);
        }
    }
}