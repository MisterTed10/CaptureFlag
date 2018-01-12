
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Core
{

    public class Game
    {

        private Team[]                          m_teams;
        private GameConfigurator                m_configuration;
        private Map                             m_currentMap;
        private int                             m_elapsedSecondFromStart;
        private Factory                         m_factory;
        private Dictionary<Team, int>           m_points;
        private int                             m_pointsToWin;
        private string                          m_logs;


        public bool Running { get { return m_points.Count > 0; } }
        public string Log { get { return m_logs; } }
        public Game(string configurationPath = "")
        {
            m_factory = new Factory(this);
            m_configuration = new GameConfigurator();
            m_configuration.LoadFileConfiguration(configurationPath, true);
        }
        public GameConfigurator GameConfiguration
        {
            get { return m_configuration; }
            private set { m_configuration = value; }
        }
        public int ElapsedTime
        {
            get { return m_elapsedSecondFromStart; }
            private set { m_elapsedSecondFromStart = value; }
        }
        public Map Map
        {
            get { return m_currentMap; }
            private set { m_currentMap = value; }
        }
        public Factory Factory { get {return m_factory;}}
        public Dictionary<Team, int> Score { get { return m_points; } }
        public bool Initialize()
        {
           
            //we load the configuration file if he exist
            //Team Creations

            int.TryParse(m_configuration.GetItemValue("PointToWin"), out m_pointsToWin);

            
            if (!BuildTeams())
            {
                Debug.Log("Failed to build team during game initializing");
                return false;
            }
            //Map Creation
            float mapWidth=0.0f, mapHeight=0.0f;
            float.TryParse(m_configuration.GetItemValue("MapWidth"), out mapWidth);
            float.TryParse(m_configuration.GetItemValue("MapHeight"), out mapHeight);
            m_currentMap = new Map(mapWidth, mapHeight);
            // reset of the scoreboard
            if (m_points == null)
                m_points = new Dictionary<Team, int>();
            m_points.Clear();
            // trigger the start for each team
            foreach (Team team in m_teams)
            {
                m_points[team] = 0;
                team.Start();
            }

            return true;
        }
        public void Clean()
        {// clean the scene after endgame
            foreach (Team t in m_teams)
            {
                m_factory.DestroyTeam(t);
            }
            m_teams = null;
            m_configuration = null;
            m_currentMap.CleanGraphics();
            Material.Destroy(m_currentMap.Material);
            m_currentMap = null;
            m_points.Clear();
        }
        public bool IsSeeingAPotentialTarget(Character c, out Flag target)
        {
            target = null;
            foreach (Team t in m_teams)
            {
                if (c.Team == t)
                    continue;
                if ((t.Flag.Position - c.Position).magnitude < c.Visibility )
                {
                    target = t.Flag;
                    return true;
                }
            }
            return false;
        }
        public bool GetNearestEnemies(Character c, out ArrayList enemies)
        {
            enemies = new ArrayList();
            if (c == null) return false;
            foreach (Team team in m_teams)
            {
                if (team == c.Team) continue;// there is no enemy in our own team
                foreach (Character currentEnemy in team.Members)
                { 
                    if (!currentEnemy.IsAlive())continue;// we don't care about dead enemy
                    CharacterDistance charDist;
                    charDist.character=currentEnemy;
                    charDist.distance=(currentEnemy.Position-c.Position).magnitude;
                    enemies.Add(charDist);
                }
            }
            enemies.Sort();
            return true;
        }
        public void NotifyCombat(Character c1, Character c2) 
        {
            Debug.Log(c1.Name + " (Team " + c1.Team.Id + ") engage the fight with " + c2.Name + "(Team " + c2.Team.Id + ")");
        }
        public void NotifyKill(Character dead, Character killer, float respawnTime)
        {
            Debug.Log(killer.Name + " (Team " + killer.Team.Id + ") killed " + dead.Name + "(Team " + dead.Team.Id + "). He will be back in "+respawnTime+" secondes.");
            m_logs=(killer.Name + " (Team " + killer.Team.Id + ") killed " + dead.Name + "(Team " + dead.Team.Id + "). He will be back in " + respawnTime + " secondes.");
        }
        public void NotifyDamageInflicted(Character attacker, Character victim, int damage)
        {
            Debug.Log(attacker.Name + " (Team " + attacker.Team.Id + ") inflicted " + damage + " damage to " + victim.Name + "(Team " + victim.Team.Id + ")");
        }
        public void NotifyFlagTaken(Flag f)
        {
            Debug.Log("Team " + f.Team.Id+"'s flag have been take by " + f.Carrier.Name + "(Team " + f.Carrier.Team.Id + ")");
        }
        public void NotifyFlagDrop(Flag f)
        {
            Debug.Log("Team " + f.Team.Id + "'s flag have been take by " + f.Carrier.Name + "(Team " + f.Carrier.Team.Id + ")");
        }
        public void NotifyFlagCaptured(Flag f)
        {
            Debug.Log("Team " + f.Team.Id + "'s flag have been take by " + f.Carrier.Name + "(Team " + f.Carrier.Team.Id + ")");
            m_logs=("Team " + f.Team.Id + "'s flag have been take by " + f.Carrier.Name + "(Team " + f.Carrier.Team.Id + ")");
            m_points[f.Carrier.Team]++;
            if (m_points[f.Carrier.Team] >= m_pointsToWin)
            {//end of the game
                Debug.Log("the team " + f.Carrier.Team.Id + " win the party");
                m_logs=("the team " + f.Carrier.Team.Id + " win the party");
                // todo wait sec for display the winner and then clean and quit
                //cleaning
                Clean();
                Application.Quit();
            }
        }

        public void NotifyCharacterRespawn(Character c)
        {
            Debug.Log(c.Name+"(Team " + c.Team.Id + ") is back on the game");
            m_logs=(c.Name + "(Team " + c.Team.Id + ") is back on the game");
        }
        private bool BuildTeams()
        {
            int teamCount = 0;
            Int32.TryParse(m_configuration.GetItemValue("TeamCount"), out teamCount);

            m_teams = new Team[teamCount];
            for (int i = 0; i < teamCount; i++)
            {
                m_teams[i] = new Team(this, i, Color.gray);

                {//extract Spawnposition
                    string spawPositionString = m_configuration.GetItemValue("Team[" + i + "].SpawnPosition");
                    string[] tokenizeSpawnPos = spawPositionString.Split(' ');
                    if (tokenizeSpawnPos.Length == 2)
                    {

                        Vector2 outPos;
                        if (float.TryParse(tokenizeSpawnPos[0], out outPos.x) &&
                            float.TryParse(tokenizeSpawnPos[1], out outPos.y))
                        {
                            m_teams[i].SpawnPoint.Position = outPos;
                        }
                        else
                        {
                            Debug.Log("Failed to parse spawn position");
                        }
                    }
                    else
                    {
                        Debug.Log("Failed to extract spawn position");
                    }
                }
                {//extract Flag position
                    string spawPositionString = m_configuration.GetItemValue("Team[" + i + "].FlagPosition");
                    string[] tokenizeSpawnPos = spawPositionString.Split(' ');
                    if (tokenizeSpawnPos.Length == 2)
                    {

                        Vector2 outPos;
                        if (float.TryParse(tokenizeSpawnPos[0], out outPos.x) &&
                            float.TryParse(tokenizeSpawnPos[1], out outPos.y))
                        {
                            m_teams[i].Flag.SpawnPosition = outPos;
                        }
                        else
                        {
                            Debug.Log("Failed to parse spawn position");
                        }
                    }
                    else
                    {
                        Debug.Log("Failed to extract spawn position");
                    }
                }
                {//extract color
                    string colorString = m_configuration.GetItemValue("Team[" + i + "].Color");
                    string[] tokenizeColor = colorString.Split(' ');
                    if (tokenizeColor.Length == 3)
                    {

                        int[] colorComponent = new int[3];
                        if (Int32.TryParse(tokenizeColor[0], out colorComponent[0]) &&
                            Int32.TryParse(tokenizeColor[1], out colorComponent[1]) &&
                            Int32.TryParse(tokenizeColor[2], out colorComponent[2]))
                        {
                            m_teams[i].Material.color = new Color(colorComponent[0], colorComponent[1], colorComponent[2]);
                        }
                    }
                }
                int knightCount = 0;
                if (!Int32.TryParse(m_configuration.GetItemValue("Team[" + i + "].KnightCount"), out knightCount))
                {
                    Debug.Log("Failed to get the knight count of team " + i);
                    return false;
                }
                m_teams[i].AddKnight(knightCount);
                int samouraiCount = 0;
                if (!Int32.TryParse(m_configuration.GetItemValue("Team[" + i + "].SamouraiCount"), out samouraiCount))
                {
                    Debug.Log("Failed to get the samourai count of team " + i);
                    return false;
                }
                m_teams[i].AddSamourai(samouraiCount);
            }
            return true;
        }
    }
}
