// Golden Version
// Main Contributors: Olivia Lazar
namespace ZombieSpace
{
    // Operational state of a level
    public enum LevelState
    {
        Active,     // Running
        Paused,     // Temporarily suspended
        Over,       // Permanently finished
    }

    // Reasons for losing a level
    public enum DefeatType
    {
        PlayerKilled,       // Player was killed
        TargetDestroyed,    // Target pickup was destroyed
    }

    // States of zombie behavior
    public enum ZombieState
    {
        Wander,
        Chase,
        Attack,
        Dead
    }

    // Zombie target that affects movement and attacking
    public enum ZombieTarget
    {
        None,       // Moving randomly
        Player,     // Moves towards player
        Pickup,     // Moves towards pickup
        Ally,       // Moves towards cured human
    }

    // Target pickup state
    public enum TargetState
    {
        Safe,           // Target pickup has no zombies nearby
        Threatened,     // Zombies approaching target pickup
        Attacked,       // Zombies attacking target pickup
        Destroyed,      // Target pickup destroyed
        Collected,      // Target is no longer active in scene
    }

    // Type of weapon
    public enum WeaponType
    {
        None,       // No weapon selected
        Ranged,     // Straight shot, long distance
        Melee,      // Short distance
    }

    // Name of the weapon
    public enum WeaponName
    {
        None,
        Crowbar,
        Bat,
        Sledge,
        Axe,
        Pistol,
        SMG,
        Machine,
        Shotgun,
        Sniper,
    }

    // Scale of weapon strength
    public enum WeaponScale
    {
        Low,
        Mid,
        High,
    }    
}
