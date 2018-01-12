
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Factory
    {
        private Game m_game;
        private HashSet<string> m_freeKnightName;
        private HashSet<string> m_takenKnightName;
        private HashSet<string> m_freeSamouraiName;
        private HashSet<string> m_takenSamouraiName;
        public Factory(Game g)
        {
            m_game = g;
            m_freeKnightName= new HashSet<string>();
            m_takenKnightName = new HashSet<string>();
            m_freeSamouraiName = new HashSet<string>();
            m_takenSamouraiName = new HashSet<string>();
            m_freeSamouraiName.Add("Ukita nakayori");
            m_freeSamouraiName.Add("Gosanke shigesane");
            m_freeSamouraiName.Add("Kagami suekatsu");
            m_freeSamouraiName.Add("Hisamatsu takemochi");
            m_freeSamouraiName.Add("Morikawa nakahiro");
            m_freeSamouraiName.Add("Soejima mitsutoki");
            m_freeSamouraiName.Add("Ochi tanenao");
            m_freeSamouraiName.Add("Kitanokouji michikuni");
            m_freeSamouraiName.Add("Nawa yoshinari");
            m_freeSamouraiName.Add("Nijou hikotake");
            m_freeSamouraiName.Add("Kataoka toshimori");
            m_freeSamouraiName.Add("Matsuda mitsutame");
            m_freeSamouraiName.Add("Torii toshimitsu");
            m_freeSamouraiName.Add("Akiyama nagataka");
            m_freeSamouraiName.Add("Yamaguchi tsunetame");
            m_freeSamouraiName.Add("Hirano hirouji");
            m_freeSamouraiName.Add("Tanegashima tsuneyori");
            m_freeSamouraiName.Add("Goto yukiuji");
            m_freeSamouraiName.Add("Satomi motonari");
            m_freeSamouraiName.Add("Iwasaki nariatsu");
            m_freeSamouraiName.Add("Akita koresuke");
            m_freeSamouraiName.Add("Ichijo sukehide");
            m_freeSamouraiName.Add("Haga mitsutake");
            m_freeSamouraiName.Add("Chikusa moronao");
            m_freeSamouraiName.Add("Taguchi iemitsu");
            m_freeSamouraiName.Add("Matsui hisamichi");
            m_freeSamouraiName.Add("Itagaki sueteru");
            m_freeSamouraiName.Add("Ina mototaka");
            m_freeSamouraiName.Add("Fujioka michimoto");
            m_freeSamouraiName.Add("Akabashi hidesue");

            m_freeKnightName.Add("Tammie the Tracker");
            m_freeKnightName.Add("Jowell the Young");
            m_freeKnightName.Add("Jan of the West");
            m_freeKnightName.Add("Robin the Loyal");
            m_freeKnightName.Add("Geve the Poor");
            m_freeKnightName.Add("Gervesot the Quick");
            m_freeKnightName.Add("Frederic the Dreamer");
            m_freeKnightName.Add("Rab the Patriot");
            m_freeKnightName.Add("Williame the Wild");
            m_freeKnightName.Add("Eudo the Loyal");
            m_freeKnightName.Add("Ludovicus the Bear");
            m_freeKnightName.Add("How the Defender");
            m_freeKnightName.Add("Benger the Confident");
            m_freeKnightName.Add("Houdart the Wild");
            m_freeKnightName.Add("Althalos the Caring");
            m_freeKnightName.Add("Wiscar of the Ice");
            m_freeKnightName.Add("Godefroy the Pygmy");
            m_freeKnightName.Add("Gislebertus the Tough");
            m_freeKnightName.Add("Edun the Stubborn");
            m_freeKnightName.Add("Carle the Noble");
            m_freeKnightName.Add("Tybout the Tracker");
            m_freeKnightName.Add("Ausout of the Winter");
            m_freeKnightName.Add("HarveyBreton the Illuminator");
            m_freeKnightName.Add("Mosseus the Cute");
            m_freeKnightName.Add("Droet the Warm");
            m_freeKnightName.Add("Salaman the Brave");
            m_freeKnightName.Add("Dicky the Honorable");
            m_freeKnightName.Add("Bardolf the Quick");
            m_freeKnightName.Add("Thierri the Magnificent");
            m_freeKnightName.Add("Kit the Champion");

        }

        private string GetSamouraiName()
        {
            foreach (string name in m_freeSamouraiName)
            {
                m_freeSamouraiName.Remove(name);
                m_takenSamouraiName.Add(name);
                return name;
            }
            return "Default Samourai Name";
        }
        private void FreeSamouraiName(string s)
        {
            m_freeSamouraiName.Add(s);
            m_takenSamouraiName.Remove(s);
        }
        private string GetKnightName()
        {
            foreach (string name in m_freeKnightName)
            {
                m_freeKnightName.Remove(name);
                m_takenKnightName.Add(name);
                return name;
            }
            return "Default Knight Name";
        }
        private void FreeKnightName(string s)
        {
            m_freeSamouraiName.Add(s);
            m_takenSamouraiName.Remove(s);
        }
        public Character CreateSamourai(Team t)
        {
            Samourai item = new Samourai(t);
            if (m_game.GameConfiguration != null)
            {
                int maxStamina = 0;
                if (Int32.TryParse(m_game.GameConfiguration.GetItemValue("SamouraiStamina"), out maxStamina))
                {
                    if (maxStamina>0)
                        item.StaminaMax = maxStamina;
                }
                int speed = 0;
                if (Int32.TryParse(m_game.GameConfiguration.GetItemValue("SamouraiSpeed"), out speed))
                {
                    if (speed > 0)
                        item.Speed= speed;
                }
                item.ChangeName(GetSamouraiName());
            }
            return item;
        }
        public Character CreateKnight(Team t)
        {
            Knight item = new Knight(t);
            if (m_game.GameConfiguration != null)
            {
                int maxStamina = 0;
                if (Int32.TryParse(m_game.GameConfiguration.GetItemValue("KnightStamina"), out maxStamina))
                {
                    if (maxStamina > 0)
                        item.StaminaMax = maxStamina;
                }
                int speed = 0;
                if (Int32.TryParse(m_game.GameConfiguration.GetItemValue("KnightSpeed"), out speed))
                {
                    if (speed > 0)
                        item.Speed = speed;
                }
                item.ChangeName(GetKnightName());
            }
            return item;
        }
        //to destroy a character, we have to clean the entire game
        private void DestroyCharacter(Character c)
        {
            //removing grahical object
            if (c is Samourai)
                FreeSamouraiName(c.Name);
            if (c is Knight)
                FreeKnightName(c.Name);
            c.CleanGraphical();
            
        }
        //to destroy a Team, we have to clean the entire game
        public void DestroyTeam(Team t)
        {
            t.SpawnPoint.CleanGraphical();
            t.Flag.CleanGraphical();
            foreach (Character c in t.Characters)
            {
                DestroyCharacter(c);
            }
            t.Target = null;
            t.Flag = null;
            t.SpawnPoint = null;
            t.CleanGraphical();
            Material.Destroy(t.Material);
        }

    }
}
