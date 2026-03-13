=== MysteriousWitch ===
{ hasCompletedFirstGame == false:
Hello there!!! #speaker:Mysterious Witch
You look like you're new here. #speaker:Mysterious Witch
- else:
It's great to see you again, my fellow traveler. #speaker:Mysterious Witch
}

{ hasBloodBone == false:
I have some cool stuff for you. Would you like to trade? #speaker:Mysterious Witch
* [Exchange <color=\#EEC15F>100 runes</color> for <b><color=\#FF001D>Bloodborne</color></b> Sigil.]
//~ chosenSigil("bloodborne")
-> chosen
* [No, I don't want to exchange.] 
-> no_choice    
- else:
Oh, it seems you own a sigil that's very useful to me. Would you like to trade? #speaker:Mysterious Witch
* [Exchange <color=\#EEC15F>God Hand</color> for <b><color=\#FF001D>Blood Knight's Oath</color></b> Sigil.]
//~ chosenSigil("Black Knight's Oath")
-> chosen
* [No, I don't want to exchange.]
-> no_choice   
}

=== chosen ===
That's a wise choice. Good luck on your journey. #speaker:Mysterious Witch
-> END

=== no_choice ===
That's a shame, but I wish you good luck on your journey anyway. #speaker:Mysterious Witch
-> END
