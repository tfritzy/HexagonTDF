public enum WaveType
{
    // Enemies have normal size, health, and spacing
    Normal,

    // Enemies are large, healthier, and spread farther apart.
    Spread,


    // Enemies are closer together, smaller, and have reduced health
    Clustered,

    // Enemies are clustered together, which much lower health, and periodically a big one spawns in the group.
    ClusteredWithBiggies,
}