using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Core
{
    public class Flag
    {
        private static float    deepLevel = 2.0f;
        private Vector2         m_spawnPosition;
        private Character       m_carrier;
        private GameObject      m_graphics;
        private Team            m_team;
        public Vector2 Position
        {
            get
            {
                if (m_carrier != null)
                    return m_carrier.Position;
                else
                    return m_spawnPosition;
            }
        }
        public Character Carrier
        {
            get
            {
                return m_carrier;
            }
        }
        public Vector2 SpawnPosition 
        {
            get { return m_spawnPosition; } 
            set { 
                m_spawnPosition = value;
                updatePosition();
            }
        }

        public Team Team { get { return m_team; } }
        public Flag(Vector2 spawn, Team t)
        {
            m_team=t;
            m_graphics = GameObject.CreatePrimitive(PrimitiveType.Plane);
            m_graphics.name = "Team " + t.Id + " flag";
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
            matFlag.mainTexture= Resources.Load("Token/Flag") as Texture;
            Material[] mats = new Material[2];
            mats[0] = matFlag;
            mats[1] = t.Material;
            renderer.materials = mats;
            m_spawnPosition=spawn;
        }
        public void Take(Character c)
        {
            m_carrier = c;
            m_team.Game.NotifyFlagTaken(this);
            updatePosition();
        }
        public void Drop()
        {
            m_team.Game.NotifyFlagDrop(this);
            m_carrier = null;
            updatePosition();
        }
        public void updatePosition()
        {
            if (m_carrier != null)
            {
                m_graphics.transform.position = new Vector3(m_carrier.Position.x, deepLevel, m_carrier.Position.y);
            }
            else
            {
                Vector3 v;
                v.x =  m_spawnPosition.x;
                v.z = m_spawnPosition.y;
                v.y = deepLevel;
                m_graphics.transform.position = v;
            }
        }
        public void CleanGraphical()
        {
            GameObject.Destroy(m_graphics);
        }
    }
}
