# AI Squad Pathfinding & Behavior System (Unity)

Technical Unity project focused on AI squad behavior, pathfinding and decision-making logic.

## Overview
This project simulates two opposing teams composed of a general unit and multiple pawns.
The generals are controlled via mouse input, while pawns follow and react dynamically
based on combat conditions, health, tower status and leadership state.

This is a technical/academic project, not a full game.

## Core Systems
- Click-based general movement (left click / right click per team)
- Pawn following behavior using pathfinding
- Dynamic decision-making based on health and environment
- Tower-based healing and fallback logic
- Leadership-based behavior changes

## AI Behaviors
- Pawns follow their general during normal conditions
- If health is low and tower is alive, pawns retreat to heal
- After healing, pawns return to their general
- If the tower is destroyed, pawns fight alongside their general until death
- If the general dies:
  - Pawns defend the tower if alive
  - Otherwise, they disperse to low-visibility map positions

## Tech
- Unity 2022.3.5f1
- C#
- Pathfinding
- Finite State Machine (FSM)
- OOP principles

## Controls
- Left Click: Move Team A General
- Right Click: Move Team B General

## Notes
This project focuses purely on AI logic and behavior systems.
Visuals and combat are intentionally minimal.

