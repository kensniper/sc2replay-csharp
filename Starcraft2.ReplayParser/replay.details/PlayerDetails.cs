﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerDetails.cs" company="SC2ReplayParser">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Parses the PlayerDetails structure inside the replay.details file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using System.IO;
    using System.Text;

    /// <summary>
    /// Parses the PlayerDetails structure inside the replay.details file.
    /// </summary>
    public class PlayerDetails
    {
        #region Public Methods

        /// <summary> Parses a single PlayerDetail structure from the current position of a BinaryReader object. </summary>
        /// <param name="reader"> The reader, at the position containing the start of the PlayerDetail structure. </param>
        /// <returns> Returns a Player filled with the parsed information. </returns>
        public static Player Parse(BinaryReader reader)
        {
            reader.ReadBytes(2); // playerHeader
            int shortNameLength = KeyValueStruct.Parse(reader).Value;

            byte[] nameBytes = reader.ReadBytes(shortNameLength);
            string shortName = Encoding.UTF8.GetString(nameBytes);

            reader.ReadBytes(3);
            KeyValueStruct.Parse(reader);
            reader.ReadBytes(6);

            var subId = KeyValueStruct.Parse(reader).Value;
            var battlenetId = KeyValueStruct.Parse(reader).Value;

            int raceLength = KeyValueStruct.Parse(reader).Value;

            byte[] raceBytes = reader.ReadBytes(raceLength);
            var race = Encoding.UTF8.GetString(raceBytes);

            reader.ReadBytes(3); // unknown5

            var keys = new KeyValueStruct[9];

            // paramList - This contains the player's color and should eventually be parsed.
            for (int i = 0; i < 9; i++)
            {
                keys[i] = KeyValueStruct.Parse(reader);
            }

            return new Player
                {
                    Name = shortName, 
                    Race = race, 
                    Color =
                        string.Format(
                            "#{0}{1}{2}{3}", 
                            keys[0].Value.ToString("X2"), 
                            keys[1].Value.ToString("X2"), 
                            keys[2].Value.ToString("X2"), 
                            keys[3].Value.ToString("X2")), 
                    Handicap = keys[6].Value, 
                    //// IsWinner = keys[7].Value == 1, -- Incorrect.
                    //// Ignoring the team here because it's unreliable in many replays. Parsed in replay.attributes.events
                    //// Team = keys[8].Value,
                           
                    BattleNetId = battlenetId, 
                    BattleNetSubId = subId, 
                };
        }

        #endregion
    }
}