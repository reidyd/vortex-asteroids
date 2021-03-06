﻿using asteroids.Components.Powerups;
using Vortex.Scenegraph.Components;
using Vortex.Scenegraph.Components.Gui;

namespace asteroids.Components
{
    public class HudController : ScriptComponent
    {
        public LabelWidgetComponent StatusLabel { get; set; }
        public ImageWidgetComponent ShipHealth { get; set; }
        public LabelWidgetComponent GameOverLabel { get; set; }
        public LabelWidgetComponent ScoreLabel { get; set; }

        private GameDirector _gameDirector;

        private int _displayedScore;

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);

            if (_gameDirector == null)
            {
                _gameDirector = Scene.GetComponent<GameDirector>();
            }

            if (_gameDirector == null)
                return;

            UpdateScore();
            UpdateStatus();
            UpdateShipHealth();
            UpdateGameOverText();
        }

        private void UpdateStatus()
        {
            StatusLabel.Text = GetLabelText();
        }

        private void UpdateShipHealth()
        {
            ShipHealth.HorizontalCrop = GetShipHealthPercentage();
        }

        private void UpdateGameOverText()
        {
            GameOverLabel.Visible = false;
            if (_gameDirector.LivesRemaining < 0)
            {
                GameOverLabel.Visible = true;
                GameOverLabel.Text = "Game over man!";
            }
            else if (_gameDirector.GameCompleted)
            {
                GameOverLabel.Visible = true;
                GameOverLabel.Text = "You're winner!";
            }
        }

        private void UpdateScore()
        {
            EaseDisplayedScore();
            ScoreLabel.Text = string.Format("Score: {0}", _displayedScore);
        }

        private void EaseDisplayedScore()
        {
            if (_gameDirector.Score == 0)
            {
                _displayedScore = 0;
                return;
            }

            var diff = _gameDirector.Score - _displayedScore;

            if (diff > 10)
            {
                diff = (int) (diff*0.1f);
            }

            _displayedScore += diff;
        }

        private ShipDefence ShipDefenceComponent
        {
            get
            {
                // Get component attached to the one the player is controlling.
                var entity = Scene.GetEntityWithComponent<ShipMovement>();
                return entity != null ? entity.GetComponent<ShipDefence>() : null;
            }
        }

        private float GetShipHealthPercentage()
        {
            var shipDefenceComponent = ShipDefenceComponent;

            if (shipDefenceComponent == null)
                return 0;

            return shipDefenceComponent.HealthPoints / (float)shipDefenceComponent.MaximumHealthPoints;
        }

        private string GetLabelText()
        {
            if (_gameDirector == null)
                return string.Empty;

            if (IsGameOver)
            {
                return string.Format("");
            }

            var powerupText = string.Empty;

            var shipFiring = Scene.GetComponent<ShipFiring>();
            if (shipFiring != null)
            {
                powerupText += string.Format("Fire rate: {0}", shipFiring.FireRate);
            }


            return string.Format("Current level: {1}, asteroids: {0}, lives: {2} {3}", 
                _gameDirector.AsteroidCount,
                _gameDirector.CurrentLevel,
                _gameDirector.LivesRemaining,
                powerupText);
        }

        private bool IsGameOver
        {
            get { return _gameDirector.LivesRemaining < 0; }
        }
    }
}