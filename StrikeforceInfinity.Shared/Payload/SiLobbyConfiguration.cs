﻿namespace StrikeforceInfinity.Shared.Payload
{
    public class SiLobbyConfiguration
    {
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid OwnerSessionId { get; set; }
        public Guid UID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MaxPlayers { get; set; }

        public SiLobbyConfiguration(string name, int maxPlayers)
        {
            Name = name;
            MaxPlayers = maxPlayers;
        }

        public SiLobbyConfiguration()
        {

        }
    }
}