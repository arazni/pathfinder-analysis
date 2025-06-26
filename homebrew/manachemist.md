# Manachemist

**Key Attribute: Intelligence**  
At 1st level, your class gives you an attribute boost to Intelligence.

**Hit Points: 8 plus your Constitution modifier**  
You increase your maximum number of HP by this number at 1st level and every level thereafter.

**Perception**  
Trained in Perception

**Saving Throws**  
Expert in Fortitude  
Trained in Reflex  
Expert in Will

**Skills**
Trained in Arcana  
Trained in a number of additional skills equal to 3 plus your Intelligence modifier  

**Attacks**  
Trained in simple weapons  
Trained in unarmed attacks

**Defenses**  
Trained in light armor  
Trained in unarmored defense

**Class DC**  
Trained in Manachemist class DC

## Manachemistry Elements

A manachemistry molecule is comprised of several manachemistry elements, which are made by manachemists and consumed in arcane reactions known as manaburns.

Start:

- Shard and one more Material Element
- Invade and one more Target Elements

Learn:

- Another Material Element
  - Level 5
  - Level 11
- Another Target Element
  - Level 3
  - Level 9
  - Level 15
- Empower at level 1
- Scale at level 3

### Understanding Elements

- Each action you spend allows you to add two manachemistry elements to a molecule.
- A molecule must contain all required elements in order to Ignite Manaburn:
  - A Material element
  - A Target element
- Exclusionary elements cannot be combined with other elements of their set.
  - e.g. A molecule cannot contain two material elements.
- Double elements can be in a molecule twice, activating their effect a second time.
- Multi elements can be in a molecule multiple times.
  - Multi elements will specify how they handle multiples being in a molecule.
- Focus elements
  - Cost 1 focus point to include in a molecule
  - A molecule cannot have 2 focus elements in it
  - You have as many focus points as focus elements you can use, up to the maximum of 3

#### Material Element (Exclusionary) (Required)

- Shard
  - Feature 1
  - Defense: AC
  - Attack roll
  - Attack trait
  - Base damage die: 1d6
  - Heighten: Level (+4) +1 dice
  - Piercing or slashing damage
- Ether
  - Feature 1
  - Defense: basic will save
  - Base damage die: 1d4
  - Heighten: Level (+4) +1 dice
  - Mental or spirit damage
- Plasma
  - Feature 1
  - Defense: basic reflex save
  - Base damage die: 1d4
  - Heighten: Level (+4) +1 dice
  - Fire or electric damage
- Body
  - Feature 1
  - Defense: basic fortitude save
  - Base damage die: 1d4
  - Heighten: Level (+4) +1 dice
  - Bludgeoning or cold damage
- Soul
  - Feature 1
  - Duration 1 minute
  - Special: When you learn this ability, also gain a level 1 Material Amplifying Element feat that requires the Soul element

#### Target Element (Exclusionary) (Required)

- Invade
  - Feature 1
  - Target: 1 creature
  - Range: 30ft
  - Heighten: Level (+2) Range increases by 5ft.
- Onward
  - Feature 1
  - Area: 20ft line
  - Heighten: Level (+2) Range increases by 5ft.
- Blast
  - Feature 1
  - Area: 15ft cone
  - Heighten: Level (+2) Range increases by 5ft.
- Burst
  - Feature 1
  - Range: 30ft
  - Area: 5ft burst
  - Heighten: Level (+3) Range and Area increases by 5ft.
- Radiate
  - Feature 1
  - Area: 5ft emanation centered on self
  - The manaburn gains the aura trait
  - Heighten: Level (+3) Range increases by 5ft.
  - Assumes larger creatures have larger emanations

#### Target Amplifying Element

- Split (Multi)
  - Feat 1
  - Base: Invade
  - Target an additional creature for each Split element in your molecule.
- Expand (Multi)
  - Feat 1
  - The Area is multiplied by 1 plus the number of Expand elements in your molecule.
- Reach (Multi)
  - Feat 1
  - The Range is multiplied by 1 plus the number of Reach elements in your molecule.
- Careful
  - Feat 4
  - Only enemies can be targets
- Friendly
  - Feat 4
  - Only allies can be targets
- Chain (Double)
  - Feat 10
  - Pick one creature that did not critically succeed at their save or, if combined with Shard, you did not critically miss:
    - That creature becomes the point of origin of a new Ignite Manaburn
    - This new Ignite Manaburn's molecule is an exact copy with a Chain element removed
    - However, creatures who already saved or were already missed are immune to these subsequent manaburns.
- Switch-Chain (Focus)
  - Feat 16
  - While adding a Switch-Chain element to your molecule, also add one Target element you know, which is inert until triggered.
  - Then, resolve the Switch-Chain element as if it were a Chain element with one difference:
    - the copied molecule replaces its original Target molecule with the Switch-Chain's inert Target element, which is now active.

#### Material Amplifying Element

##### Damage Amps

- Upgrade (Multi)
  - Feat 10
  - Requirement: Material Element must have a base damage die
  - The damage (todo: or healing?) die size upgrades one step for each Upgrade in your molecule, to a maximum of d12
- Improve (Multi)
  - Feat 10
  - Requirement: Material Element must have a base damage die
  - The manaburn deals extra damage equal to the number of damage dice times the number of Improve elements in the molecule
- Scale
  - Feature 3
  - Requirement: Material Element must have a base damage die
  - The Heighten entry in your Material Element changes to Level (+2)
- Empower
  - Feature 1
  - Requirement: Material Element must have a base damage die
  - Double the number of damage dice

##### Buff Amps

- Numb
  - Feat 1
  - Requirement: Soul
  - When you Ignite Manaburn and each time you Sustain the manaburn, targets gain temporary HP equal to your level that lasts for the duration.
- Bless
  - Feat 1
  - Requirement: Soul
  - When you Ignite Manaburn and each time you Sustain the manaburn, targets gain a +1 status bonus to attack rolls for 1 round.
- Skill
  - Feat 1
  - Requirement: Soul
  - When you Ignite Manaburn and each time you Sustain the manaburn, targets gain a +1 status bonus to skill checks for 1 round.
- Expert
  - Feat 8
  - Requirement: Soul
  - Choose a specific non-lore skill
  - When you Ignite Manaburn and each time you Sustain the manaburn, targets have their proficiency in that skill increase by 1 step, to a maximum of master, for 1 round.
  - Additionally, targets count as one additional proficiency higher for the purpose of meeting the minimum requirement to disable complex hazards with that skill.
    - For example, an untrained target would become trained in the skill and be able to attempt to disable a complex hazard that normally requires an expert in the skill.
  - Special: Targeted NPCs gain a +2 untyped bonus to the skill instead, or have a skill bonus equal to their relevant ability modifier plus their level plus 2 if the skill is unlisted.
  - Heighten: Level 17, the maximum changes from master to legendary.
- Ward
  - Feat 6
  - Requirement: Soul
  - When you Ignite Manaburn and each time you Sustain the manaburn, targets gain a +1 status bonus to AC for 1 round.
- Regenerate
  - Feat 10
  - Requirement: Soul
  - When you Ignite Manaburn and each time you Sustain the manaburn, targets gain fast healing equal to half your level for 1 round.
- Treat
  - Feat 6
  - Requirement: Soul
  - When you Ignite Manaburn and each time you Sustain the manaburn, targets have particularly effective assistance when rolling to recover from persistent damage for 1 round.
- Reassure
  - Feat 8
  - Requirement: Soul
  - Targets reduce their frightened condition by 2 at the end of their turn instead of the usual 1.
- Resist
  - Feat 12
  - Requirement: Soul
  - Choose an energy or physical damage type
  - When you Ignite Manaburn and each time you Sustain the manaburn, targets gain resistance to that damage type equal to half your level for 1 round.
- Bypass
  - Feat 14
  - Requirement: Soul
  - Choose an energy or physical damage type
  - When you Ignite Manaburn and each time you Sustain the manaburn, targets' damaging abilities ignore an amount of resistance to that damage type equal to half your level for 1 round.

##### Debuff Amps

- Fear
  - Feat 6
  - Requirement: Ether
  - This effect has the mental and emotion traits
  - Each target that fails its save is frightened 1 or frightened 2 on a critical failure.
- Brain Fog
  - Feat 10
  - Requirement: Ether
  - This effect has the mental trait
  - Each target that fails its save is stupefied 1 or stupefied 2 on a critical failure, for 1 minute.
- Nauseate
  - Feat 12
  - Requirement: Shard
  - Each target hit is sickened 1 or sickened 2 on a critical hit, for up to 1 minute.
- Deepen
  - Feat 12
  - Requirement: Shard
  - Each target that is hit has a single numeric condition worsen by 1, to a maximum of 2
  - Each target that is crit has two numeric conditions worsen by 1, to a maximum of 2
  - For example, a target that is drained 1 could become drained 2.
- Dizzy
  - Feat 6
  - Requirement: Plasma or Ether
  - Special: If combined with Ether, this effect has the mental trait
  - Each target that fails its save is clumsy 1 or clumsy 2 on a critical failure, for 1 minute.
- Weaken
  - Feat 6
  - Requirement: Body or Ether
  - Special: If combined with Ether, this effect has the mental trait
  - Each target that fails its save is enfeebled 1 or enfeebled 2 on a critical failure, for 1 minute.
- Drain
  - Feat 18
  - Requirement: Body or Ether
  - Special: If combined with Ether, this effect has the mental trait
  - Each target that fails its save is drained 1 or drained 2 on a critical failure.
- Tumble
  - Feat 2
  - Requirement: Plasma
  - Each target that critically fails its save is knocked prone
- Grab
  - Feat 12
  - Requirement: Body
  - Each target that fails its save is immobilized until it Escapes against your class DC, or grabbed on a critical failure.

#### Molecule Amplifying Element

- Grow
  - Feat 1
  - Requirement: Soul and a Target element with an area
  - The Area increases by 10ft every time you Sustain the manaburn.
- Compress (Focus)
  - Feat 2
  - When adding this element, add two more elements.
- Terraform
  - Feat 8
  - Requirement: Body, Plasma, or Shard and a Target element with an area
  - Choose 1:
    - All ground terrain in the area becomes difficult terrain
    - All ground terrain in the area is no longer difficult terrain, but this cannot change greater difficult terrain
  - Heighten: Level 14 Greater difficult terrain can be reduced to difficult terrain

### Basic Manachemistry

A manachemist can make Strikes using a Shard-Invade manaburn. However, these Strikes do not have a critical specialization and are not in a weapon group.

For the purpose of allies' feats and abilities:

- Other manaburns that Ignite only the 2 required elements are considered cantrips
- All manaburns that Ignite at least 3 elements are considered spells, but are considered neither cantrips nor slotted
- Manaburns that use a focus point are considered focus spells
