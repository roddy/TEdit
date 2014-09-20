using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEdit.Data.Util
{
    class VersionUtils
    {
        #region Properties

        /// <summary>
        /// The current terraria release. This is used for saving files.
        /// </summary>
        public const int TERRARIA_CURRENT_RELEASE = 102;

        /// <summary>
        /// Minimum supported release for this tool. This happens to be the release that introduced lookup-by-ID over
        /// lookup-by-name. Coincidence? Nope.
        /// </summary>
        //public const int MIN_SUPPORTED_RELEASE = 38;
        /// Actually for now we're only supporting the current release.
        public const int MIN_SUPPORTED_RELEASE = TERRARIA_CURRENT_RELEASE;

        /// <summary>
        /// Apparently there's a hard-limit for the maximum number of worlds (which we found by looking at the size
        /// of the array of default spawn points in the Players class.) In this partcular version, the max is 200. We've
        /// exposed it here just in case it changes sometime in the future.
        /// </summary>
        public static readonly int MAX_WORLDS = 200;

        private static readonly int MIN_RELEASE_HAIR_DYE = 82;
        private static readonly int MIN_RELEASE_HIDE_VISUAL = 83;

        private static readonly int MIN_RELEASE_16_ITEM_ARMOR = 81;

        private static readonly int MIN_RELEASE_DYE = 47;
        private static readonly int MIN_RELEASE_EXTENDED_DYE = 81;
        private static readonly int INITIAL_DYE_SIZE = 3;
        private static readonly int EXTENDED_DYE_SIZE = 8;

        private static readonly int MIN_RELEASE_EXTENDED_INVENTORY = 58;
        private static readonly int INVENTORY_SIZE_ORIGINAL = 48;
        private static readonly int INVENTORY_SIZE_EXTENDED = 58;

        private static readonly int MIN_RELEASE_EXTENDED_BANK = 58;
        private static readonly int BANK_SIZE = 40;

        private static readonly int MIN_RELEASE_ANGLER = 98;

        private static readonly int BUFF_INCREASE_RELEASE = 75;
        private static readonly int BUFF_COUNT_ORIGINAL = 10;
        private static readonly int BUFF_COUNT_EXTENDED = 22;

        /// <summary>
        /// The magic number to end all magic numbers.
        /// </summary>
        public static readonly int MAX_ITEM_ID = 2749;

        #endregion

        public static bool IsHairDyeSupported(int release)
        {
            return release >= MIN_RELEASE_HAIR_DYE;
        }

        public static bool IsHideVisualSupported(int release)
        {
            return release >= MIN_RELEASE_HIDE_VISUAL;
        }
        
        public static bool Is16PieceArmorSupported(int release)
        {
            return release >= MIN_RELEASE_16_ITEM_ARMOR;
        }

        public static bool IsDyeSupported(int release)
        {
            return release >= MIN_RELEASE_DYE;
        }

        public static bool IsExtendedDyeSupported(int release)
        {
            return release >= MIN_RELEASE_EXTENDED_DYE;
        }

        public static int GetDyeSize(int release)
        {
            if (!IsDyeSupported(release))
            {
                return 0;
            }
            else if (IsExtendedBankSizeSupported(release))
            {
                return EXTENDED_DYE_SIZE;
            }
            else
            {
                return INITIAL_DYE_SIZE;
            }
        }

        public static int GetArmorSize(int release)
        {
            if (VersionUtils.Is16PieceArmorSupported(release))
            {
                return 16;
            }
            else
            {
                return 11;
            }
        }

        public static bool IsExtendedInventorySupported(int release)
        {
            return release >= MIN_RELEASE_EXTENDED_INVENTORY;
        }

        public static int GetInventorySize(int release)
        {
            if (IsExtendedInventorySupported(release))
            {
                return INVENTORY_SIZE_EXTENDED;
            }
            else
            {
                return INVENTORY_SIZE_ORIGINAL;
            }
        }

        public static bool IsExtendedBankSizeSupported(int release)
        {
            return release >= MIN_RELEASE_EXTENDED_BANK;
        }

        public static int GetBankSize(int release)
        {
            return BANK_SIZE;
        }

        public static bool IsAnglerSupported(int release)
        {
            return release >= MIN_RELEASE_ANGLER;
        }

        public static bool IsBuffCountIncreasedInRelease(int release)
        {
            return release >= BUFF_INCREASE_RELEASE;
        }

        public static int GetBuffCount(int release)
        {
            if (IsBuffCountIncreasedInRelease(release))
            {
                return BUFF_COUNT_EXTENDED;
            }
            else
            {
                return BUFF_COUNT_ORIGINAL;
            }
        }
    }
}
