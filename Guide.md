# Guide

This guide explains how to use this Tabletop system. It lets a player drag-and-drop cards around a scene. Cards can be placed in random position or in a *deck*. A *deck* is considered a collection of cards in which they can be laid down in different ways (in a stack, in a line, in a circle, etc.)

## Set up

Once the Unity Package is imported into your project, setting up a scene is easy.

The only `GameObject` required is the prefab `Finger` and it is located in *Tabletop* → *Engine*. Add it to the scene.

Make sure you use a camera with orthographic projection.

## Decks

To add a deck to the scene, simply add the `Deck` prefab into it. It is located in *Tabletop* → *Engine*.

A deck needs a *style* to know how to lay down its cards. To create one go to *Assets* → *Create* → *Tabletop* → *Deck Style*. Here you can choose which style you prefer. Edit the values of the newly created Scriptable Object and the assign it to the Deck from the Inspector. Once assigned, Gizmos will show you where cards will be placed.

The number of cards simulated with the Gizmos can be changed from the Inspector. Cards are represented with coloured squares in the Scene View. The blue square represent where the top card will be placed, while the white square represents the bottom one.

Functions and values available for the `TabletopDeck` component:

- `timeForGradually`: Cards can be laid down one after the other. This is the time it waits in between.
- `AddCard(GameObject card, [int index], [bool gradually])`: Add a card. `Index` defines which index it will be: `0` is bottom, `-1` is top, `-2` is second from top and so on. `Gradually` defines if the cards are repositioned instantly or gradually staring from `Index`.
- `AddCards(List<GameObject> cards, [int index], [bool gradually])`: Similar to `AddCard()`, but it add a list of cards.
- `LayDown([int fromIndex],  [bool gradually])`: Force cards to lay down. There should be no need to call this.
- `Shuffle()`

## Deck Styles

There are 2 deck styles available.

They both have scatter values.

The following are some values that may require a more detailed explanation:

- Line
    - *Angle*: cards are laid down in a horizontal line by default. If you need a vertical line or any custom angle change this value.
    - *Alignment*: Think of it as the alignment property in text editors, however, since the line can have any angle the terminology has to be different. Please play around with it in the editor.
    - *Cards Orientation*: Where does the bottom of a card face? *Normal* does not rotate the card; *In* makes it face the center of the deck; *Out* make it face the opposite side to the center of the deck.
- Circle
    - *Alignment*: Given that the starting point of the line of cards is given by the *Radius* and *Start Angle*, *Clockwise* means the clockwise side of that point.
    - *Direction*: Means in which side of the starting point of the line the top card is.
    - *Cards Orientation*: Same as the previous.

Deck Styles are not meant to be changed in real-time. Instead, you should create a new style and assign that to the deck.

## Cards

Cards are generated from a custom Editor Window located in *Window* → *Tabletop* → *Cards Database*.

On the Window, if missing, add the `Card` prefab located in *Tabletop* → *Engine*. Once you want to instantiate your cards in the scene, click *Update Scene* in the top right corner.

Functions and values available for the `TabletopCard` component:

- `faceUp`

Drag and drop cards in Play Mode using the left click. Click *F* to flip a card when dragging or hovering it.

## Smooth animations

It is possible to change values for the smooth animations for all the `GameObject` that have a `SmoothAnimatimations` component attached.
