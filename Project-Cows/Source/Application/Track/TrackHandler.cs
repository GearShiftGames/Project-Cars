﻿/// Project: Cow Racing
/// Developed by GearShift Games, 2015-2016
///     D. Sinclair
///     N. Headley
///     D. Divers
///     C. Fleming
///     C. Tekpinar
///     D. McNally
///     G. Annandale
///     R. Ferguson
/// ================
/// TrackHandler.cs

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Project_Cows.Source.Application.Entity;
using Project_Cows.Source.Application.Physics;
using Project_Cows.Source.System;
using Project_Cows.Source.System.Graphics;

namespace Project_Cows.Source.Application.Track {
    class TrackHandler {
        // Class to handle any aspects of the track
        // ================

        // Variables
        public List<CheckpointContainer> m_checkpoints = new List<CheckpointContainer>();
        public List<EntityStruct> m_vehicles = new List<EntityStruct>();
        public List<Barrier> m_barriers = new List<Barrier>();      // TEMP
        private List<int> m_rankings = new List<int>();

        private World fs_world;

        // Methods
        public TrackHandler() {
            // TrackHandler constructor
            // ================
        }

        public void Initialise(World world_) {
            fs_world = world_;

            m_checkpoints.Clear();
            m_vehicles.Clear();
            m_barriers.Clear();
            m_rankings.Clear();

            // Add checkpoints
            Level.LoadLevel("0");       // NOTE: This would be done in the in-game state in future -Dean
            m_checkpoints = Level.GetCheckpoints();
            // Add entities to the checkpoints
            foreach (CheckpointContainer cc in m_checkpoints) {
                if (cc.GetCheckpoint().GetType() == CheckpointType.FIRST) {
                    cc.SetEntity(fs_world, TextureHandler.m_gameFinishLine, cc.GetCheckpoint().GetRotation());
                } else {
                    cc.SetEntity(fs_world, TextureHandler.m_debugCheckpoint, cc.GetCheckpoint().GetRotation());
                }
            }

            // Add vehicles
            m_vehicles = Level.GetVehicles();

            // Add barriers
            List<EntityStruct> m_barrierEntityStructs = new List<EntityStruct>();
            m_barrierEntityStructs = Level.GetBarriers();
            // Add entities to Barriers
            foreach (EntityStruct es in m_barrierEntityStructs) {
                m_barriers.Add(new Barrier(fs_world, TextureHandler.m_gameBarrier, es));
            }
        }

        public void Update(List<Player> players_){
            Debug.AddText(new DebugText("Checkpoints:" + m_checkpoints.Count(), new Vector2(20, 500)));      // TEMP
            Debug.AddText(new DebugText("Players:" + players_.Count(), new Vector2(20, 520)));               // TEMP

            if (players_[0].GetVehicle().m_vehicleBody.GetBody().ContactList != null) {
                if (players_[0].GetVehicle().m_vehicleBody.GetBody().ContactList.Next != null) {
                    Debug.AddText(players_[0].GetVehicle().m_vehicleBody.GetBody().ContactList.Next.ToString(), new Vector2(500, 600));
                }
            }
            // Loop for each checkpoint
            foreach (CheckpointContainer cc in m_checkpoints) {
                // Loop for each player
                foreach (Player p in players_) {
                    // Check if player and checkpoint are colliding
                    if (AreBodiesColliding(cc.GetEntity().GetBody(), p.GetVehicle().m_vehicleBody.GetBody())) {
                        // If colliding checkpoint is the next checkpoint, change checkpoint
                        if (cc.GetCheckpoint().GetID() == p.m_currentCheckpoint.GetNextID()) {
                            p.m_currentCheckpoint = cc.GetCheckpoint();
                            
                            // If checkpoint is the first checkpoint, increment lap
                            if (p.m_currentCheckpoint.GetType() == CheckpointType.FIRST) {
                                p.m_currentLap++;
                            }
                        }
                    }
                }
            }

            // NOTE: This *may* run into problems with multiple collisions with different players,
            //       will need to do some testing to see how it works -Dean

            // AYE, IT DID

            // Draw debug info
            foreach (Player p in players_) {
                Debug.AddText(new DebugText("Player " + p.GetID(), new Vector2(20.0f + 150 * p.GetID(), 70.0f)));
                Debug.AddText(new DebugText("Lap: " + p.m_currentLap.ToString(), new Vector2(20.0f + 150 * p.GetID(), 90.0f)));
                Debug.AddText(new DebugText("Checkpoint: " + p.m_currentCheckpoint.GetID().ToString(), new Vector2(20.0f + 150 * p.GetID(), 110.0f)));
                Debug.AddText(new DebugText("Path: " + p.m_currentCheckpoint.GetPath().ToString(), new Vector2(20.0f + 150 * p.GetID(), 130.0f)));
            }

            // Get rankings
            m_rankings.Clear();
            while (m_rankings.Count != players_.Count) {
                int highestID = 0;
                int highestScore = 0;

                // Check for highest score (i.e. front-most non-ranked player)
                foreach(Player p in players_) {
                    int checkpointScore = p.m_currentLap * (m_checkpoints.Count - 1) + p.m_currentCheckpoint.GetID();
                    if (checkpointScore > highestScore) {
                        bool ranked = false;
                        foreach (int i in m_rankings) {
                            if (p.GetID() == i) {
                                ranked = true;
                            }
                        }

                        if (!ranked) {
                            highestScore = checkpointScore;
                            highestID = p.GetID();
                        }
                    }
                }
                // Add front-most player to rankings
                m_rankings.Add(highestID);
            }

            foreach (Barrier b in m_barriers) {
                b.UpdateSprites();
            }
        }

        public void Draw() {
            // Draw the objects to the screen
            // ================

            // Render barriers
            foreach (Barrier b in m_barriers) {
                GraphicsHandler.DrawSprite(b.GetSprite());
            }

            // Add checkpoints to Debug screen
            foreach (CheckpointContainer cc in m_checkpoints) {
                if (cc.GetCheckpoint().GetType() == CheckpointType.FIRST) {
                    GraphicsHandler.DrawSprite(cc.GetEntity().GetSprite());
                } else {
                    Debug.AddSprite(cc.GetEntity().GetSprite());                    
                }
            }

            // Render ranking text
            if (m_rankings.Count != 0) {
                Debug.AddText(new DebugText("1st - Player " + m_rankings[0].ToString(), new Vector2(1500f, 50f)));
                if (m_rankings.Count > 1) {
                    Debug.AddText(new DebugText("2nd - Player " + m_rankings[1].ToString(), new Vector2(1500f, 70f)));
                    if (m_rankings.Count > 2) {
                        Debug.AddText(new DebugText("3rd - Player" + m_rankings[2].ToString(), new Vector2(1500f, 90f)));
                        if (m_rankings.Count > 3) {
                            Debug.AddText(new DebugText("4th - Player" + m_rankings[3].ToString(), new Vector2(1500f, 110f)));
                        }
                    }  
                }
            }
        }

        private bool AreBodiesColliding(Body bodyA_, Body bodyB_){
            // If either of the bodies are null
            if (bodyA_ == null || bodyB_ == null) {
                return false;
            }

            // If either of the bodies have no contact list
            if (bodyA_.ContactList == null || bodyB_.ContactList == null) {
                return false;
            }
            ContactEdge ce = bodyA_.ContactList;

            
                // For each contact
                while (ce != null) {
                    // If the two bodies are colliding (AABB)
                    if (ce.Other == bodyB_) {
                        Contact c = ce.Contact;
                        if (c.IsTouching && c.Enabled) {
                            // If the two bodies are physically touching
                            return true;
                        }
                    }
                    ce = ce.Next;
                }
                
            
             
                    // If the two bodies are not colliding
                    return false;
                
        }
        // Getters


        // Setters


    }
}
