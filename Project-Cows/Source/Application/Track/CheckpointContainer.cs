﻿// Project Cows -- GearShift Games
// Written by D. Sinclair
// ================
// CheckpointContainer.cs

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Project_Cows.Source.Application.Entity;

namespace Project_Cows.Source.Application.Track {
    class CheckpointContainer {
        // Class to hold the checkpoint 'struct' and the entity
        // ================

        // Variables
        private Checkpoint m_checkpoint;
        private Entity.Entity m_entity;

        // Methods
        public CheckpointContainer(Checkpoint checkpoint_) {
            // CheckpointContainer constructor
            // ================

            m_checkpoint = checkpoint_;
        }

        public CheckpointContainer(Checkpoint checkpoint_, Entity.Entity entity_) {
            // CheckpointContainer constructor
            // ================

            m_checkpoint = checkpoint_;
            m_entity = entity_;
        }


        // Getters
        public Checkpoint GetCheckpoint() {
            return m_checkpoint;
        }

        public Entity.Entity GetEntity() {
            return m_entity;
        }

        // Setters
        public void SetCheckpoint(int id_, int nextID_, int pathID_, CheckpointType type_) {
            m_checkpoint = new Checkpoint(id_, nextID_, pathID_, type_);
        }

        public void SetEntity(ContentManager content_, Texture2D texture_, Vector2 position_, float rotation_) {
            m_entity = new Entity.Entity(content_, texture_, position_, rotation_);
        }
    }
}
