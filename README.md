# The Minesweeper Kata in C#

See the [full description of the Kata](https://codingdojo.org/kata/Minesweeper/).

# Thoughts on learning C#

How do you learn a language? Looking back at those I've learnt, I can see
there's been a mixture of:

1) opening code, tinkering with it, asking for help on forums, Just-In-Time doc searches
		(Visual Basic, JavaScript, C++, Clojure, Python, Ruby, PHP, Inform, PureScript, C#)
2) swallowing a textbook (or these days, learning videos)
		(Perl, C, Java, SQL, HTML, CSS, Lisp, Haskell)
3) learning on the job, with an experienced colleague to guide the way
		(XQuery, Cobol)

That's a simplification of course. All roads eventually end in #1, and in many
cases I've read great books (_JavaScript: The Good Parts_, _The Joy of Clojure_,
even _Mastering Cobol_...) early in my learning to help really get my head around the
subject.

Reading a book is a wonderful way to learn a language. I realise you *should* do
the exercises, and I've genuinely made an effort with for example K&R C and others,
but I rarely get more than half-way. Mostly, I like to read, uninterrupted, as if
the book were a novel...

This has several notable disadvantages, and I'm sure I shouldn't recommend it as
a general approach: you don't learn any muscle memory, and you don't get a
practical sense of what programming in the language feels like, or those lessons
that you inevitably learn only in that moment when code hits the compiler.

*But* you do get a feel for the story of the language, its idioms and
traditions. Often I feel much more productive when I eventually get going, and
don't need to stop as frequently to scratch my head/search the docs/beg for help
etc.

I've found though, that it only really works for "new" languages/idioms. Once
you know Perl, it's hard to read an introductory book on Python. Same for
Haskell -> Clojure, and of course Java -> C#.

If you're learning a language for a job, it's easy: learn what you need to do
to fix the bug/implement the feature you're working on. Otherwise a toy project
is a great motivator. I've usually got an idea that I want to play with, so this
is easy enough to manage, but these projects do tend to spiral out of control if
you let them, so a readymade *Code Kata* is a great way to get started. I've wasted
many hours on Minesweeper back when it was the only game on everyone's computer,
so why not do something useful with it now?

# Getting started

Hillel Wayne has a great piece on [The Hard Part of Learning a Language](https://hillelwayne.com/post/learning-a-language/)
Even with a well-documented language (and C# has all of Microsoft's formidable
documentation team resources behind it) raises so many questions:

* *Can* I install it? I'm on a Mac. Oh I see, dotNet Core is platform agnostic...
* But do I need Visual Studio? Ah no, VS Code should do.
* What's the best way to configure my editor.
* What should I do in the IDE/editor, and what from the `dotnet` command-line app?
* Which test framework should I use?
* How do you structure a project? (I was confused by having to create separate `src/` and `test/` projects for example.)
* Woah, the source code conventions in examples have a very expansive braces style, will I be shunned if I commit code in K&R style instead? (I've gone with the standard.)

All of these things slow you down. I think you can budget a day of what feels like unproductive swearing, experimentation, reading docs, and asking for help on Slack. But I don't think it's unproductive... after all, the next day, coming at it fresh, I managed to get started properly, write a few tests, and feel generally self-sufficient.

After all, if I'd wanted to know all the answers before I started coding, I'd have gone with option #2 and swallowed a book first, right?

(Option #3 is probably the fastest way to hit the ground running: an opinionated guru/team to help you get started. It might not be the *best* setup, but at least you don't have to engage with any soul-searching about optimizing it, which reduces cognitive load just when you don't want it.)

# First impressions

Not loving the very whitespace-y brace style. I can see the benefits in terms of making structure clear though.

Otherwise, it's rather nice. It's good to have code completion, though I'm not finding the IDE side of things as impressive as e.g. IntelliJ so far (or even the VSCode plugin for PureScript - the other main language I'm learnin at the moment.)

The language is fine and mostly unsurprising for its DNA in C and Java. Linq (which I've wanted to play with for ages) seems very well designed. This mix of OO language with a powerful data structure manipulation library reminds me a lot of the experience coding in Python.

Obviously, coming to the language new, I'm googling for *everything* I can't immediately autocomplete. What's nice is that C# is well documented and you can usually find the docs on Microsoft's pages or failing that, StackOverflow.

Despite the frequency of lookups, the speed I could find an answer and move on meant that it didn't disturb my flow too much. Also, the *familiarity* of C# is a benefit. I know how classes and methods and datatypes work. The syntax is usually pretty similar to Java or others. I understand the naming of functions and their return values.

(This is very different from my current experience learning PureScript, where even simple tasks -- constructing a `Time` value -- can require looking at several doc pages and possibly expert assistance, while more complicated things -- accessing nested data structures -- lead you down rabbit holes like Lenses... I'm enjoying that experience too, don't get me wrong, but it's definitely not as "smooth".)

Some things are harder to do ad hoc lookups for: I should actually learn Generics, for example, rather than trying to figure out how to write a Higher Order function zipping two grids together by skimming a few SO questions.
(They are very similar to Haskell's type signatures, but you have to introduce the type variable, as with PureScript `forall`.)

In general it all holds together, even coming from a preference for FP on the one hand and more Dynamic/lightweight OO languages on the other. The only unfortunate missing feature I think I've bounced against is a lack of first-class Array literals.

# The Algorithm

Think of the grid as a 2D bitmask, with an empty space as 0 and mine as 1. So given the example grid:

```
*...
....
.*..
....
```
we want to expand the bitmask in each of the 8 directions (cardinal and diagonal.)
The easiest way to do this is to shift the whole grid in each of those directions
and then overlay the masks together with `OR`. e.g.

```
orig    left    right          full
*...    ....    .*..   etc. =  **..
....    ....    ....           ***.
.*..    *...    ..*.           ***.
....    ....    ....           ***.
```
That's not quite right though! We don't just want a mask of all areas that are a mine
(or adjacent to one.) Rather, we want a count of the number of adjacent mines.

So instead of overlaying the masks together with `OR`, we need to SUM them together.
This results in
```
1100
2210
1110
1110
```
We do the overlay of multiple grids with a `reduce` (which is spelt `IEnumerable.Aggregate`
in C# with Linq.)

And we need one final transformation to reintroduce the mines if they existed in the original
grid picture.

All of these transformations over a grid are done by zipping over the rows and then over
each row in turn, which I've encapsulated in a Higher Order Function called `ZipOverGrid`.
(This is the reason I ended up having to figure out Just Enough Generics to handle 3 type
variables and a function delegate...)

# What's next?

* Missing parts of Kata spec
  * Implement console command
  * Read input file
  * Parse command and grid picture
  * Output
* improve C# idioms
  * all the `.ToArray()` calls are clunky. Should I be using/passing a different structure, or handling converions differently?
  * best practice for handling method preconditions (as in the `Grid()` constructor?
  * ...
  * what else am I missing? Comments welcome!

