===TheSalesman===
{hasCompletedFirstGame == false:
Oh, it seems another one has come here to offer themselves up to him. #speaker:Salesman
This world is a loop, and it will never end as long as he's still here. #speaker:Salesman
Anyway, I'm just a humble street vendor; what right do I have to judge anyone? #speaker:Salesman
}
My young friend asked me, "What do you want to buy today?" #speaker:Salesman
* [Go to the store]
~ OpenStore()
I hope you find something good here. #speaker:Salesman
-> leave_the_store
* [No, I don't need anything.]
-> leave_the_store

===leave_the_store===
Thank you for visiting, and I wish you good luck on your journey. #speaker:Salesman
-> END
