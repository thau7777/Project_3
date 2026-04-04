=== MysteriousHunter ===
{MysteriousHunterCompletedFirstMeet == false:
Hello my friend, it seems you're new here. #speaker:Captain of the brave warriors #voice:CaptainFirstMeet
So I think you should get used to some things here. #speaker:Captain of the brave warriors #voice:CaptainFirstMeet
~ MysteriousHunterCompletedFirstMeet = true
}
I have a little challenge for you. #speaker:Captain of the brave warriors #voice:CaptainDefault
Overcome it, and I will reward you handsomely. #speaker:Captain of the brave warriors #voice:CaptainDefault
* [Accept the challenge]
~ TriggerMiniGame()
Haha, well done, my friend. Keep trying! #speaker:Captain of the brave warriors #voice:CaptainAcceptChallenge
-> MysteriousHunter_challenge_end
* [Refuse the challenge]
It seems you're still not confident enough. #speaker:Captain of the brave warriors #voice:CaptainRefuseChallenge
Keep practicing, and I'll challenge you again next time. #speaker:Captain of the brave warriors #voice:CaptainRefuseChallenge
-> END

=== MysteriousHunter_challenge_end ===
Good job, my friend. Here's a small reward for your efforts. #speaker:Captain of the brave warriors #voice:CaptainAcceptChallenge
Hopefully, these runes will be helpful on your journey. #speaker:Captain of the brave warriors #voice:CaptainAcceptChallenge
->END