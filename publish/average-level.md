# Average single-target damage across 20 levels

Pathfinder is a very long game, with experiences that are relatively stable and experiences that vary wildly. A level 1-20, 6 book Adventure Path campaign can easily take 400 hours to complete (3 hours a week for over 2.5 years). There are a ton of character options, from classes and their respective feats or spells, to archetypes, to skill feats and ancestry feats. However, most of these characters are going to spend many of their actions taking a Stride, Casting a Spell, or making a Strike.

So it becomes interesting to analyze how some of these standard spells, weapons, and activities scale from year 1 to year 3 of a game together.

## What's in this chapter?

This chapter provides a DPR analysis of various standard character options across 20 levels that damage a single target. The goal of this chapter is not to powergame or optimize for DPR, rather to see how relatively standard courses of action stack up compared to each other across various classes.

Despite the flaws of DPR analysis, I think this is relatively fair for now, since the choices we are picking are not hyper-specific but very generalized.

The charts in this chapter assume average damage rolls, compare 

This data breaks down into 3 major tiers:

- Cantrips and ranged backup weapons
- Focus spells and range specialized martials
- Spell slots and melee specialized martials

This chapter will focus on quantitatively analyzing competent performance of these tiers of character options across 20 levels.

For our martials, we will generally be consider the Fighter and Barbarian. For our casters, we will primarily consider the Druid and the Sorcerer.

We choose the Fighter and the Barbarian because they are both relatively simple to calculate damage for and are beloved for their low skill floor and high impact. We choose the druid as a stand-in for the average caster with solid focus spell and blasting options, and the sorcerer for the extra 1 damage per spell rank they get on some of their spells (a very consistent way for the class to do just a little bit more damage).

## Base assumptions for this chapter

While we are comparing against the entire bestiary's AC and saves for each level band, we will be making a few assumptions to make things easier.

### We will not concern ourselves with weaknesses, resistances, or immunities

We will assume that a player operating at that tier is doing so on the normal test case. If a player's favored weapons or spells are rendered ineffective, or are resisted, they will likely use an alternative approach. And this alternative approach will likely drop them down to a different tier, such as the cantrip tier.

This is likely the case if an enemy flies out of a sword fighter's reach and they're forced to use a backup shortbow, or if a bard's frustrations with mental immunity force them to rely on [Telekinetic Projectile](https://2e.aonprd.com/Spells.aspx?ID=1718) for a while.

### Um, actually, we will consider immunity to critical hits

Oozes and the like fundamentally change the calculus of abilities very strongly. It would be a bit silly not to handle them. So, for anything that targets AC, we will only consider rider effects like Deadly and Fatal by using their average damage for the level they're at.

### We will apply resistance to spells correctly

Some creatures do have bonuses to saves against magic. We will apply those to saving throws against spells.

### We will assume a maximum offensive attribute modifier

Unless otherwise specified, we assume that a character starts with a +4 in their offensive modifier, increases it to 5 at level 10, increases it through an apex item at level 17, and maximizes it to 7 at level 20.

This example is not always realistic for off-key ability modifier classes, but we're focusing our analysis on straightforward classes. My beloved investigator gets a lot of advantages that do not easily translate to damage or damage charts, for example.

### We will always handle MAP when it applies

We are looking at standard rounds, so if we're using attacks, our subsequent attacks of the round will be dealing with lower accuracy.

### We always assume average damage rolls

Since we're interested in the average scenario and average results over the course of 20 levels, we're not going to break down the impacts of rolling lower or higher than average in this chapter.

### We assume all weapon runes apply as soon as you reach that level (except in the cantrip tier)

In my experience of Blood Lords, there were many times that my monk did not have a fundamental rune for an entire level or two. But for this analysis, we're assuming as soon as we reach the same level as a rune that can be put on our weapon, it's on there.

Additionally, unless otherwise specified, we'll assume a damaging property rune is chosen and applied to a weapon at levels 8, 10, and 16. This adds a total of 3d6 to each weapon's Strike damage.

However, these weapon rune assumptions will be explored first in the cantrip tier of analysis. It will be highlighted in that section.
