=== MysteriousHero ===
{ MysteriousHeroCompletedFistMeet == false:
Hello, I heard there's a new person here, so it turns out it's you! #speaker:Mysterious Hunter
There are so many traps out there. I can't let a newcomer with unknown abilities go out there and throw themselves to their death. #speaker:Mysterious Hunter
~ MysteriousHeroCompletedFistMeet = true
}
I have a challenge: catch a BamBear and bring it back here, and I will acknowledge your abilities. #speaker:Mysterious Hunter
* [Accept the challenge]
~ TriggerMiniGame()
That's great, do your best! #speaker:Mysterious Hunter
-> MysteriousHero_challenge_end
* [Refuse the challenge]
Don't be ashamed for backing down. I understand that in this chaotic world, everyone puts their own life first. #speaker:Mysterious Hunter
But always remember to strive to improve your strength so you can protect yourself. #speaker:Mysterious Hunter
-> END

=== MysteriousHero_challenge_end ===
Well done, brave warrior. This is your reward for your efforts. #speaker:Mysterious Hunter
-> END