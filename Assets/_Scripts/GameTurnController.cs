using UnityEngine;
using System.Collections;

public class GameTurnController : MonoBehaviour {

    public enum PlayerState {
        Tutorial,
        //PlayerTurn is when the Player can do all of their moves. Torches are burning down and enemies will not move
        PlayerTurn,
        //Combat is when the Player has entered combat, and it is their turn to use an ability. Enemies will wait during this state
        Combat,
        //CommandChain is when the commands that the Player has saved is executing. Player can't do anything else while
        // they are being executed, Enemies will not do anything either, and only a faulty command (like walking into a wall)
        // will cancel this state before the function is done
        CommandChain,
        Moving,
        //EnemyTurn is when the enemy is moving around in the dungeon. Player can't do anything while that happens.
        EnemyTurn,
        //EnemyCombatTurn is while in combat, when the enemy is doing their moves. Player can't do anything.
        EnemyCombatTurn
    }

    public PlayerState CurrentState;
}
