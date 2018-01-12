using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Core
{

    public class Map
    {
        private GameObject m_mapRoot;
        private GameObject m_mapGraphics;
        private Material m_material;
        private float m_width;
        private float m_height;
        public float Width
        {
            get { return m_width; }
            private set { m_width = value; }
        }
        public float Height
        {
            get { return m_height; }
            private set { m_height = value; }
        }
        public Material Material { get { return m_material; } }
        public Map(float width, float height, string backGroundTexture = "Map/DefaultBackGroundTexture")
        {
            m_width = width;
            m_height = height;
            m_mapRoot = new GameObject("map");
            m_mapRoot.transform.localPosition = new Vector3(0.0f, -1.0f, 0.0f);
            m_mapGraphics = GameObject.CreatePrimitive(PrimitiveType.Plane);
            m_mapGraphics.transform.parent = m_mapRoot.transform;
            m_mapGraphics.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            //0.2f because at creation, a plane is 5unit by 5 unit
            m_mapGraphics.transform.localScale = new Vector3(width * 0.5f * 0.2f, 1.0f, height * 0.5f * 0.2f);
            m_material = new Material(Shader.Find("Standard"));
            m_mapGraphics.GetComponent<Renderer>().material = m_material;
            m_material.SetFloat("_Glossiness", .0f);
            m_material.SetFloat("_Metallic", 1.0f);
            Texture tex = Resources.Load(backGroundTexture) as Texture;
            if (tex == null)
            {
                Debug.Log("Failed to load texture :" + backGroundTexture);
            }
            else
            {
                Debug.Log("background texture found!!");
            }
            m_material.mainTexture = tex;
        }
        public void CleanGraphics()
        {
            GameObject.Destroy(m_mapRoot);
        }
    }
}