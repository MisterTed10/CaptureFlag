using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Core
{
    public class SpawnPoint
    {
        private Vector2         m_position;
        private Team            m_team;
        private GameObject      m_graphics;
        public SpawnPoint(Team t)
        {
            m_team = t;
            
            m_graphics = GameObject.CreatePrimitive(PrimitiveType.Plane);
            m_graphics.name = "Team " + t.Id + " spawn point";
            Position = new Vector2(0.0f, 0.0f);
            m_graphics.transform.localScale = new Vector3(0.2f, 1.0f, 0.2f);
            m_graphics.transform.parent = t.GraphicRoot.transform;
            Renderer renderer = m_graphics.GetComponent<Renderer>();
            Material matFlag= new Material(Shader.Find("Standard"))
            {
                color = Color.white
            };
            matFlag.SetFloat("_Mode", 2);
            matFlag.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            matFlag.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            matFlag.SetInt("_ZWrite", 0);
            matFlag.DisableKeyword("_ALPHATEST_ON");
            matFlag.EnableKeyword("_ALPHABLEND_ON");
            matFlag.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            matFlag.renderQueue = 3000;
            
            matFlag.mainTexture= Resources.Load("Token/Spawn") as Texture;
            Material[] mats = new Material[2];
            mats[0] = matFlag;
            mats[1] = t.Material;
            renderer.materials = mats;
        }
        public Vector2 Position
        {
            get
            {
                return m_position;
            }

            set
            {
                m_position = value;
                m_graphics.transform.localPosition = new Vector3(m_position.x, 0.0f, m_position.y);
            }
        }
        public void CleanGraphical()
        {
            GameObject.Destroy(m_graphics);
        }
    }

    public class Team
    {
        public enum TeamState
        {
            SEARCH,
            REACH,
            BRINGBACK
        };
        private ArrayList       m_characters;
        private Game            m_game;
        private Flag            m_flag;
        private Flag            m_target;
        private SpawnPoint      m_spawnPoint;
        private TeamState       m_state;
        private int             m_id;
        private Material        m_mat;
        private GameObject      m_graphicRoot;
        private GameObject      m_graphicCharacterRoot;

        public ArrayList Members
        {
            get { return (ArrayList)m_characters.Clone(); } 
        }// we give a copy for ReadOnly purpose
        public Flag Target
        {
            get
            {
                return m_target;
            }

            set
            {
                m_target = value;
                if (m_state == TeamState.SEARCH)
                    m_state = TeamState.REACH;

            }
        }
        public SpawnPoint SpawnPoint
        {
            get
            {
                return m_spawnPoint;
            }

            set
            {
                m_spawnPoint = value;
            }
        }
        public Flag Flag
        {
            get
            {
                return m_flag;
            }

            set
            {
                m_flag = value;
            }
        }
        public Material Material
        {
            get
            {
                return m_mat;
            }

            set
            {
                m_mat = value;
            }
        }
        public int Id
        {
            get
            {
                return m_id;
            }

            set
            {
                m_id = value;
            }
        }
        public TeamState State
        {
            get { return m_state; }
            private set { m_state = value; }
        }
        public Game Game
        {
            get { return m_game; }
            private set { m_game = value; }
        }
        public GameObject GraphicRoot { get { return m_graphicRoot; } }
        public GameObject GraphicCharacterRoot { get { return m_graphicCharacterRoot; } }
        public ArrayList Characters { get { return m_characters; } }

        public Team(Game g, int id, Color c)
        {
            m_id = id;
            m_game = g;
            m_characters = new ArrayList();
            m_state = TeamState.SEARCH;
            m_mat = new Material(Shader.Find("Standard"))
            {
                color = c,
                name= "Team "+id+" Material"
            };
            m_graphicRoot = new GameObject();
            m_graphicRoot.name = "Team " + id;
            m_graphicCharacterRoot = new GameObject();
            m_graphicCharacterRoot.name = "characters";
            m_graphicCharacterRoot.transform.parent = m_graphicRoot.transform;
            m_spawnPoint= new SpawnPoint(this);
            m_flag = new Flag(new Vector2(), this);
            m_target = null;
        }
        public void AddKnight(int count = 1)
        {
            
            for (int i = 0; i < count; i++)
                m_characters.Add(Game.Factory.CreateKnight(this));
        }
        public void AddSamourai(int count = 1)
        {
            for (int i = 0; i < count; i++)
                m_characters.Add(Game.Factory.CreateSamourai(this));
        }
        public bool IsInTeam(Character c)
        {
            foreach (Character current in m_characters)
            {
                if (c == current)
                    return true;
            }
            return false;
        }
        public bool ClosestAlly(Character src , out CharacterDistance[] allies)
        {
            allies = new CharacterDistance[1];
            if (!IsInTeam(src))
                return false;
            if (m_characters.Count==0)
            {
            return false;
            }
                
            allies= new CharacterDistance[m_characters.Count-1];// -1 because src shouldn't be in the list
            
            int index = 0;
            foreach (Character character in m_characters)
            {
                if (character == src) continue;
                allies[index].character = character;
                allies[index].distance = (src.Position-character.Position).magnitude;
                index++;
            }
            System.Array.Sort(allies);
            return true;
        }
        public void Start()
        {
            int index=0;
            // float angle=2 * Mathf.PI / m_characters.Count;
            foreach (Character character in m_characters)
            {
                character.Angle = Random.value * 2 * Mathf.PI;
                character.WakeUp();
                index++;
            }
        }
        public void CleanGraphical() 
        {
            GameObject.Destroy(m_graphicRoot);
        }
        public void NotifyEnemyFlagCaptured()
        {
            m_state = TeamState.REACH;
        }
        public void NotifyEnemyFlagTaken()
        {
            m_state = TeamState.BRINGBACK;
        }
        public void NotifyEnemyFlagDropped()
        {
            m_state = TeamState.REACH;
        }
    }
}