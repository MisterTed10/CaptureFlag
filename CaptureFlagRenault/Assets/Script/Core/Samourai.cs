using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Samourai : Character
    {
        private static float visibilityRange=15.0f;
        private static Material SamouraiMaterial;
        private static float SamouraiGroupRange=3.0f;
        private float m_speed;
        private int randomness = 0;
        public Samourai(Team t, string name = "DefaultSamouraiName") : base (t)
        {
            if (SamouraiMaterial == null)
            {
                SamouraiMaterial = new Material(Shader.Find("Standard"));
                SamouraiMaterial.name = "SamouraiMaterial";
                SamouraiMaterial.color = Color.white;
                SamouraiMaterial.SetFloat("_Mode", 2);
                SamouraiMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                SamouraiMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                SamouraiMaterial.SetInt("_ZWrite", 0);
                SamouraiMaterial.DisableKeyword("_ALPHATEST_ON");
                SamouraiMaterial.EnableKeyword("_ALPHABLEND_ON");
                SamouraiMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                SamouraiMaterial.renderQueue = 3000;
                Texture tex = Resources.Load("Token/Samourai") as Texture;
                SamouraiMaterial.mainTexture = tex;
            }
            Renderer renderer = m_graphics.GetComponent<Renderer>();
            Material currentMat = renderer.material;
            Material[] mats = new Material[2];
            mats[0] = currentMat;
            mats[1] = SamouraiMaterial;
            renderer.material = SamouraiMaterial;
            //renderer.materials = mats; 
            //invert the 2 lines above for rendering the color of the team
            ChangeName(name);
            randomness = (int)(Random.value * 2);// define if he is leader or follower
            float.TryParse(Team.Game.GameConfiguration.GetItemValue("SamouraiSpeed"), out m_speed);

        }
        protected override void Move(float delta)
        {
            switch (Team.State )
            { 
                case Team.TeamState.SEARCH :
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
        {
           //could be different depending the team state
            Angle = Random.value * 2 * Mathf.PI;

        }
        public override float Visibility { get {return Samourai.visibilityRange;}}
        private void SearchForEnemyFlag(float delta)
        {                       
            /*
             * Strategy for finding the enemy flag:
             * regroup then if 2 or more allies are in range,  each pawn move forward in the map until the found the enemy Flag
             * if randomness==0 => it's a leader samourai
             * otherwise it's a follower
             */
            if (randomness != 0)
            {
                CharacterDistance[] allies;
                Team.ClosestAlly(this, out allies);
                if (allies.Length > 2)
                {
                    for (int i = 0; i < 2; i++)
                        if (allies[i].distance > Samourai.SamouraiGroupRange)
                        {
                            MoveTo(allies[i].character.Position, delta * m_speed);
                            return;
                        }
                }
            }
            MoveForward(delta* m_speed);

            Flag enemyFlag;
            if (IsSeeingPotentialTarget(out enemyFlag))
            {//we discover the enemy flag, all the team go for it
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
        {//same behavior that for going to the enemy flag except for the carrier

            
            if (Team.Target != null)
            {
                if (Team.Target.Carrier == this)
                {
                    MoveTo(Team.Flag.SpawnPosition, delta * m_speed);
                    CaptureTheFlag();
                }
                else
                {
                    MoveTo(Team.Target.Position, delta * m_speed);
                }
            }

        }
    }
}