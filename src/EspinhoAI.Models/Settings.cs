using System;
namespace EspinhoAI.Models
{
    public class Settings
    {
        public int KeyOne { get; set; }
        public AzureSettings Azure { get; set; } = null!;
    }

    public class AzureSettings
    {
        public string? Endpoint { get; set; } = null;
        public string? Key { get; set; } = null;
    }
}

