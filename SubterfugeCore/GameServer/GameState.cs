﻿using SubterfugeCore.Entities;
using SubterfugeCore.Entities.Base;
using SubterfugeCore.GameEvents;
using SubterfugeCore.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Xna.Framework;

namespace SubterfugeCore
{
    /// <summary>
    /// This class holds information about a game's current state.
    /// </summary>
    public class GameState
    {

        private Queue<GameEvent> pastEventQueue = new Queue<GameEvent>();
        private Queue<GameEvent> futureEventQueue = new Queue<GameEvent>();
        private List<GameObject> activeSubs = new List<GameObject>();
        private List<GameObject> outposts = new List<GameObject>();
        private GameTick currentTick;
        private GameTick startTime;

        public GameState()
        {
            this.currentTick = new GameTick(new DateTime(), 0);
            activeSubs.Add(new Sub(new Vector2(0, 0), new Vector2(1, 1)));
            outposts.Add(new Outpost(new Vector2(100, 100)));
            outposts.Add(new Outpost(new Vector2(800, 800)));
        }

        public void interpolateTick(int tick)
        {
            GameTick interpolateTick = new GameTick(new DateTime(), tick);
            if(interpolateTick > currentTick)
            {
                while (futureEventQueue.Peek().getTick() < interpolateTick)
                {
                    // Move commands from the future to the past
                    pastEventQueue.Enqueue(futureEventQueue.Dequeue());
                }
            } else
            {
                while (pastEventQueue.Peek().getTick() > interpolateTick)
                {
                    // Move commands from the past to the future
                    futureEventQueue.Enqueue(pastEventQueue.Dequeue());
                }
            }
        }

        public List<GameObject> getSubList()
        {
            return this.activeSubs;
        }

        public List<GameObject> getOutposts()
        {
            return this.outposts;
        }

        public void update()
        {
            foreach(GameObject gameObject in this.activeSubs)
            {
                gameObject.setPosition(new Vector2(gameObject.getPosition().X + 1, gameObject.getPosition().Y + 1));
            }
        }
    }
}
