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
        
        private static const int MIN_RELEASE_DIFFICULTY = 10;
        private static const int MIN_RELEASE_DIFFICULTY_AS_BYTE = 17;

        private static const int MIN_RELEASE_HAIR_DYE = 82;

        private static const int MIN_RELEASE_HIDE_VISUAL = 83;

        private static const int MIN_RELEASE_BINARY_GENDER = 17;

        #endregion

        public static bool IsDifficultySupported(int release)
        {
            return release >= MIN_RELEASE_DIFFICULTY;
        }

        public static bool IsDifficultyAByte(int release)
        {
            return release >= MIN_RELEASE_DIFFICULTY_AS_BYTE;
        }

        public static bool IsHairDyeSupported(int release)
        {
            return release >= MIN_RELEASE_HAIR_DYE;
        }

        public static bool IsHideVisualSupported(int release)
        {
            return release >= MIN_RELEASE_HIDE_VISUAL;
        }

        public static Boolean IsBinaryGenderSupported(int release)
        {
            return release >= MIN_RELEASE_BINARY_GENDER;
        }
    }
}
