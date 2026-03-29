EXTERNAL TradeSigilByRune(rune, sigil_name)
EXTERNAL TradeSigilBySigil(sigil_name, sigil_name)
EXTERNAL OpenStore()
EXTERNAL TriggerMiniGame()
EXTERNAL ChosenSigil(sigil_name)
EXTERNAL UpdateRune(rune)
EXTERNAL BlockEarnRune(number)
EXTERNAL UpdateHealth(health)

VAR hasBloodFang = false
VAR currentRune = 0

VAR MysteriousWitchCompletedFirstMeet = false
VAR TheSalesmanCompletedFirstMeet = false
VAR MysteriousHunterCompletedFirstMeet = false
VAR MysteriousHeroCompletedFistMeet = false

INCLUDE TheSalesman.ink
INCLUDE MysteriousWitch.ink
INCLUDE MysteriousHunter.ink
INCLUDE MysteriousHero.ink
INCLUDE TAE.ink