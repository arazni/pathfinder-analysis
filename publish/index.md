# Welcome to my Pathfinder Analysis Notebook

A couple years ago, my Google Sheets analyses and charts came to a screeching halt. There comes a point where dozens of charts that depend on thousands of calculated rows of data that themselves depend on thousands of calculated rows that themselves... You get the idea. It was calculations all the way down.

Then I did a bunch of analysis in F# and enjoyed the process of it but burned out on trying to share it with people. Well, now I'm working on actually making it meaningfully shareable! And there's excitement in the webpage updating so smoothly as I work on it (although getting it deployed was a bit of a headache).

## What makes this different from other analyses?

Most other analyses use averages and the GM Core / CRB guidelines for creature creation. I use every creature available on [Archives of Nethys](https://2e.aonprd.com/Creatures.aspx) as of June 21, 2025 and make my programmatic comparisons against all creatures of the same level.

Should I have? Probably not. But a lot of times, in these discussions, people tend to get very side-tracked by debating the nuances and merits of comparing one ability against moderate AC/DC and another against high AC/DC. I also wanted to check how well official Pathfinder creatures stick to their own guidance.

### Mathfinder's DPR critique still applies here

My analyses do compare against every creature as of a certain date, but they are essentially boiling down to averages. Actually, they're boiled down to 20 tiers of results (in honor of our sacred d20), but [Mathfinder's critique of averages and DPR](https://youtu.be/HV0_2nVlAkQ?t=754) still applies here.

You will never cast Fireball or use Vicious Swing against every single creature in the bestiary at once. You will always be in a situation where some subset of actions are optimal (casting fireball against low reflex enemies with weakness to fire), some are totally fine (swinging a greatsword at a creature with some slashing resistance), and some are ill-advised (using a precise finisher against a precision-immune enemy).

That being said, I do plan to do my best to keep this critique in mind. However, the general goal here is to compare things that are generally good to other things that are generally good across classes rather than dig deep into specific optimization niches.

### Rarity and analysis

Originally, I wanted to filter out Unique and Rare creatures in the results, but ultimately most creatures at high levels are already Rare. And while Unique creatures might be, well, unique, they still represent a design space that is part of the Pathfinder experience, especially if you're going through Adventure Paths.

### What about kineticist?

Kineti-who?

Just kidding. I just haven't really planned to do much analyzing of them beyond what already seems clear to me about them.

- Elemental Blast: Usually on par with a weapon Strike without damage boosting class features
  - Examples: Earth and shortsword are d8s, air and shortbow are d6s
- Overflow: Usually about as strong as a focus spell and reaches a spell slot if it is 3 actions and sustain
  - Examples: Retch Rust vs. Pulverizing Cascade, Rock Rampart vs. Wall of Stone
- Non-overflow: Usually a little stronger than a cantrip
  - Examples: Aerial Boomerang (cantrip with better AoE, plus single action follow-up next round)
  - Counter-example: Timber Sentinel
- Overflow 18+ Capstones: High rank spell slots, but limited selection.

## The charts look bad on mobile

Yup. I don't know how to fix it yet, I'm sorry.
