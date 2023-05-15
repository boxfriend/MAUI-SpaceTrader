using System.ComponentModel.DataAnnotations;

namespace SpaceTrader.Data;

public class AgentRegisterData
{
    [Required,StringLength(14,MinimumLength = 3, ErrorMessage = "Agent Name must be between 3 and 14 characters.")]
    public string AgentName { get; set; }

    [Required]
    public string FactionName { get; set; }

    public static bool ValidateFaction(string FactionName)
    {
        return FactionName.ToUpper() is "COSMIC" or "VOID" or "GALACTIC" or "QUANTUM" or "DOMINION";
    }
}
