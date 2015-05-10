/// <summary>
/// From micropolis.h
/// </summary>

namespace MicropolisSharp.Types
{
    public enum CityClassification
    {
        Village,     ///< Village
        Town,        ///< Town, > 2000 citizens
        City,        ///< City, > 10000 citizens
        Capital,     ///< Capital, > 50000 citizens
        Metropolis,  ///< Metropolis, > 100000 citizens
        Megalopolis, ///< Megalopolis, > 500000 citizens

        Count,  ///< Number of city classes
    }
}
