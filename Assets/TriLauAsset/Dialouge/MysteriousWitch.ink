=== MysteriousWitch ===
{ MysteriousWitchCompletedFirstMeet == false:
Hello there!!! #speaker:Mysterious Witch #voice:WitchFirstMeet
You look like you're new here. #speaker:Mysterious Witch #voice:WitchFirstMeet
~ MysteriousWitchCompletedFirstMeet = true
- else:
It's great to see you again, my fellow traveler. #speaker:Mysterious Witch #voice:WitchDefault
}

{ hasBloodFang == false:
I have some cool stuff for you. Would you like to trade? #speaker:Mysterious Witch #voice:WitchHasntBloodFang
    {currentRune >= 100:
    * [Exchange <color=\#EEC15F>100 runes</color> for <b><color=\#FF001D>Blood Fang</color></b> Sigil.]
    ~ TradeSigilByRune(100, "Blood Fang")
    That's a wise choice. #speaker:Mysterious Witch #voice:WitchChosen
    -> chosen
    - else:
    * [Get <b><color=\#FF001D>Blood Fang</color></b>, but you can't get any more runes in the next <b><color=\#FF001D>5</color></b> combats.]
    ~ ChosenSigil("Blood Fang")
    ~ BlockEarnRune(5)
    That's a wise choice. #speaker:Mysterious Witch #voice:WitchChosen
    -> chosen
    }
- else:
Oh, it seems you own a sigil that's very useful to me. Would you like to trade? #speaker:Mysterious Witch #voice:WitchHasBloodFang
* [Exchange <b><color=\#FF001D>Blood Fang</color></b> for <b><color=\#FF001D>Blood Knight's Oath</color></b> Sigil.]
~ TradeSigilBySigil("Blood Fang", "Blood Knight's Oath")
That's a wise choice. #speaker:Mysterious Witch #voice:WitchChosen
-> chosen
}
* [No, I don't want to exchange.]
-> no_choice   

=== chosen ===
Good luck on your journey. #speaker:Mysterious Witch #voice:WitchChosen
-> END

=== no_choice ===
That's a shame, but I wish you good luck on your journey anyway. #speaker:Mysterious Witch #voice:WitchNoChoice
-> END
