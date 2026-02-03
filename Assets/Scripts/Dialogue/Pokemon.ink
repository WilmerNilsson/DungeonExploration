-> Main

=== Main ===
#name:Prof. Oak #sprite:UI_Blue
Welcome to the wonderful and adventurous world of Pokemon!
Which Pokemon do you choose?
    + [Bulbasaur]
        -> chosen("<color=green>Bulbasaur")
    + [Charmander]
        -> chosen("<color=red>Charmander")
    + [Squirtle]
        -> chosen("<color=blue>Squirtle")
    + [Secret fourth option]
        -> other()
=== chosen(pokemon) ===
#sprite:UI_Green
You chose {pokemon}!
->END

=== other ===
#sprite:UI_Red
You showed up late expecting to get a <color=yellow>Pikachu.
Instead you get nothing.
->END