namespace GorillaFaces
{
    public class Constants
    {
        // General

        /// <summary>
        /// The GUID (globally unique identifier) used to identify the mod
        /// </summary>
        public const string GUID = "dev.gorillafaces";

        /// <summary>
        /// The name of the mod
        /// </summary>
        public const string Name = "GorillaFaces";

        /// <summary>
        /// The version of the mod utilizing semantic versioning (major.minor.patch)
        /// </summary>
        public const string Version = "1.0.0";

        // Logic

        /// <summary>
        /// The bottom row of the mouth sheet
        /// </summary>
        public const float BottomRow = -(1f / 3f);

        /// <summary>
        /// The middle row of the mouth sheet
        /// </summary>
        public const float MiddleRow = 0f;

        /// <summary>
        /// The top row of the mouth sheet
        /// </summary>
        public const float TopRow = 1f / 3f;

        /// <summary>
        /// The first column of the mouth sheet
        /// </summary>
        public const float FirstColumn = 0f;

        /// <summary>
        /// The second column of the mouth sheet
        /// </summary>
        public const float SecondColumn = 1f / 4f;

        /// <summary>
        /// The third column of the mouth sheet
        /// </summary>
        public const float ThirdColumn = 1f / 2f;

        /// <summary>
        /// The fourth column of the mouth sheet
        /// </summary>
        public const float FourthColumn = 3f / 4f;

        /// <summary>
        /// How loud a scream from a player is
        /// </summary>
        public const float ScreamVolume = 1f / 5f;

        /// <summary>
        /// How long a scream from a player lasts
        /// </summary>
        public const float ScreamDuration = 1f / 2f;
    }
}
