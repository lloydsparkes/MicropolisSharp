/// <summary>
/// From micropolis.h
/// </summary>

namespace MicropolisSharp.Types
{
    public enum CityVotingProblems
    {
        Crime,                    ///< Crime
        Pollution,                ///< Pollution
        Housing,                  ///< Housing
        Taxes,                    ///< Taxes
        Traffic,                  ///< Traffic
        Unemployment,             ///< Unemployment
        Fire,                     ///< Fire

        Count,              ///< Number of problems

        CountOfProblemsToComplainAbout = 4,   ///< Number of problems to complain about.

        NumberOfProblems = 10,
    }
}
