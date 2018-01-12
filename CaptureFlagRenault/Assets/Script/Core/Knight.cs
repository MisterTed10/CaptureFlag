using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Core
{
    public class Knight : Character
    {
        private static float visibilityRange=15.0f;
        private static Material KnightMaterial;
        private int m_randomness = 0;
        private float m_speed;

        public Knight(Team t,string name="DefaultKnightName") : base(t)
        {
            m_randomness = (int)(Random.value * 2.0f);
            if (KnightMaterial == null)
            {
                KnightMaterial = new Material(Shader.Find("Standard"))
                {
                    color = Color.white
                };
                KnightMaterial.name = "KnightMaterial";
                KnightMaterial.SetFloat("_Mode", 2);
                KnightMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                KnightMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                KnightMaterial.SetInt("_ZWrite", 0);
                KnightMaterial.DisableKeyword("_ALPHATEST_ON");
                KnightMaterial.EnableKeyword("_ALPHABLEND_ON");
                KnightMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                KnightMaterial.renderQueue = 3000;
                Texture tex = Resources.Load("Token/Knight") as Texture;
                KnightMaterial.mainTexture = tex;
            }
            Renderer renderer = m_graphics.GetComponent<Renderer>();
            Material currentMat = renderer.material;
            Material[] mats = new Material[2];
            mats[0] = currentMat;
            mats[1] = KnightMaterial;
            renderer.material = KnightMaterial;
            //renderer.materials = mats; 
            //invert the 2 lines above for rendering the color of the team
            ChangeName(name);            
            float.TryParse(Team.Game.GameConfiguration.GetItemValue("KnightSpeed"), out m_speed);

        }
        public override float Visibility { get { return Knight.visibilityRange; } }
        protected override void Move(float delta)
        {
            switch (Team.State)
            {
                case Team.TeamState.SEARCH:
                    SearchForEnemyFlag(delta);
                    break;
                case Team.TeamState.REACH:
                    GoToEnemyFlag(delta);
                    break;
                case Team.TeamState.BRINGBACK:
                    BringBackEnemyFlag(delta);
                    break;
            }

        }
        protected override void SetRespawnTransform()
        {// could be different depending of team state
            switch (Team.State)
            {
                case Team.TeamState.SEARCH:
                case Team.TeamState.REACH:
                case Team.TeamState.BRINGBACK:
                    Angle = Random.value * 2 * Mathf.PI;
                    break;
            }
        }
        private void SearchForEnemyFlag(float delta)
        {            
            /*
             * Strategy for finding the enemy flag:
             * Knight try to defend their flag if they haven't discover the enemy flag location yet
             * Each pawn move forward in the map until the found the enemy Flag
             */
            if (Team.Flag.Carrier != null)
            {
                MoveTo(Team.Flag.Carrier.Position, delta * m_speed);
                return;
            }
            MoveForward(delta * m_speed);
            Flag enemyFlag;
            if (IsSeeingPotentialTarget(out enemyFlag))
            {//we discover the enemy flag, all the team fo for it
                Team.Target = enemyFlag;
            }
        }
        private void GoToEnemyFlag(float delta)
        {            
            if (Team.Target != null)
            {
                MoveTo(Team.Target.Position, delta * m_speed);
                TakeTheTarget();
            }
        }
        private void BringBackEnemyFlag(float delta)
        {//2 behavior: the carrier and an escort go back to the base / some wait at the flag position            
            if (Team.Target != null)
            {
                if (Team.Target.Carrier == this)
                {
                    MoveTo(Team.Flag.SpawnPosition, delta * m_speed);
                    CaptureTheFlag();
                }
                else
                {
                    if (m_randomness == 0)
                        MoveTo(Team.Target.Position, delta * m_speed);
                    else
                        MoveTo(Team.Target.SpawnPosition, delta * m_speed);
                }
                
            }
        }
    }
}