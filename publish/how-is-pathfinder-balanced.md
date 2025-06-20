# How does Pathfinder balance damage?

## Assumptions from Synthesizing Designer Comments

- An option is balanced against its performance ceiling, but a character/class is not assumed to always be at its performance ceiling (e.g. having the best spells at all times)--it's assumed to be played well.

- Hyperspecializing in damage (or hyperfocusing on it, as is the case with this guide), is counterable and can lead to lower damage in play.

- Buffs, debuffs, healing, damage mitigation, utility, and range are all important and the game assumes the party can handle all or most of these things.

## Weapons

It's no secret that there are a lot of weapons in this game; there are at least 55 martial ranged weapons and 135 martial melee weapons to consider. This section will investigate the implications of their mechanics.

We'll keep the scope of this to martial weapons, who are the most likely to care heavily about their weapon choice, and take a few things for granted:

- A simple weapon is usually a die size downgrade from a martial weapon
  - Spear's d6 vs. Trident's d8
- An advanced weapon is usually a trait upgrade from a martial weapon
  - Butchering Axe = Greataxe + Shove

Let's start by establishing a baseline set of martial weapons that we can then make comparisons against. We can validate these against Archives of Nethys (AoN) by writing queries in the search bar, but ultimately we can narrow it down to these basic results.

For someone using melee martial weapons, I recommend comparing against the baseline of the longsword or greatsword. Or, you can compare against the bastard sword, which is a d8 in one hand and a d12 in two hands, but otherwise traitless. The shortsword is great for classes looking for weapons that are both agile and finesse.

Melee:
d12 - Greatsword. 2 hands. 1 trait.
d10 - Halberd. 2 hands. Reach + 1 trait.
d8 - Longsword. 1 hand. 1 trait.
d6 - Shortsword. 1 hand. Finesse + Agile + 1 trait. Or Uncommon Reach.
d4 - Almost anything goes.

For ranged martial weapons, I recommend comparing against the baseline of the shortbow, which requires 1+ hands (2 hands to use, but no actions to adjust grip) and is Reload 0.

Ranged:
d10 - Harmona Gun. 2 hands. Reload 1. 150ft. 1 trait.
d8 - Sukgung. 1 hand. Reload 1. 200ft. 1 trait.
d6 - Shortbow. 1+ hands. Reload 0. 60ft. 1/2 traits.
d4 - Almost anything goes.

You'll notice right away that ranged weapons don't reach d12 (backpack weapons with 1-minute reloads excluded).

We'll exclude throwing weapons and combination weapons for now, but they are other ways to exchange damage for versatility, enabling you to throw a normally melee weapon at a fleeing enemy or enabling an investigator to switch to a fatal gun when they know their devised strategem is going to pay off.

### The value of Finesse

Gains:

- Your accuracy with melee and ranged attacks can share the same stat: dexterity.
  - Pairing with a backup shortbow can go a long way to prevent you from being countered by flying enemies and odd terrain features.
- A reduced need to invest in strength.

Costs:

- Your weapon damage die is capped to roughly d6s.
  - d8s require two hands or the advanced Aldori dueling sword.
  - Implies a dependency on class features to boost your damage, some of which can be countered (precision immunity).

Mitigation:

- Oozes with low AC, crit, and precision immunity are still vulnerable to deadly and fatal traits, as well as non-damage crit effects from alchemical bombs.
- Alchemical bombs like ghost charges can assist against precision immune ghosts.
- Still investing in strength can enable a backup greatsword or athletic attacks to debuff your enemies.
- Investing in some magic, like damaging cantrips.
- Plan for alternative support actions when your damage boosting class features are countered.

### The value of Reach

Gains:

- Feat synergy:
  - Reactive Strike, Stand Still, etc. can trigger off of many enemies moving adjacent to you.
  - Gang Up can allow a rogue to provide flanking against many enemies at once.
- Enemies may still need to spend an action moving to reach you.
- Rarely, an action saved not needing to move yourself where a non-reach weapon might.

Costs:

- Almost always 2-handed.
- Capped at d10s.
- 1-handed is almost always d4s, d6s are always uncommon.

### The value of Agile

Gains:

- Higher accuracy on multiple attacks in a turn.
  - Other traits combo well with this (Sweep, Backswing, Forceful, Twin)
- Some feats reduce this penalty even further (Double Slice, Agile Grace, Flurry).

Costs:

- Capped at d6s.
  - Implies a dependency on class features to boost your damage, some of which can be countered (precision immunity).

Mitigation:

- See finesse.

### The value of Range

Gains:

- Can take an action that has impact from safer locations
  - Behind one-sided cover or set up to lean
  - Behind difficult, hazardous, or unreachable terrain
- Can switch targets easily
- Can sometimes stay multiple Strides away from slower creatures
- Can continue attacking while backing off from melee, spreading incoming damage to allies, allowing you to keep fighting

Costs:

- Either reduced damage, a need to reload, or a returning rune (or thrower's bandolier)
- Cannot benefit from flanking (mitigated by melee thrown weapons)

### The value of a Free Hand

Gains:

- Interact actions don't require additional actions to get back to "normal" (regrip 2-handed weapon, redraw offhand weapon)
- Better adaptability to more unexpected situations
- Easier consumable usage
  - Easier to spread consumable usage across 2 turns without interrupting flow
    - Strike-Strike-draw + Strike-Strike-drink vs. Strike-Strike-release-draw + drink-regrip-Strike
  - Stride-draw-feed an elixir to a downed ally
- All attack options that require a free hand are available to you
  - Many feat synergies (Snagging Strike, Combat Grab, Dueling Dance)
  - All athletic maneuvers and Dirty Trick
- Easy access to Battle Medicine

Costs:

- Less damage
  - Sometimes mitigated by the action and feat cost of gaining stances (see: monk)

### Weapon Conclusion

Within the context of weapons, we see the ability to trade some damage for some versatility and vice versa. Dual-wield and two-hand weapons pay for their damage increases with action penalties to engaging in versatile behaviors that require a free hand. That price may manifest as the character going down more often, having weaker turns spent recovering, and/or having someone else in the party sacrifice actions to support.

If we treat longsword as the baseline, it scales from 1d8+8 to 4d8+13 in the hands of a strength martial, with possible 3d6.

## Spells

Spells in particular are somewhat interesting because they don't require any hands or action cost unless you're supplementing with scrolls, staves, and wands. In that way, they typically ignore all of the tradeoffs above. However, they come with their own separate tradeoffs, usually by being a limited resource or by requiring two actions for their usage.

### Cantrips

Gains:

- Infinite usage
- Auto-scaling
- Usually get about 5 if you're a caster, 2 if you're hybrid

Costs:

- Damage scales roughly on par with a martial using a backup weapon
- Usually 2 actions

### Focus Spells

Gains:

- Roughly "per encounter" usage
- Typically up to 3 uses by mid-game
- Auto-scaling
- Strong, ranged single target damage scales roughly on par with ranged martials available to some subclasses
- Strong AoE spells available to very few subclasses

Costs:

- Typically requires feats to gain access
- Usually 2 actions, but some 1-action attack spells

### Slotted Spells

Gains:

- Strong AoE spells
- Strong, ranged single-target damage that competes with melee martials available to some traditions
- Large variety of effects
- Lower ranks are spammable in late-game
- Higher rank AoEs keep the standard damage scaling but are much easier to use and affect much more average

Costs:

- "Daily" usage
- Usually 3-4 per rank per day
- Not auto-scaled
- Usually 2 actions, but some are 1 or 3 actions for different effects

## Traditions generally

**Arcane** can easily target all defenses and do battlefield control, but cannot heal.
**Primal** can heal and do a variety of elemental damage, but struggles to target Will.
**Religion** can heal, buff, debuff, and hit unholy/holy for massive damage, but struggles to target Reflex.
**Occult** can heal, buff, debuff, but struggles to target Reflex and often countered by mental immunity.

## Referenced Principles from the Designers

### Balance options around their performance ceiling, rather than the floors or the middle

[Michael Sayre](https://paizo.com/threads/rzs43kh6?Long-air-repeater#7):
> There's a ton of permutations that could happen, but a weapon should be balanced to the performance ceiling otherwise it isn't balanced at all, and the ceiling here is pretty straightforward- can the player control the engagement distance? Whenever that answer is yes, the [60ft range option] offers strictly better action economy on two different axes, which equates to more damage dealt and less damage taken (there's also a point in time where less damage taken also converts into more damage dealt, that being when the action economy required to keep you alive could be used to continue DPR on the enemy but is instead spent keeping you functioning.)

[Michael Sayre](https://paizo.com/threads/rzs43kh6?Long-air-repeater#12):
> As I mentioned, balance is about ceiling, not floor... Just about everything in PF2 is balanced against two very simple questions
>
> - "What is the most effective thing someone could do with this in a tactical environment?"
>
> - and "What is the most effective thing of this type anyone can do in a tactical environment?"
>
> Those two questions establish your design ceiling. Everything below that is "safe" design space where you can innovate with different toggles and switches, but you should never break that ceiling, otherwise you're introducing power creep.

### The ceiling does not assume you have the perfect spell at all times

[Michael Sayre](https://paizo.com/threads/rzs43vmk&page=2?Michael-Sayre-on-Casters-Balance-and-Wizards#76):
> The game does assume that e.g. your wizard, using those resources, likely has a spell that can affect a low-Will brute and some option on hand for shutting down a troll's regeneration, because both of those functions exist within common cantrips and are otherwise achievable within a single wizard or within any other reasonably balanced party.
>
> The way PF2 is designed assumes that even your "blaster specialist" is capable of targeting 3-4 defenses and at least a few weaknesses, though. The game simply wasn't designed to handle characters who can consistently target something other than enemy's strongest save choosing not to do so. The game knows you have versatility and expects you to use it, but that's not at all the same as expecting a "Schroedinger's Wizard" who simultaneously has all silver bullets at all times.

### There's a rough assumption of one top rank spell slot being used per caster in a moderate encounter

[Michael Sayre](https://paizo.com/threads/rzs43vmk&page=2?Michael-Sayre-on-Casters-Balance-and-Wizards#79):
> Three [moderate+] encounters is basically the assumed baseline, which is why 3 is the default number of spells per level that core casters cap out at. You're generally assumed to be having about 3 [moderate] encounters per day and using 1 top-rank slot per encounter, supplemented by some combination of cantrips, focus spells, consumables, limited-use non-consumables, lower level slots, etc. (exactly what level you are determines what that general assumption might be, since obviously you don't have lower-rank spells that aren't cantrips at 1st level.)
>
> Some classes supplement this with bonus slots, some with better cantrips, some with better access to focus spells, some with particular styles of feats, etc., all kind of depending on the specific class in play. Classes like the psychic and magus aren't even really expected to be reliant on their slots, but to have them available for those situations where the primary play loops represented by their spellstrike and cascade or amps and unleashes don't fit with the encounter they find themselves in, or when they need a big boost of juice to get over the hump in a tough fight.

[Mark Seifter](https://www.reddit.com/r/Pathfinder2e/comments/16g5zpf/comment/k065e07/):
> The game is not balanced around 3 encounters total per day. But it is balanced around the definitions of moderate, severe, and extreme encounters found in the CRB (which if you follow through with them, do imply that it's unlikely for an average group to reliably take many more than 3 moderate+ encounters in a day). If you get too attached to a number of encounters per day, it will never be accurate for your actual situation and it will only make things more confusing.  This is why the encounter building and adventure sections of the CRB and GMG try to explain the interactions between the encounters in the same adventuring day, rather than state a number.

### There's a rough assumption of party capability

[Michael Sayre](https://paizo.com/threads/rzs43vmk&page=2?Michael-Sayre-on-Casters-Balance-and-Wizards#79):
> The game assumes that any given party has roughly the capabilities of a cleric, fighter, rogue, and wizard who are using the full breadth of their capabilities. You can shake that formula by shifting more of a particular type of responsibility onto one character or hyper-specializing the group into a particular tactical spread, but hyper-specialization will always come with the risk that you encounter a situation your specialty just isn't good for, even (perhaps especially) if that trick is focus-fire damage.
